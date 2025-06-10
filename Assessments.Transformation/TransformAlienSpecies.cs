using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.AlienSpecies.Profiles;
using Assessments.Mapping.AlienSpecies.Source;
using Assessments.Shared.Helpers;
using Assessments.Transformation.Database.Fab4;
using Assessments.Transformation.Helpers;
using Assessments.Transformation.TextSanitizer;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Assessments.Transformation
{
    public class TransformAlienSpecies
    {
        private static Fab4Context _dbContext;

        private static readonly Dictionary<int, int>
            LinkSubSpeciesToSpeciesDictionary = new Dictionary<int, int>()
            {
                { 2637, 2120 },
                { 1027, 2354 },
                { 1026, 2354 },
                { 1835, 1825 },
                { 2206, 595  },
                { 2015, 830  },
                { 2016, 830  },
                { 2037, 830  },
                { 1370, 1070 },
                { 7356, 1070 },
                {  850, 7355 },
                { 6068, 1451 },
                { 2158, 464  },
                { 2157, 464 },
                { 2625, 2616 }
            };

        private static List<string> _targettingFields = new List<string>() { "Samkopiert lokalitet \\ økologi / kvantitet" };
        private static string _replaceWith = "**Innholdet er skjult av personvernhensyn. Vennligst ta kontakt med Artsdatabanken for mer info.**";

        public static async Task TransformDataModels(IConfigurationRoot configuration, bool upload)
        {
            await SetupDatabaseContext(configuration);

            Progress.ProgressBar = new ProgressBar(100, "Henter vurderinger", new ProgressBarOptions
            {
                DisplayTimeInRealTime = false,
                EnableTaskBarProgress = true
            });

            var excludedExpertGroups = new List<string> { "Testedyr", "Ikke-marine invertebrater" };

            var databaseAssessments = _dbContext.Assessments.AsNoTracking().Include(x => x.Attachments.Where(y => y.IsDeleted == false))
                .Where(x => (bool)!x.IsDeleted && !excludedExpertGroups.Contains(x.Expertgroup));

            var totalCount = await databaseAssessments.CountAsync();

            Progress.ProgressBar.Tick(0, $"Transformerer {totalCount:N0} vurderinger");
            Progress.ProgressBar.MaxTicks = totalCount;

            var sourceItems = new List<FA4>();
            var targetItems = new List<AlienSpeciesAssessment2023>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AlienSpeciesAssessment2023Profile>()).CreateMapper();

            foreach (var assessment in databaseAssessments)
            {
                var fa4 = JsonSerializer.Deserialize<FA4>(assessment.Doc);

                if (fa4 == null || AssessmentToBeExcluded(fa4))
                    continue;

                FixImpactCategory(fa4);

                // datasett 
                fa4.Attachmemnts = assessment.Attachments.Where(x => !x.IsDeleted).Select(x => new Attachment()
                {
                    Id = x.Id,
                    Description = !string.IsNullOrWhiteSpace(x.Description) ? x.Description : x.Name,
                    FileName = x.FileName,
                    MimeType = x.Type
                }).ToArray();


                fa4.Id = assessment.Id;
                sourceItems.Add(fa4);

                var transformedAssessment = mapper.Map<AlienSpeciesAssessment2023>(fa4);
                targetItems.Add(transformedAssessment);

                Progress.ProgressBar.Tick();
            }

            FixSubSpeciesLinkedToSpecies(targetItems, sourceItems);

            Progress.ProgressBar.Message = "Serialiserer og lagrer filer (tar litt tid)";

            var files = new Dictionary<string, string>
            {
                {
                    DataFilenames.AlienSpecies2023, JsonSerializer.Serialize(targetItems)
                },
                {
                    DataFilenames.AlienSpecies2023Temp, JsonSerializer.Serialize(sourceItems)
                }
            };

            foreach (var (key, value) in files)
            {
                await File.WriteAllTextAsync(Path.Combine(configuration.GetValue<string>("FilesFolder"), key), value);

                if (upload)
                    await Storage.UploadToBlob(configuration, key, value);
            }

            Progress.ProgressBar.Message = $"Transformering fullført, {sourceItems.Count} vurderinger ble lagret";
            Progress.ProgressBar.Dispose();
        }

        public static async Task UploadAttachments(IConfigurationRoot configuration, bool upload)
        {
            await SetupDatabaseContext(configuration);

            Progress.ProgressBar = new ProgressBar(100, "Henter vedlegg", new ProgressBarOptions
            {
                DisplayTimeInRealTime = false,
                EnableTaskBarProgress = true
            });

            var excludedExpertGroups = new List<string> { "Testedyr", "Ikke-marine invertebrater" };

            var databaseAttachments = _dbContext.Attachments
                .Include(x => x.Assessment)
                .Include(x => x.AttachmentFile)
                .AsNoTracking()
                .Where(x => (bool)!x.Assessment.IsDeleted
                            && !excludedExpertGroups.Contains(x.Assessment.Expertgroup)
                            && x.IsDeleted == false);

            var totalCount = await databaseAttachments.CountAsync();
            var attachmentCount = 0;

            Progress.ProgressBar.Tick(0, $"Laster opp {totalCount:N0} vedlegg");
            Progress.ProgressBar.MaxTicks = totalCount;

            foreach (var attachment in databaseAttachments)
            {
                var fa4 = JsonSerializer.Deserialize<FA4>(attachment.Assessment.Doc);

                if (fa4 == null || AssessmentToBeExcluded(fa4))
                    continue;

                if (upload)
                {
                    var sanitizedAttachmentFile = SanitizedAttachmentFile(attachment);

                    await Storage.UploadFile(configuration, DataFilenames.CalculateAlienSpecies2023AttachmentFilePath(attachment.Id, attachment.FileName), sanitizedAttachmentFile);
                }

                attachmentCount++;
                Progress.ProgressBar.Tick();
            }

            Progress.ProgressBar.Message = $"Opplasting fullført, {attachmentCount} vedlegg ble lagret";
            Progress.ProgressBar.Dispose();
        }

        private static byte[] SanitizedAttachmentFile(Database.Fab4.Models.Attachment attachment)
        {
            var originalFile = attachment.AttachmentFile.File;
            
            if (attachment.FileName == "ArtskartData.zip" || !attachment.FileName.EndsWith(".csv"))
            {
                return originalFile;
            }

            
            var recordsList = CsvFileHandler.ReadFile(originalFile);
            if (recordsList == null || recordsList.Count() == 0)
                return originalFile;
            var hasField = false;
            var recordDict = (IDictionary<string, object>)recordsList.First();
            foreach (var fieldName in _targettingFields.Where(fieldName => recordDict.ContainsKey(fieldName)))
            {
                hasField = true;
            }

            if (!hasField)
            {
                return originalFile;
            }

            var sanitizedRecordsList = Sanitation.Sanitize(recordsList, _targettingFields, _replaceWith, out bool anyThingReplaced);
            
            if (anyThingReplaced == false)
            {
                return originalFile;
            }
            
            return CsvFileHandler.WriteFile(sanitizedRecordsList);
        }


        /// <summary>
        /// Check extra Criteria for excluding an assessment
        /// </summary>
        /// <param name="fa4">AlienSpecies assessment</param>
        /// <returns></returns>
        private static bool AssessmentToBeExcluded(FA4 fa4)
        {
            Progress.ProgressBar.Tick();

            // ekskluderer vurderinger som ikke har kategori
            if (string.IsNullOrEmpty(fa4.Category))
                return true;

            // ekskluderer vurderinger som ikke er ferdigstilte + ikke horisonstskannet
            if (!fa4.EvaluationStatus.Equals("finished", StringComparison.OrdinalIgnoreCase) && !fa4.HorizonDoScanning)
                return true;

            // ekskluderer vurderinger som er "ikke fremmed" i 2023 og 2018
            if (fa4.AlienSpeciesCategory == "NotAlienSpecie" &&
                fa4.PreviousAssessments.FirstOrDefault(x => x.RevisionYear == 2018) is
                    { MainCategory: "NotApplicable", MainSubCategory: "notAlienSpecie" })
            {
                return true;
            }

            // ekskluderer vurderinger med pattedyr som er husdyrraser
            if (new[] { 4937, 4963, 4964, 4965, 5217 }.Contains(fa4.Id))
                return true;

            return false;
        }

        private static void FixImpactCategory(FA4 assessment)
        {
            if (!assessment.HorizonDoScanning)
                return;

            assessment.Category = "NR";
            assessment.AssessmentConclusion = "WillNotBeRiskAssessed";
            assessment.AlienSpeciesCategory = "HorizonScannedButNoRiskAssessment";
        }

        private static void FixSubSpeciesLinkedToSpecies(List<AlienSpeciesAssessment2023> targetItems, List<FA4> sourceItems)
        {
            // lag relasjon mellom underart og art der arten skal representere kategori
            foreach (var targetItem in targetItems)
            {
                if (!LinkSubSpeciesToSpeciesDictionary.ContainsKey(targetItem.Id))
                {
                    continue;
                }

                // overfør informasjon fra hovedvurderingen
                // også for sourceitems slik at livetransform også virker
                var parentAssessment =
                    targetItems.SingleOrDefault(x => x.Id == LinkSubSpeciesToSpeciesDictionary[targetItem.Id]);
                Debug.Assert(parentAssessment != null, nameof(parentAssessment) + " != null");
                targetItem.Category = parentAssessment.Category;
                targetItem.ParentAssessmentId = parentAssessment.Id;
                targetItem.DecisiveCriteria = parentAssessment.DecisiveCriteria;
                var sourceItem = sourceItems.SingleOrDefault(x => x.Id == targetItem.Id);
                Debug.Assert(sourceItem != null, nameof(sourceItem) + " != null");
                sourceItem.ParentAssessmentId = parentAssessment.Id;
                sourceItem.Category = parentAssessment.Category.ToString();
                sourceItem.Criteria = parentAssessment.DecisiveCriteria;
            }
        }

        private static async Task SetupDatabaseContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Fab4");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("ConnectionStrings:Fab4 (app secret) mangler");

            var optionsBuilder = new DbContextOptionsBuilder<Fab4Context>()
                .UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(120));

            _dbContext = new Fab4Context(optionsBuilder.Options);

            if (!await _dbContext.Database.CanConnectAsync())
                throw new Exception("Kan ikke koble til databasen");
        }
    }
}