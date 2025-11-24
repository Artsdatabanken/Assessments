using System.ComponentModel;

namespace Assessments.Mapping.NatureTypes.Model;

public class NatureTypeAssessmentExport
{
    [DisplayName("Id for vurderingen")]
    [Description("Intern ID for vurderingen")]
    public int Id { get; set; }

    [DisplayName("Vurderingsenhet")]
    [Description("Navn på naturtypen")]
    public string Name { get; set; }

    [DisplayName("Kortnavn")]
    [Description("Kortnavn for naturtypen")]
    public string PopularName { get; set; }

    [DisplayName("Kategori")]
    [Description("Endelig rødlistekategori for naturtypen")]
    public string Category { get; set; }

    [DisplayName("Endelig kategori og kriterium")]
    [Description("Endelig rødlistekategori for naturtypen, og angivelse av hvilke(t) kriterium som er utslagsgivende for endelig rødlistekategori. Hvis feltet er tomt er kategori angitt direkte av ekspertkomiteen uten å angi informasjon for hvert enkelt kriterium, og kriterievalget er forklart i feltet \"Kriteriedokumentasjon\" (i de aller fleste tilfeller er disse LC, DD eller NE).")]
    public string CategoryCriteria { get; set; }

    [DisplayName("Alle kategorier og kriterier")]
    [Description("Alle kategorier og kriterier angitt for naturtypen. Kun høyeste kategori med tilhørende kriterium er gjeldende. Feltet er gruppert etter rødlistekategori etterfulgt av tilhørende kriterier. Hver kategori er separert med \";\", mens innenfor hver kategorigruppering er kriteriene separert med \"+\". Se metodeveileder for terskelverdier og bruk av katgorier og kriterier.")]
    public string CriteriaSummary { get; set; }

    [DisplayName("NiN-langkode")]
    [Description("NiN-langkode for NiN-enheten som definerer den primære naturtypen (se også kolonne \"Anvendte variabler\" for eventuell definisjon av sekundære naturtyper avledet av den primære naturtypen)")]
    public string LongCode { get; init; }

    [DisplayName("NiN-kode")]
    [Description("NiN-kortkode for NiN-enheten som definerer den primære naturtypen")]
    public string ShortCode { get; init; }

    [DisplayName("Ekspertkomitée")]
    [Description("Hvilken ekspertkomite som har utført vurderingen (se Rødlista for naturtyper 2025 på nett for oversikt over ekspertkomiteene og deres sammensetning)")]
    public string CommitteeName { get; set; }

    [DisplayName("Vurderingsområde")]
    [Description("Angir om vurderingen er gjort for Fastlands-Norge med havområder eller Svalbard")]
    public string Region { get; set; }

    [DisplayName("Anvendte variabler")]
    [Description("Angir NiN-kortkode og trinn for eventuelle variabler som er brukt for å definere sekundære naturtyper i tilfeller hvor det er stor biologisk variasjon internt i den primære naturtypen (Se metodeveileder for rødlista for naturtyper 2025). Hvis tomt; ingen variabler er anvendt for å definere sekundære naturtyper. hvis flere variabler er anvendt er disse separert med \";\". NiN-kortkoden er angitt med strukturen \"XX-YY_t\" hvor XX viser til egenskapskategorien (gruppe av variabler i NiN), mens YY viser til den enkelte variabelen, og \"t\" viser til hvilke(t) trinn av variabelen som definerer den sekundære naturtypen, separert med \"&\". For eksempel: SA-HA_KA;RM-MS_b&c&d viser til to variabler med tilhørende trinn;  1) variabelen SA-HA_KA angir trinn KA av variabelen SA-HA Naturgitt dominerende stasjonær megafauna, i egenskapskategorien SA (Strukturerende og funksjonelle artsgrupper); mens 2) variabelen RM-MS_b&c&d angir trinnene b,c og d av variabelen RM-MS Marine bioklimatiske soner i egenskapskategorien RM (Regional miljøvariabel).")]
    public string AppliedVariables { get; set; }

    [DisplayName("Nin kode tema hovedtype")]
    [Description("Angir for naturtyper i typesystemet Natursystem navnet på hovedtypen naturtypen inngår i, eller landformtypesystem for naturtypene i gruppen landformer.")]
    public string NinCodeTopicName { get; set; }

    [DisplayName("Nin kode tema hovedtypegruppe")]
    [Description("Angir for naturtyper i typesystemet Natursystem navnet på hovedtypegruppen naturtypen inngår i, eller om det er en naturtype i gruppen landformer.")]
    public string NinCodeTopicDescription { get; set; }

    [DisplayName("Beskrivelse av vurderingsenheten")]
    [Description("Ekspertkomiteens beskrivelse av naturtypen.")]
    public string DescriptionHtml { get; set; }

    [DisplayName("Kriteriedokumentasjon")]
    [Description("Ekspertkomiteens forklaringer og dokumentasjon av grunnlag og bruk av kriteriesettet for angivelse av rødlistekategori.")]
    public string CriteriaDocumentationHtml { get; set; }

    [DisplayName("Påvirkningsfaktor")]
    [Description("Ekspertkomiteens forklaringer og beskrivelser av påvirkningsfaktorer som påvirker naturtypen")]
    public string ImpactsCommentHtml { get; set; }

    [DisplayName("Påvirkningsfaktorer")]
    [Description("De(n) viktigste påvirkningsfaktoren(e) som påvirker naturtypen, med tidspunkt omfang og alvorlighetsgrad for påvirkningen. Påvirkningsfaktoren er angitt fra et predefinert hierarki av potensielle påvirkningsfaktorer (se Rødlista på nett), og påvirkningsfaktorens navn og plassering i hierarkiet angis her fra høyt hierarkisk nivå ned til det angitte nivået. For eksempel viser \"Påvirkning på habitat > Landbruk > Jordbruk\" til påvirkningsfaktoren \"Jordbruk\" i gruppen \"Landbruk\" i hovedgruppen \"Påvirkning på habitat\". Videre er hver påvirkningsfaktors tidspunk (f.eks. \"pågåede\" eller \"historisk\"), omfang (andel av naturtypens areal som påvirkes), og alvorlighetsgrad (andel reduksjon av naturtypens areal over en tidsperiode) angitt, separert med \"_\". Hver ulik påvirkningfaktor er separert med \";\".")]
    public string CodeitemsList { get; set; }

    [DisplayName("Areal kommentar")] 
    [Description("Ekspertkomiteens beskrivelse av naturtypens utbredelse")]
    public string AreaInformationCommentHtml { get; set; }

    [DisplayName("Kjent totalareal (km2)")]
    [Description("Naturtypens kjente totalareal, altså arealet i kvadratkilometer som dekkes av naturtypen.")]
    public decimal? AreaInformationTotalArea { get; set; }

    [DisplayName("Mørketall totalareal")] 
    [Description("Mørketallet beskriver usikkerheten i det kjente totalarealet. Mørketall = 1 betyr at naturtypens totalareal er fullstendig kjent og uten usikkerhet, mens høyere mørketall viser til større usikkerhet og antatt større totalareal. Mørketallet multipliseres med det kjente arealtallet for å estimere beregnet totalareal.")]
    public decimal? AreaInformationTotalAreaMultiplier { get; set; }

    [DisplayName("Beregnet totalareal (km2)")]
    [Description("Naturtypens beregnede totalareal, beregnet som kjent totalareal multiplisert med mørketallet.")]
    public decimal? AreaInformationTotalAreaCalculated { get; set; }

    [DisplayName("Kjent utbredelsesareal (km2)")]
    [Description("Naturtypens kjente utbredelsesareal i kvadratkilometer, altså arealet av det minste konvekse polygon som omslutter kjente forekomster av naturtypen.")]
    public decimal? AreaInformationExtentArea { get; set; }

    [DisplayName("Mørketall utbredelsesareal")]
    [Description("Mørketallet beskriver usikkerheten i det kjente utbredelsesarealet. Mørketallet multipliseres med det kjente arealtallet for å estimere Beregnet utbredelsesareal. Mørketall = 1 betyr at naturtypens utbredelsesareal er fullstendig kjent og uten usikkerhet, mens høyere mørketall viser til større usikkerhet og antatt større utbredelsesareal.")]
    public decimal? AreaInformationExtentAreaMultiplier { get; set; }

    [DisplayName("Beregnet utbredelsesareal (km2)")]
    [Description("Naturtypens beregnede utbredelsesareal i kvadratkilometer, beregnet som kjent utbredelsesareal multiplisert med mørketallet.")]
    public decimal? AreaInformationExtentAreaCalculated { get; set; }

    [DisplayName("Antall forekomstruter")]
    [Description("Naturtypens kjente antall forekomstruter, dvs antallet ruter av 10x10 km hvor naturtypen forekommer.")]
    public decimal? AreaInformationLocalityArea { get; set; }

    [DisplayName("Mørketall forekomster")]
    [Description("Beskriver usikkerheten i det angitte antallet kjente forekomstruter. Mørketallet multipliseres med antallet forekomstruter, og med 100km2 for å beregne beregnet forekomstareal. Mørketall = 1 betyr at naturtypens forekomstruter er er fullstendig kjent og uten usikkerhet, mens høyere mørketall viser til større usikkerhet og antatt flere forekomstruter.")]
    public decimal? AreaInformationLocalityAreaMultiplier { get; set; }

    [DisplayName("Beregnet forekomstareal (km2)")]
    [Description("Naturtypens beregnede forekomstareal, dvs samlet areal av forekomstrutene hvor naturtypen forekommer, beregnet som produktet av antall kjente forekomstruter, 100km2 og mørketallet.")]

    public decimal? AreaInformationLocalityAreaCalculated { get; set; }
    
    [DisplayName("Regioner og havområder")]
    [Description("Angivelse av regioner og havområder med kjente forekomster av naturtypen, separert med \";\"")]
    public string RegionsList { get; set; }

    [DisplayName("Referanser")]
    [Description("Referanser til bakgrunnsinformasjon og litteratur som ligger til grunn for vurderingen, separert med \"|\"")]
    public string ReferencesList { get; set; }

    [DisplayName("Sitering")]
    [Description("Sitering av naturtypens rødlistevurdering.")]
    public string Citation { get; set; }
}