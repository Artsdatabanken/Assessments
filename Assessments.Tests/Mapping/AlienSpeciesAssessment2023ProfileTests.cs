using Assessments.Mapping.AlienSpecies.Profiles;
using AutoMapper;

namespace Assessments.Tests.Mapping;

public class AlienSpeciesAssessment2023ProfileTests
{
    [Fact]
    public void AlienSpeciesAssessment2023ProfileShouldBeValid()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new AlienSpeciesAssessment2023Profile()));

        mapper.AssertConfigurationIsValid();
    }

    [Fact]
    public void AlienSpeciesAssessment2023ExportProfileShouldBeValid()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new AlienSpeciesAssessment2023ExportProfile()));

        mapper.AssertConfigurationIsValid();
    }
}