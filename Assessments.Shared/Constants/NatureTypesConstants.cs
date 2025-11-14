namespace Assessments.Shared.Constants;

public static class NatureTypesConstants
{
    public const string Title2025 = "Norsk rødliste for naturtyper 2025";
    
    public static readonly DateTime PublishedDate = new(2025, 11, 26);
    
    public const string CitationSummary = $"{Title2025}. Artsdatabanken.";

    public static class Headings
    {
        public static readonly string Description = "Beskrivelse av naturtypen";
        public static readonly string Summary = "Oppsummering";
        public static readonly string CodeItems = "Påvirkningsfaktorer";
        public static readonly string CriteriaInformation = "Vurderingskriteriene";
        public static readonly string AreaInformation = "Naturtypens areal";
        public static readonly string Regions = "Regioner og havområder";
        public static readonly string References = "Referanser";
    }

    // TODO: rln fjern etter lansering
    public const string TemporaryAccessCookieName = "adb.req.enablenaturetypes";
}