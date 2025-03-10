using System.ComponentModel.DataAnnotations;

namespace Assessments.Web.Infrastructure.RedlistSpecies.Enums
{
    public enum RedlistSpeciesAssessment2021Regions
    {
        [Display(Name = "Østfold")]
        Ost,
        
        [Display(Name = "Oslo og Akershus")]
        Ooa,

        [Display(Name = "Hedmark")]
        Hed,

        [Display(Name = "Oppland")]
        Opp,

        [Display(Name = "Buskerud")]
        Bus,

        [Display(Name = "Vestfold")]
        Ves,
        
        [Display(Name = "Telemark")]
        Tel,

        [Display(Name = "Aust-Agder")]
        Aus,

        [Display(Name = "Vest-Agder")]
        VeA,

        [Display(Name = "Rogaland")]
        Rog,

        [Display(Name = "Hordaland")]
        Hor,

        [Display(Name = "Sogn og Fjordane")]
        SoF,

        [Display(Name = "Møre og Romsdal")]
        MoR,

        [Display(Name = "Trøndelag")]
        Tron,

        [Display(Name = "Nordland")]
        Nor,

        [Display(Name = "Troms")]
        Tro,

        [Display(Name = "Finnmark")]
        Fin,

        [Display(Name = "Svalbard")]
        Sva,

        [Display(Name = "Jan Mayen")]
        JaM,

        [Display(Name = "Skagerrak og Nordsjøen")]
        NoSj,

        [Display(Name = "Norskehavet")]
        NoHa,

        [Display(Name = "Barentshavet sør")]
        BaSo,

        [Display(Name = "Barentshavet nord og Polhavet")]
        BaNoP,

        [Display(Name = "Grønlandshavet")]
        Gro,
    }
}
