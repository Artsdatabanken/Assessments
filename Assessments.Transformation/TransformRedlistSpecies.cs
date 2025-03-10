using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assessments.Mapping.RedlistSpecies;
using Assessments.Mapping.RedlistSpecies.Profiles;
using Assessments.Mapping.RedlistSpecies.Source;
using Assessments.Shared.Helpers;
using Assessments.Transformation.Database.Rodliste2020;
using Assessments.Transformation.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShellProgressBar;

namespace Assessments.Transformation
{
    public static class TransformRedlistSpecies
    {
        private static Rodliste2020Context _dbContext;

        public static async Task TransformDataModels(IConfigurationRoot configuration, bool upload = false)
        {
            var connectionString = configuration.GetConnectionString("Rodliste2020");

            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("ConnectionStrings:Rodliste2020 (app secret) mangler");

            var optionsBuilder = new DbContextOptionsBuilder<Rodliste2020Context>()
                .UseSqlServer(connectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(120));

            _dbContext = new Rodliste2020Context(optionsBuilder.Options);

            if (!await _dbContext.Database.CanConnectAsync())
                throw new Exception("Kan ikke koble til databasen");

            Progress.ProgressBar = new ProgressBar(100, "Henter vurderinger", new ProgressBarOptions
            {
                DisplayTimeInRealTime = false,
                EnableTaskBarProgress = true
            });

            var databaseAssessments = _dbContext.Assessments.AsNoTracking().Where(x => (bool)!x.IsDeleted && x.Expertgroup != "Testarter" && x.Expertgroup != "Moser (Svalbard)");
            var revisions = await _dbContext.AssessmentRevisions.AsNoTracking().Include(x => x.AssessmentHistory).ToArrayAsync();

            var totalCount = await databaseAssessments.CountAsync();

            Progress.ProgressBar.Tick(0, $"Transformerer {totalCount:N0} vurderinger");
            Progress.ProgressBar.MaxTicks = totalCount;

            var rodliste2019Assessments = new List<Rodliste2019>();
            var speciesAssessment2021Mapper = new MapperConfiguration(cfg => cfg.AddProfile<SpeciesAssessment2021Profile>()).CreateMapper();
            var speciesAssessment2021Assessments = new List<SpeciesAssessment2021>();
            var revisionsTransformed = revisions.Select(x => new
            {
                Id = x.Id,
                Revision = x.RevisionId,
                RevDate = x.RevisionDateTime,
                FutureRevisionDate = x.FutureRevisionDateTime,
                Assessment = JsonConvert.DeserializeObject<Rodliste2019>(x.AssessmentHistory.Doc)
            }).GroupBy(x => x.Id).ToDictionary(x => x.Key);

            // NB! all modifisering - som senere f.eks. skal kunne bli med i live-transformering på plasseres på kildeobjekt! (note to self)

            foreach (var item in databaseAssessments)
            {
                var id = item.Id; // id fra databasen
                var rodliste2019 = JsonConvert.DeserializeObject<Rodliste2019>(item.Doc);
                if (rodliste2019 != null)
                {
                    rodliste2019.Id = id.ToString();

                    // enhancement of source
                    rodliste2019.RevisionDate = new DateTime(2021, 11, 24);

                    if (revisionsTransformed.ContainsKey(id))
                    {
                        var list = revisionsTransformed[id].OrderBy(x => x.Revision).ToArray();
                        foreach (var innerItem in list)
                        {
                            rodliste2019.RevisionDate = innerItem.FutureRevisionDate;

                            var innerAssessment = innerItem.Assessment;
                            innerAssessment.RevisionDate = innerItem.RevDate;
                            innerAssessment.Revision = innerItem.Revision;
                            innerAssessment.Id = rodliste2019.Id;

                            if (rodliste2019.Revisions == null) rodliste2019.Revisions = new List<Rodliste2019>();
                            rodliste2019.Revisions.Add(innerAssessment);
                        }
                    }

                    var transformedAssessment = speciesAssessment2021Mapper.Map<SpeciesAssessment2021>(rodliste2019);

                    speciesAssessment2021Assessments.Add(transformedAssessment);
                }

                //var assessment = JsonConvert.DeserializeObject<Rodliste2019>(item.Doc);
                if (rodliste2019 != null)
                {
                    rodliste2019Assessments.Add(rodliste2019);
                }

                Progress.ProgressBar.Tick();
            }

            Progress.ProgressBar.Message = "Serialiserer og lagrer filer (tar litt tid)";


            var files = new Dictionary<string, string>
            {
                {
                    DataFilenames.Species2021, JsonConvert.SerializeObject(speciesAssessment2021Assessments)
                },
                {
                    DataFilenames.Species2021Temp, JsonConvert.SerializeObject(rodliste2019Assessments)
                }
            };

            foreach (var (key, value) in files)
            {
                await File.WriteAllTextAsync(Path.Combine(configuration.GetValue<string>("FilesFolder"), key), value);

                if (upload)
                    await Storage.UploadToBlob(configuration, key, value);
            }

            Progress.ProgressBar.Message = "Transformering fullført";
            Progress.ProgressBar.Dispose();
        }
    }
}
