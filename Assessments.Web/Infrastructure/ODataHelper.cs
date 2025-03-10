using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.RedlistSpecies;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Assessments.Web.Infrastructure;

public static class ODataHelper
{
    public static void Options(ODataOptions options)
    {
        options.EnableQueryFeatures().SetMaxTop(100).AddRouteComponents("odata", GetModel());
    }

    private static IEdmModel GetModel()
    {
        var modelBuilder = new ODataConventionModelBuilder().EnableLowerCamelCase();

        modelBuilder.EntitySet<SpeciesAssessment2021>(nameof(SpeciesAssessment2021));
        modelBuilder.EntitySet<AlienSpeciesAssessment2023>(nameof(AlienSpeciesAssessment2023));

        var edmModel = modelBuilder.GetEdmModel();

        edmModel.MarkAsImmutable();

        return edmModel;
    }
}