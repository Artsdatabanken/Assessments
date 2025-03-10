using Assessments.Shared.Helpers;
using CsvHelper.Configuration.Attributes;

namespace Assessments.Web.Infrastructure.Services
{
    public class ExpertCommitteeMemberService
    {
        private readonly DataRepository _dataRepository;

        public ExpertCommitteeMemberService(DataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public async Task<List<string>> GetExpertCommitteeMembers(string expertCommitteeName, int year)
        {
            var data = await _dataRepository.GetData<ExpertCommitteeMember>(DataFilenames.SpeciesExpertCommitteeMembers);

            var expertCommitteeMembers = data.Where(x => x.ExpertCommittee.Equals(expertCommitteeName.Trim(), StringComparison.InvariantCultureIgnoreCase) && x.Year == year).ToList();
            
            if (!expertCommitteeMembers.Any()) // find members in composite commiteenames (comma separated)
            {
                expertCommitteeMembers = data.Where(x => x.Year == year && x.ExpertCommittee.Split(",", StringSplitOptions.TrimEntries)
                .Select(y => y.ToLowerInvariant()).Contains(expertCommitteeName.Trim().ToLowerInvariant()))
                    // remove duplicate names
                    .GroupBy(x => x.Name).Select(x => x.First()).ToList();
            }

            var results = expertCommitteeMembers.Select(x =>
            {
                x.ExpertCommittee = x.ExpertCommittee // remove assessmentarea from names in order to match mapped model
                    .Replace("(Svalbard)", string.Empty, StringComparison.InvariantCultureIgnoreCase)
                    .Replace("(Norge)", string.Empty, StringComparison.InvariantCultureIgnoreCase).Trim();
                return x;
            }).ToList();
            
            var sortedexpertCommitteeMembers = expertCommitteeMembers.OrderBy(x => new List<string> { "leder", "nestleder", "medlem" }.IndexOf(x.Role.ToLowerInvariant())).ThenBy(x => x.LastName).Select(x => $"{x.LastName} {x.FirstNameInitals}").Distinct().ToList();
            
            return sortedexpertCommitteeMembers;
        }
    }

    public class ExpertCommitteeMember
    {
        [Name("RodlisteAr")]
        public int Year { get; set; }

        public string ExpertCommittee { get; set; }

        public string SpeciesGroup { get; set; }

        [Name("EkspertgruppeRolle")]
        public string Role { get; set; }

        [Name("FulltNavn")]
        public string Name { get; set; }

        [Name("Etternavn")]
        public string LastName { get; set; }

        [Name("FornavnInit")]
        public string FirstNameInitals { get; set; }

        [Name("Institusjon")]
        public string Institution { get; set; }
    }
}
