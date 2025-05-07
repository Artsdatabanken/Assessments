using System.Linq;
using Assessments.Mapping.RedlistSpecies.Helpers;
using Assessments.Mapping.RedlistSpecies.Source;
using AutoMapper;

namespace Assessments.Mapping.RedlistSpecies.Profiles
{
    public class SpeciesAssessment2021Profile : Profile
    {
        public SpeciesAssessment2021Profile()
        {
            CreateMap<Rodliste2019.Pavirkningsfaktor, SpeciesAssessment2021ImpactFactor>()
                .ForMember(dest => dest.Severity, opt => opt.MapFrom(src => src.Alvorlighetsgrad))
                .ForMember(dest => dest.PopulationScope, opt => opt.MapFrom(src => src.Omfang))
                .ForMember(dest => dest.TimeScope, opt => opt.MapFrom(src => src.Tidspunkt))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .AfterMap(SpeciesAssessment2021ProfileHelper.CorrectImpactFactors);

            CreateMap<Rodliste2019.MinMaxProbable, SpeciesAssessment2021MinMaxProbableIntervall>(); // use the one below - differentiate on punktestimat
            CreateMap<Rodliste2019.MinMaxProbableIntervall, SpeciesAssessment2021MinMaxProbableIntervall>()
                .ForMember(dest => dest.Punktestimat, opt => opt.MapFrom(src => (src.Punktestimat == "true")));
            CreateMap<Rodliste2019.Reference, SpeciesAssessment2021Reference>();

            CreateMap<Rodliste2019.Fylkesforekomst, SpeciesAssessment2021RegionOccurrence>()
                .ForMember(dest => dest.Fylke, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.ResolveRegion(src.Fylke)));

            CreateMap<Rodliste2019.TaxonHistory, SpeciesAssessment2021TaxonHistory>()
                .ForMember(dest => dest.ExpertCommittee, opt => opt.MapFrom(src => src.Ekspertgruppe))
                .ForMember(dest => dest.ScientificName, opt => opt.MapFrom(src => src.VitenskapeligNavn))
                .ForMember(dest => dest.AuthorCitation, opt => opt.MapFrom(src => src.VitenskapeligNavnAutor))
                .ForMember(dest => dest.TaxonomicHierarchy, opt => opt.MapFrom(src => src.VitenskapeligNavnHierarki))
                .ForMember(dest => dest.ScientificNameId, opt => opt.MapFrom(src => src.VitenskapeligNavnId));

            CreateMap<Rodliste2019, SpeciesAssessment2021>()
                
                .ForMember(dest => dest.RegionOccurrences, opt => opt.MapFrom(src => src.Fylkesforekomster))

                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))

                .ForPath(dest => dest.A1.ReductionBasedOn, opt => opt.MapFrom(src => src.A1EndringBasertpåKode))
                .ForPath(dest => dest.A1.PreliminaryCategory, opt => opt.MapFrom(src => src.A1OpphørtOgReversibel))
                .ForPath(dest => dest.A1.QuantifiedReduction, opt => opt.MapFrom(src => src.A1OpphørtOgReversibelAntatt))

                .ForPath(dest => dest.A2.ReductionBasedOn, opt => opt.MapFrom(src => src.A2EndringBasertpåKode))
                .ForPath(dest => dest.A2.PreliminaryCategory, opt => opt.MapFrom(src => src.A2Forutgående10År))
                .ForPath(dest => dest.A2.QuantifiedReduction, opt => opt.MapFrom(src => src.A2Forutgående10ÅrAntatt))

                .ForPath(dest => dest.A3.ReductionBasedOn, opt => opt.MapFrom(src => src.A3EndringBasertpåKode))
                .ForPath(dest => dest.A3.PreliminaryCategory, opt => opt.MapFrom(src => src.A3Kommende10År))
                .ForPath(dest => dest.A3.QuantifiedReduction, opt => opt.MapFrom(src => src.A3Kommende10ÅrAntatt))

                .ForPath(dest => dest.A4.ReductionBasedOn, opt => opt.MapFrom(src => src.A4EndringBasertpåKode))
                .ForPath(dest => dest.A4.PreliminaryCategory, opt => opt.MapFrom(src => src.A4Intervall10År))
                .ForPath(dest => dest.A4.QuantifiedReduction, opt => opt.MapFrom(src => src.A4Intervall10ÅrAntatt))

                .ForMember(dest => dest.ProportionOfMaxPopulation, opt => opt.MapFrom(src => src.AndelNåværendeBestand))

                .ForPath(dest => dest.B1.Statistics, opt => opt.MapFrom(src => src.B1IntervallUtbredelsesområde))
                .ForPath(dest => dest.B1.PreliminaryCategory, opt => opt.MapFrom(src => src.B1UtbredelsesområdeKode))

                .ForPath(dest => dest.B2.PreliminaryCategory, opt => opt.MapFrom(src => src.B2ForekomstarealKode))
                .ForPath(dest => dest.B2.Statistics, opt => opt.MapFrom(src => src.B2IntervallForekomstareal))

                .ForMember(dest => dest.BaiSevereFragmentation, opt => opt.MapFrom(src => src.BA1KraftigFragmenteringKode))

                .ForPath(dest => dest.BAii.PreliminaryCategory, opt => opt.MapFrom(src => src.BA2FåLokaliteterKode))
                .ForPath(dest => dest.BAii.Statistics, opt => opt.MapFrom(src => src.BaIntervallAntallLokaliteter))

                .ForMember(dest => dest.BBOptions, opt => opt.MapFrom(src => src.BBPågåendeArealreduksjonKode.OrderBy(SpeciesAssessment2021ProfileHelper.RomanNumberSort)))
                .ForMember(dest => dest.BCOptions, opt => opt.MapFrom(src => src.BCEksterneFluktuasjonerKode.OrderBy(SpeciesAssessment2021ProfileHelper.RomanNumberSort)))
                //.ForMember(dest => dest.RationaleNotApplicable, opt => opt.MapFrom(src => src.BegrensetForekomstNA))

                .ForPath(dest => dest.C1.Statistics, opt => opt.MapFrom(src => src.C1PågåendePopulasjonsreduksjonAntatt))
                .ForPath(dest => dest.C1.ThresholdValue, opt => opt.MapFrom(src => src.C1PågåendePopulasjonsreduksjonKode))

                .ForPath(dest => dest.C2Ai.Statistics, opt => opt.MapFrom(src => src.C2A1PågåendePopulasjonsreduksjonAntatt))
                .ForPath(dest => dest.C2Ai.ThresholdValue, opt => opt.MapFrom(src => src.C2A1PågåendePopulasjonsreduksjonKode))

                .ForPath(dest => dest.C2Aii.ThresholdValue, opt => opt.MapFrom(src => src.C2A2PågåendePopulasjonsreduksjonKode))
                .ForPath(dest => dest.C2Aii.PercentageOneSubpop, opt => opt.MapFrom(src => src.C2A2ReproduksjonsdyktigeIndivid))
                .ForPath(dest => dest.C2Aii.TruthValue, opt => opt.MapFrom(src => src.C2A2SannhetsverdiKode))

                .ForMember(dest => dest.C2bExtremeFluctuations, opt => opt.MapFrom(src => src.C2BPågåendePopulasjonsreduksjonKode))

                .ForPath(dest => dest.C.Statistics, opt => opt.MapFrom(src => src.CPopulasjonsstørrelseAntatt))
                .ForPath(dest => dest.C.PreliminaryCategory, opt => opt.MapFrom(src => src.CPopulasjonsstørrelseKode))
                .ForPath(dest => dest.C.SubstrateUnitsPerLocation, opt => opt.MapFrom(src => src.CSubstratenheter))

                .ForMember(dest => dest.D1PreliminaryCategory, opt => opt.MapFrom(src => src.D1FåReproduserendeIndividKode))
                .ForMember(dest => dest.D2PreliminaryCategory, opt => opt.MapFrom(src => src.D2MegetBegrensetForekomstarealKode))
                
                .ForMember(dest => dest.ExpertCommittee, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.RemoveAssessmentArea(src.Ekspertgruppe)))
                .ForMember(dest => dest.SpeciesGroup, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.ResolveSpeciesGroupName(src)))
                
                .ForMember(dest => dest.EPreliminaryCategory, opt => opt.MapFrom(src => src.EKvantitativUtryddingsmodellKode))
                
                .ForMember(dest => dest.GenerationLength, opt => opt.MapFrom(src => src.Generasjonslengde))
                
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Kategori))
                .ForMember(destination => destination.Category, opt => opt.NullSubstitute(string.Empty))

                .ForMember(dest => dest.CategoryAdjustedFrom, opt => opt.MapFrom(src => src.KategoriEndretFra))
                .ForMember(dest => dest.CategoryAdjustedTo, opt => opt.MapFrom(src => src.KategoriEndretTil))

                .ForMember(dest => dest.ExpertStatement, opt => opt.MapFrom(src => HtmlCleaner.MakeHtmlSafe(src.Kriteriedokumentasjon, true)))

                .ForMember(dest => dest.CriteriaSummarized, opt => opt.MapFrom(src => src.Kriterier))

                //.ForMember(dest => dest.RationaleNotEvaluated, opt => opt.MapFrom(src => src.KunnskapsStatusNE))
                .ForMember(dest => dest.PercentageEuropeanPopulation, opt => opt.MapFrom(src => src.MaxAndelAvEuropeiskBestand))
                .ForMember(dest => dest.PercentageGlobalPopulation, opt => opt.MapFrom(src => src.MaxAndelAvGlobalBestand))
                .ForMember(dest => dest.MainHabitat, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.ResolveMainHabitat(src.NaturtypeHovedenhet)))

                .ForMember(dest => dest.PopularName, opt => opt.MapFrom(src => src.PopularName))
                .ForMember(destination => destination.PopularName, opt => opt.NullSubstitute(string.Empty))
                
                .ForMember(dest => dest.ImpactFactors, opt => opt.MapFrom(src => src.Påvirkningsfaktorer.OrderBy(x=>x.Id)))

                .ForMember(dest => dest.References, opt => opt.MapFrom(src => src.Referanser.Where(r => r.Type != "Person"))) // ikke ta med personreferanser

                //.ForMember(dest => dest.EvaluationJustification, opt => opt.MapFrom(src => src.RodlisteVurdertArt))
                .ForMember(dest => dest.YearPreviousAssessment, opt => opt.MapFrom(src => src.SistVurdertAr))

                .ForMember(dest => dest.TaxonomicHistory, opt => opt.MapFrom(src => src.TaxonomicHistory))

                .ForMember(dest => dest.TaxonRank, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.Capitalize(src.TaxonRank)))

                .ForMember(dest => dest.PresumedExtinct, opt => opt.MapFrom(src => src.TroligUtdodd))
                .ForMember(dest => dest.RationaleRegionallyExtinct, opt => opt.MapFrom(src => src.UtdoddINorgeRE))

                .ForMember(dest => dest.ExtinctionRiskAffected, opt => opt.MapFrom(src => src.UtdøingSterktPåvirket))

                .ForMember(dest => dest.AssessmentArea, opt => opt.MapFrom(src => src.VurderingsContext))
                .ForMember(dest => dest.AssessmentYear, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.GetAssessmentYear(src.Ekspertgruppe)))

                .ForMember(dest => dest.ScientificName, opt => opt.MapFrom(src => src.VurdertVitenskapeligNavn))
                .ForMember(dest => dest.ScientificNameAuthor, opt => opt.MapFrom(src => src.VurdertVitenskapeligNavnAutor))

                .ForMember(dest => dest.VurdertVitenskapeligNavnHierarki, opt => opt.MapFrom(src => src.VurdertVitenskapeligNavnHierarki))

                .ForMember(dest => dest.ScientificNameId, opt => opt.MapFrom(src => src.VurdertVitenskapeligNavnId))

                .ForMember(dest => dest.ReasonCategoryChange, opt => opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.EvaluateCategoryChangeReason(src)))
                .ForMember(dest => dest.RationaleCategoryAdjustment, opt => opt.MapFrom(src => HtmlCleaner.MakeHtmlSafe(src.ÅrsakTilNedgraderingAvKategori,true)))
                .ForMember(dest=> dest.PreviousAssessments, opt=> opt.MapFrom(src => SpeciesAssessment2021ProfileHelper.GetPreviousAssessments(src)))
                .ForMember(dest => dest.RevisionReason, opt=> opt.MapFrom(src => HtmlCleaner.MakeHtmlSafe(src.Endringslogg, true)))
                .ForMember(dest => dest.Revision, opt => opt.MapFrom(src => src.Revision))
                .ForMember(dest => dest.RevisionDate, opt => opt.MapFrom(src => src.RevisionDate))
                .ForMember(dest => dest.Revisions, opt => opt.MapFrom(src => src.Revisions))
                .AfterMap((src, dest) =>
                {
                    SpeciesAssessment2021ProfileHelper.BlankCriteriaSumarizedBasedOnCategory(dest);
                    SpeciesAssessment2021ProfileHelper.BlankReasonCategoryChangeWhenNoChange(src, dest);
                    SpeciesAssessment2021ProfileHelper.CalculateQuantiles(dest);
                    SpeciesAssessment2021ProfileHelper.FixMissingCategoryChangedFrom(src, dest);
                    SpeciesAssessment2021ProfileHelper.FixMissingRegions(src, dest);
                    SpeciesAssessment2021ProfileHelper.FixSomeTaxonomy(src, dest);
                    InitialClassification.Map(src, dest);
                    dest.ScientificNameHtml = SpeciesAssessment2021ProfileHelper.FormatScientificNameHtml(dest.ScientificName);
                });
        }
    }
}