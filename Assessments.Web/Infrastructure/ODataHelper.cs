using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.RedlistSpecies;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Assessments.Web.Infrastructure;

public static class ODataHelper
{
    public static IEdmModel GetModel()
    {
        var modelBuilder = new ODataConventionModelBuilder().EnableLowerCamelCase();

        modelBuilder.ContainerName = $"{nameof(Assessments)}Container";

        modelBuilder.EntitySet<SpeciesAssessment2021>(nameof(SpeciesAssessment2021));
        modelBuilder.EntitySet<AlienSpeciesAssessment2023>(nameof(AlienSpeciesAssessment2023));
        
        var rodlisteNaturtyperAssessment = modelBuilder.EntitySet<RodlisteNaturtyper.Data.Models.Assessment>("NatureTypeAssessment2025");

        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.State);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.CreatedOn);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.IsLocked);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.LockedBy);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.LockedById);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.ModifiedBy);
        rodlisteNaturtyperAssessment.EntityType.Ignore(m => m.ModifiedById);

        var edmModel = modelBuilder.GetEdmModel();

        edmModel.MarkAsImmutable();

        return edmModel;
    }
}