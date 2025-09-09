using Assessments.Mapping.AlienSpecies.Helpers;
using Assert = Xunit.Assert;

namespace Assessments.Tests.Mapping.Helpers
{
    public class AlienSpeciesAssessmentHelpersTests
    {
        [Fact]
        public void GetMedianLifetimeSimplifiedEstimationDefaultScoreBestTest()
        {
            var testAssessment = new Assessments.Mapping.AlienSpecies.Source.RiskAssessment
            {
                AOO50yrBestInput = 20,
                AOOtotalBestInput = 40
            };

            var result = AlienSpeciesAssessment2023ProfileHelper.GetMedianLifetimeSimplifiedEstimationDefaultScoreBest("NotAssessedDoorknocker", testAssessment);
            
            Assert.Equal(4, result);
        }
    }
}
