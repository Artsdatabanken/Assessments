using System.ComponentModel;

namespace Assessments.Mapping.NatureTypes.Model;

public class NatureTypeAssessmentExport
{
    [DisplayName("Id for vurderingen")]
    [Description("Id for 2025 vurderingen")]
    public int Id { get; set; }

    [DisplayName("Vurderingsenhet")]
    [Description("")]
    public string Name { get; set; }

    [DisplayName("Kategori")]
    [Description("")]
    public string Category { get; set; }

    [DisplayName("Endelig kategori og kriterium")]
    [Description("Her listes gjeldende kategori og kriterium")]
    public string CategoryCriteria { get; set; }

    [DisplayName("Alle kategorier og kriterier")]
    [Description("Oversikt over alle kategorier og kriterier angitt for naturtypen. Kun høyeste kategori med tilhørende kriterium er gjeldende.")]
    public string CriteriaSummary { get; set; }

    [DisplayName("NiN-langkode")]
    [Description("")]
    public string LongCode { get; init; }

    [DisplayName("NiN-kode")]
    [Description("")]
    public string ShortCode { get; init; }

    [DisplayName("Ekspertkomitée")]
    [Description("")]
    public string CommitteeName { get; set; }

    [DisplayName("Vurderingsområde")]
    [Description("")]
    public string Region { get; set; }

    [DisplayName("Anvendte variabler")]
    [Description("")]
    public string AppliedVariables { get; set; }

    [DisplayName("Nin kode tema hovedtype")]
    [Description("")]
    public string NinCodeTopicName { get; set; }

    [DisplayName("Nin kode tema hovedtypegruppe")]
    [Description("")]
    public string NinCodeTopicDescription { get; set; }

    [DisplayName("Beskrivelse av vurderingsenheten")]
    [Description("")]
    public string DescriptionHtml { get; set; }

    [DisplayName("Kriteriedokumentasjon")]
    [Description("")]
    public string CriteriaDocumentationHtml { get; set; }

    [DisplayName("Påvirkningsfaktor")]
    [Description("")]
    public string ImpactsCommentHtml { get; set; }

    [DisplayName("Påvirkningsfaktorer")]
    [Description("")]
    public string CodeitemsList { get; set; }

    [DisplayName("Areal kommentar")] 
    [Description("")]
    public string AreaInformationCommentHtml { get; set; }

    [DisplayName("Kjent totalareal (km2)")]
    [Description("")]
    public decimal? AreaInformationTotalArea { get; set; }

    [DisplayName("Mørketall totalareal")] 
    [Description("")]
    public decimal? AreaInformationTotalAreaMultiplier { get; set; }

    [DisplayName("Beregnet totalareal (km2)")]
    [Description("")]
    public decimal? AreaInformationTotalAreaCalculated { get; set; }

    [DisplayName("Kjent utbredelsesareal (km2)")]
    [Description("")]
    public decimal? AreaInformationExtentArea { get; set; }

    [DisplayName("Mørketall utbredelsesareal")]
    [Description("")]
    public decimal? AreaInformationExtentAreaMultiplier { get; set; }

    [DisplayName("Beregnet utbredelsesareal (km2)")]
    [Description("")]
    public decimal? AreaInformationExtentAreaCalculated { get; set; }

    [DisplayName("Antall forekomstruter (km2)")]
    [Description("")]
    public decimal? AreaInformationLocalityArea { get; set; }

    [DisplayName("Mørketall forekomster")]
    [Description("")]
    public decimal? AreaInformationLocalityAreaMultiplier { get; set; }

    [DisplayName("Beregnet antall forekomstruter")]
    [Description("")]
    public decimal? AreaInformationLocalityAreaCalculated { get; set; }
    
    [DisplayName("Regioner og havområder")]
    [Description("")]
    public string RegionsList { get; set; }

    [DisplayName("Referanser")]
    [Description("")]
    public string ReferencesList { get; set; }

    [DisplayName("Sitering")]
    [Description("")]
    public string Citation { get; set; }
}