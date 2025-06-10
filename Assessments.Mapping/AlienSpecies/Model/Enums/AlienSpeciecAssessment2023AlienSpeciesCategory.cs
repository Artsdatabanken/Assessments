using System.ComponentModel.DataAnnotations;
using Assessments.Shared.Resources.Enums.AlienSpecies;

namespace Assessments.Mapping.AlienSpecies.Model.Enums
{
    public enum AlienSpeciecAssessment2023AlienSpeciesCategory
    {
        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.alien_species), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        AlienSpecie,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.doorknocker), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        DoorKnocker,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.effect_without_reproduction), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))] //treated like a doorknocker and will be visualised with "Dørstokkart" in the filter meny, but use this displayname in the species assessment site. 
        EffectWithoutReproduction,

        /// <summary>
        /// These are only alien in parts of Norway (they are resident species of Norway) and are therefore excluded from the filter on assessment area (EvaluationContext). 
        /// </summary>
        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.regionally_alien), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        RegionallyAlien,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.not_alien_species), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        NotAlienSpecie,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.horizon_scanned_but_no_risk_assessment), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        HorizonScannedButNoRiskAssessment,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.evaluated_at_another_level), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        TaxonEvaluatedAtAnotherLevel,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.established_at_1800), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        UncertainBefore1800,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.regionally_alien_established_at_1800), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        RegionallyAlienEstablishedBefore1800,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.misidentified), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        MisIdentified,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.all_subtaxa_assessed_separately), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        AllSubTaxaAssessedSeparately,

        [Display(Name = nameof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource.hybrid_without_own_risk_assessment), ResourceType = typeof(AlienSpeciesAssessment2023AlienSpeciesCategoryResource))]
        HybridWithoutOwnRiskAssessment
    }
}