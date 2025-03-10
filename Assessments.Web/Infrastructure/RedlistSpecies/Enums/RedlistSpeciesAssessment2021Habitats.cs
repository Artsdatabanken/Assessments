using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.RedlistSpecies.Enums
{
    public enum RedlistSpeciesAssessment2021Habitats
    {
        [Display(Name = "Arktisk")]
        Arktisk,
        
        [Display(Name = "Fjell")]
        ArktiskAlpineJorddektFastmark,
        
        [Display(Name = "Berg og ur")]
        BergUrAndreGrunnjordsystemer,
        
        [Display(Name = "Skog")]
        Fastmarksskogsmark,
        
        [Display(Name = "Ferskvann")]
        Ferskvannssystemer,
        
        [Display(Name = "Fjæresone")]
        Fjæresone,
        
        [Display(Name = "Flomsone")]
        Flomsone,
        
        [Display(Name = "Snø og is")]
        IsSnø,
        
        [Display(Name = "Kyst")]
        KysttilknyttedeFastmarkssystemer,
        
        [Display(Name = "Saltvann")]
        Saltvannssystemer,
        
        [Display(Name = "Semi-naturlig mark")]
        Fastmark,
        
        [Display(Name = "Sterkt endret mark")]
        SterktEndretFastmark,
        
        [Display(Name = "Våtmark")]
        Våtmarkssystemer,
    }
}
