using System;
using System.Collections.Generic;
using System.ComponentModel;

// ReSharper disable InconsistentNaming

namespace Assessments.Mapping.RedlistSpecies
{
    /// <summary>
    /// Basert på Rodliste2019
    /// https://github.com/Artsdatabanken/Rodliste2019/blob/master/Prod.Domain/Rodliste2019.cs
    /// </summary>
    public class SpeciesAssessment2021
    {
        /// <summary>
        /// The unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Population reduction in the past 10 years or 3 generations (whichever is longer), where the causes of the reduction are clearly reversible and understood and have ceased.
        /// </summary>
        public SpeciesAssessment2021A1 A1 { get; set; } = new();

        /// <summary>
        /// Population reduction in the past 10 years or 3 generations (whichever is longer), where the causes of reduction may not have ceased or may not be understood or may not be reversible.
        /// </summary>
        public SpeciesAssessment2021A2 A2 { get; set; } = new();

        /// <summary>
        /// Population reduction over 10 years or 3 generations (whichever is longer) into the future.
        /// </summary>
        public SpeciesAssessment2021A3 A3 { get; set; } = new();

        /// <summary>
        /// Population reduction over a period of 10 years or 3 generations (whichever is longer) that includes both the past and the future.
        /// </summary>
        public SpeciesAssessment2021A4 A4 { get; set; } = new();

        /// <summary>
        /// Current population size as percentage of maximum population size after 1900, given as predefined interval within the assessed area for the species.
        /// </summary>
        public string ProportionOfMaxPopulation { get; set; } // AndelNåværendeBestand

        public SpeciesAssessment2021B1 B1 { get; set; } = new();

        public SpeciesAssessment2021B2 B2 { get; set; } = new();

        /// <summary>
        /// Whether or not the taxon is severely fragmented, and to what degree of certainty: "ja"= yes, "jaTrolig" = uncertain if severe fragmentation, "nei" = no.
        /// </summary>
        public string BaiSevereFragmentation { get; set; } // BA1KraftigFragmenteringKode

        /// <summary>
        /// Number of locations (defined by a threat)
        /// </summary>
        public SpeciesAssessment2021BAii BAii { get; set; } = new();

        /// <summary>
        /// Continuing decline observed, estimated, inferred or projected in any of: (i) extent of occurence; (ii) area of occupancy; (iii) area, extent and/or quality of habitat; (iv) number of locations or subpopulations; (v) number of mature individuals
        /// </summary>
        public List<string> BBOptions { get; set; } = new(); // BBPågåendeArealreduksjonKode

        /// <summary>
        /// Extreme fluctuations in any of: (i) extent of occurence; (ii) area of occupancy; (iii) number of locations or subpopulations; (iv) number of mature individuals
        /// </summary>
        public List<string> BCOptions { get; set; } = new(); // BCEksterneFluktuasjonerKode

        ///// <summary>
        ///// For reasons specified herein, the taxon is Not Applicable (NA) for Red list evaluation in the AssessmentArea.
        ///// </summary>
        //public string RationaleNotApplicable { get; set; } // BegrensetForekomstNA

        /// <summary>
        /// A quantified continuing decline
        /// </summary>
        public SpeciesAssessment2021C1 C1 { get; set; } = new();

        /// <summary>
        /// A continuing decline and few mature individuals in each subpopulation
        /// </summary>
        public SpeciesAssessment2021C2Ai C2Ai { get; set; } = new();

        /// <summary>
        /// A continuing decline and a high proportion of all mature individuals in one subpopulations
        /// </summary>
        public SpeciesAssessment2021C2Aii C2Aii { get; set; } = new();

        /// <summary>
        /// Whether or not there are extreme fluctuations in the number of mature individuals. 
        /// </summary>
        public string C2bExtremeFluctuations { get; set; } // C2BPågåendePopulasjonsreduksjonKode

        /// <summary>
        /// Number of mature individuals
        /// </summary>
        public SpeciesAssessment2021C C { get; set; } = new();

        /// <summary>
        /// Preliminary category that just the number of mature individuals on the D1-criterion represents. 
        /// </summary>
        public string D1PreliminaryCategory { get; set; } // D1FåReproduserendeIndividKode

        /// <summary>
        /// Preliminary category that just the area of occupancy or number of locations on the D2-criterion represents. 
        /// </summary>
        public string D2PreliminaryCategory { get; set; } // D2MegetBegrensetForekomstarealKode

        /// <summary>
        /// Name of ekspert committee that conducted the assessments.
        /// </summary>
        public string ExpertCommittee { get; set; } // Ekspertgruppe

        /// <summary>
        /// Preliminary category based on a quantitative analysis (the E-criterion).
        /// </summary>
        public string EPreliminaryCategory { get; set; } // EKvantitativUtryddingsmodellKode

        /// <summary>
        /// Species' occurrence in counties/regions.
        /// </summary>
        public List<SpeciesAssessment2021RegionOccurrence> RegionOccurrences { get; set; } = new();

        /// <summary>
        /// Average age of reproducing individuals.
        /// </summary>
        public string GenerationLength { get; set; } // Generasjonslengde

        public PreviousAssessment[] PreviousAssessments { get; set; } = Array.Empty<PreviousAssessment>(); 
        
        /// <summary>
        /// Final category according to IUCN categories and criteria.
        /// </summary>
        public string Category { get; set; } // Kategori

        /// <summary>
        /// Preliminary category based on evaluation against the IUCN criteria for the regional population in the specified AssessmentArea. If populations outside the region does not significantly affect the extinction risk within the region (i.e., adjustment of category is not warranted), this becomes the final category.
        /// </summary>
        public string CategoryAdjustedFrom { get; set; } // KategoriEndretFra

        /// <summary>
        /// Final category after adjustment of category due to populations outside the region.
        /// </summary>
        public string CategoryAdjustedTo { get; set; } // KategoriEndretTil

        public string ExpertStatement { get; set; } // Kriteriedokumentasjon

        /// <summary>
        /// Criteria summarized according to IUCN Guidelines and Norwegian national adaptations, written on the standard IUCN format. 
        /// </summary>
        public string CriteriaSummarized { get; set; } // Kriterier

        ///// <summary>
        ///// Rationale for why a taxon is set to the category Not Evaluated NE, i.e., why it is not evaluated for the Red List
        ///// </summary>
        //public string RationaleNotEvaluated { get; set; } // KunnskapsStatusNE

        /// <summary>
        /// Percentage of the European population that the taxon's population in the AssessmenArea constitutes.
        /// </summary>
        public string PercentageEuropeanPopulation { get; set; } // MaxAndelAvEuropeiskBestand

        /// <summary>
        /// Percentage of the global population that the taxon's population in the AssessmentArea constitutes.
        /// </summary>
        public string PercentageGlobalPopulation { get; set; } // MaxAndelAvGlobalBestand

        /// <summary>
        /// The habitat type(s) of importance for the taxon. A MainHabitat type corresponds to one or several EcoSyst (Natur i Norge version 2.0) codes.
        /// </summary>
        public List<string> MainHabitat { get; set; } = new(); // NaturtypeHovedenhet
        
        /// <summary>
        /// Initial assessment of the taxon, either (in)directly to a category or to be thoroughly evaluated against Red List criteria.
        /// </summary>
        public string AssessmentInitialClassification { get; set; } // OverordnetKlassifiseringGruppeKode
        /// <summary>
        /// Sub classification of initial classification
        /// </summary>
        public string AssessmentInitialSubClassification { get; set; } // OverordnetKlassifiseringGruppeKode
        public InitialClassification AssessmentInitialClassificationCode { get; set; } // OverordnetKlassifiseringGruppeKode
        public InitialSubClassification AssessmentInitialSubClassificationCode { get; set; }
        public enum InitialClassification
        {
            [Description("Full rødlistevurdering gjennomført")]
            RodlisteVurdertArt,

            [Description("Det er svært liten tvil om at arten er utdødd fra Norge (RE)")]
            UtdoddINorgeRE,

            [Description("Arten er vurdert til å være livskraftig (LC)")]
            SikkerBestandLC,

            [Description("DD: Usikkerheten om artens korrekte kategoriplassering er meget stor og inkluderer muligheten for at den kan være livskraftig (LC)")]
            StorUsikkerhetOmKorrektKategoriDD,

            [Description("Fremmed art (NA)")]
            FremmedArtNA,

            [Description("Art med begrenset forekomst i Norge (NA)")]
            BegrensetForekomstNA,

            [Description("Arten er ikke forsøkt rødlistevurdert (NE)")]
            KunnskapsStatusNE
        }
        public enum InitialSubClassification
        {
            [Description("Arten er dokumentert eller antatt å være etablert med fast reproduserende bestand i Norge og har ikke opphav i introduserte individer")]
            EtablertBestandINorge,

            [Description("Arten er ikke fast reproduserende i Norge, men > 2 % av individene som utgjør global populasjonsstørrelse oppholder seg regelmessig i Norge i deler av sin års -/ livssyklus(gjester)")]
            IkkeReproduserendeINorge,

            [Description("Arten er fremmed, men er antatt eller dokumentert etablert med fast reproduserende bestand (reprodusert sammenhengende i mer enn 10 år) per år 1800")]
            EtablertFør1800,

            // begrensetForekomstNA
            [Description("Arten er ikke etablert med fast reproduserende bestand hos oss, og < 2 % av global populasjonsstørrelse oppholder seg regelmessig i Norge (gjest) (NA))")]

            IkkeReproduserendeGjestNA,

            [Description("Arten er observert reproduserende i Norge, men antas å ikke være etablert med fast reproduserende bestand hos oss (NA))")]

            ObservertReproduserendeIkkeEtablertNA,

            [Description("Arten er observert i reproduktivt stadium i Norge, men antas å ikke være etablert med fast reproduserende bestand hos oss (NA))")]

            ObservertReproduktivIkkeEtablertNA,

            [Description("Gjest («vagrant») som uregelmessig dukker opp naturlig i Norge (NA))")]

            UregelmessigGjestIkkeReproduserendeNA,

            [Description("Arten har ikke dokumentert forekomst i Norge (NA)")]

            IkkeDokumentertForekomstNA,

            [Description("Hybridart; uten reproduksjon (NA)")]

            HybridartUtenReproduksjonNA,

            [Description("Hybridart, men uvisst om den er reproduserende (NA)")]

            HybridartUvisstOmReproduserendeNA,

            [Description("Arten forekommer bare i hybridkombinasjon i Norge (NA)")]

            ForekommerBareIHybridkombinasjonNA,

            // kunnskapsStatusNE
            [Description("Fremmed art (NA)")]
            FremmedArtNA,

            [Description("Arten er ikke forsøkt vurdert på grunn av svært mangelfull kunnskap (NE)")]
            MangelfullKunnskapNE,
            [Description("Arten eller artsgruppen den tilhører har uavklart systematikk (NE)")]
            UavklartSystematikkNE,
            [Description("Det er uavklart om arten er etablert med fast reproduserende bestand i Norge (NE)")]
            UklartOmReproduserendeBestandNE,
            [Description("Arten tilhører en artsgruppe som ikke er rødlistevurdert etter IUCN sine kriterier til Rødlista 2021 (NE)")]
            ArtsgruppeIkkeRisikovurdertNE,
            [Description("Annet (NE)")]
            AnnetNE


        }


        /// <summary>
        /// Norwegian common names
        /// </summary>
        public string PopularName { get; set; }

        public List<SpeciesAssessment2021ImpactFactor> ImpactFactors { get; set; } = new(); // Påvirkningsfaktorer

        public List<SpeciesAssessment2021Reference> References { get; set; } = new(); // Referanser

        ///// <summary>
        ///// Justification for why the taxon is evaluated, i.e., which one of the three Norwegian inclusion criteria that is met.
        ///// </summary>
        //public string EvaluationJustification { get; set; } // RodlisteVurdertArt men erstattet av AssessmentInitialSubCategory

        /// <summary>
        /// Publication year of the latest previous assessment of the taxon. 
        /// </summary>
        public int YearPreviousAssessment { get; set; } // SistVurdertAr

        public List<SpeciesAssessment2021TaxonHistory> TaxonomicHistory { get; set; } = new();

        /// <summary>
        /// Taxonomic level of the assessed taxon, either "Species" or "SubSpecies". Note that "SubSpecies" encompasses different forms of within-species taxonomic levels (e.g., subspecies and varieties).
        /// </summary>
        public string TaxonRank { get; set; }
        
        /// <summary>
        /// The tag "Possibly Extinct" is used on Critically Endangered CR taxa that assessors suspect to be extinct.
        /// </summary>
        public bool PresumedExtinct { get; set; } // TroligUtdodd

        /// <summary>
        /// Justification for why the taxon is evaluated, i.e., which one of the three Norwegian inclusion criteria that was met before the taxon became regionally extinct.
        /// </summary>
        public string RationaleRegionallyExtinct { get; set; } // UtdoddINorgeRE

        /// <summary>
        /// Whether or not the taxon's extinction risk of within the region is significantly affected by the taxon's populations outside of the region. If "yes", adjustment of category is warranted. 
        /// </summary>
        public string ExtinctionRiskAffected { get; set; } // UtdøingSterktPåvirket

        /// <summary>
        /// Geographic area assessed
        /// </summary>
        public string AssessmentArea { get; set; } // VurderingsContext

        /// <summary>
        /// Year when species was red list assessed. This is the latest valid assessment for the species.
        /// </summary>
        public int AssessmentYear { get; set; } // Vurderingsår

        /// <summary>
        /// The scientific name. When forming part of an Identification, this should be the name in lowest level taxonomic rank that can be determined.
        /// </summary>
        public string ScientificName { get; set; } // VurdertVitenskapeligNavn

        public string ScientificNameHtml { get; set; } = string.Empty;

        /// <summary>
        /// Author of the scienfic name
        /// </summary>
        public string ScientificNameAuthor { get; set; } // VurdertVitenskapeligNavnAutor

        // TODO Må ha nytt navn
        public string VurdertVitenskapeligNavnHierarki { get; set; }

        /// <summary>
        /// An identifier for the nomenclatural (not taxonomic) details of a scientific name
        /// </summary>
        public int ScientificNameId { get; set; } // VurdertVitenskapeligNavnId
        
        /// <summary>
        /// Reason for category transfer compared to previous lists, provided when a taxon has a different category on the current Red List than on its preceding Red List assessment.
        /// </summary>
        public CategoryChangeReason ReasonCategoryChange { get; set; } // ÅrsakTilEndringAvKategori

        /// <summary>
        /// Rationale for adjusting the category based on significant effect from populations outside the region.
        /// </summary>
        public string RationaleCategoryAdjustment { get; set; } // RationaleCategoryAdjustment

        /// <summary>
        /// Group the species belongs to, in most cases a taxonomic group
        /// </summary>
        public string SpeciesGroup { get; set; }

        /// <summary>
        /// Publish date of assessment
        /// </summary>
        public DateTime RevisionDate { get; set; }
        /// <summary>
        /// Revision number
        /// </summary>
        public int Revision { get; set; }
        /// <summary>
        ///  List of revisions (Only for Current revision - point to older)
        /// </summary>
        public List<SpeciesAssessment2021> Revisions { get; set; }
        /// <summary>
        /// A text explaining the need for a new revision - no text on oldest revision 
        /// </summary>
        public string RevisionReason { get; set; }
    }
}