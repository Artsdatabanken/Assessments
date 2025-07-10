using System.ComponentModel;
using System.Data;
using Assessments.Mapping.AlienSpecies.Model;
using Assessments.Mapping.NatureTypes.Model;
using Assessments.Mapping.RedlistSpecies;
using Assessments.Shared.Constants;
using Assessments.Web.Infrastructure.Services;
using ClosedXML.Excel;
using ClosedXML.Graphics;

namespace Assessments.Web.Infrastructure;

public static class ExportHelper
{
    public static void Setup()
    {
        // add linux fallback font
        LoadOptions.DefaultGraphicEngine = new DefaultGraphicEngine("carlito");
    }

    public static MemoryStream GenerateSpeciesAssessment2021Export(IEnumerable<SpeciesAssessment2021Export> assessments, IEnumerable<ExpertCommitteeMember> expertCommitteeMembers, string displayUrl)
    {
        MemoryStream memoryStream;
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Vurderinger");
            worksheet.Cell(1, 1).InsertTable(assessments);

            var exportColumns = typeof(SpeciesAssessment2021Export).GetProperties().Select(p => new
            {
                p.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Single().DisplayName,
                p.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Single().Description
            }).ToList();

            var firstRow = worksheet.FirstRow();
            var columnNumber = 1;

            foreach (var column in exportColumns)
            {
                firstRow.Cell(columnNumber).Value = column.DisplayName;
                columnNumber++;
            }

            worksheet.Columns().Width = 28;

            var table = new DataTable("Feltnavn og beskrivelser");
            table.Columns.Add("Feltnavn");
            table.Columns.Add("Beskrivelse");

            foreach (var element in exportColumns)
                table.Rows.Add(element.DisplayName, element.Description);

            workbook.Worksheets.Add(table);
            workbook.Worksheet(2).Columns().AdjustToContents();

            var expertCommitteeWorksheet = workbook.AddWorksheet("Ekspertkomitéer");
            expertCommitteeWorksheet.Cell(1, 1).InsertTable(expertCommitteeMembers
                .OrderBy(x => x.ExpertCommittee).ThenBy(x => x.LastName).Select(x => new
                {
                    Ekspertkomité = x.ExpertCommittee,
                    Navn = x.Name,
                    Institusjon = x.Institution
                }).ToList());

            workbook.Worksheet(3).Columns().AdjustToContents();

            var citeAsWorksheet = workbook.AddWorksheet("Siteres som");
            var referringUrl = new Uri(displayUrl);
            citeAsWorksheet.Cell(1, 1).Value = $"Artsdatabanken (2021, 24. november). Norsk rødliste for arter 2021. Utvalg {CleanQueryString(referringUrl)}. {referringUrl.GetLeftPart(UriPartial.Path)}";

            foreach (var workbookWorksheet in workbook.Worksheets)
                workbookWorksheet.SheetView.FreezeRows(1);

            memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }

    public static MemoryStream GenerateAlienSpeciesAssessment2023Export(IEnumerable<AlienSpeciesAssessment2023Export> assessments, string displayUrl)
    {
        MemoryStream memoryStream;
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Vurderinger");
            worksheet.Cell(1, 1).InsertTable(assessments);

            var exportColumns = typeof(AlienSpeciesAssessment2023Export).GetProperties().Select(p => new
            {
                p.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Single().DisplayName,
                p.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Single().Description
            }).ToList();

            var firstRow = worksheet.FirstRow();
            var columnNumber = 1;

            foreach (var column in exportColumns)
            {
                firstRow.Cell(columnNumber).Value = column.DisplayName;
                columnNumber++;
            }

            worksheet.Columns().Width = 28;

            var table = new DataTable("Feltnavn og beskrivelser");
            table.Columns.Add("Feltnavn");
            table.Columns.Add("Beskrivelse");

            foreach (var element in exportColumns)
                table.Rows.Add(element.DisplayName, element.Description);

            workbook.Worksheets.Add(table);
            workbook.Worksheet(2).Columns().AdjustToContents();

            var citeAsWorksheet = workbook.AddWorksheet("Siteres som");
            var referringUrl = new Uri(displayUrl);
            citeAsWorksheet.Cell(1, 1).Value = $"Artsdatabanken (2023). Fremmedartslista 2023. Utvalg {CleanQueryString(referringUrl)}. {referringUrl.GetLeftPart(UriPartial.Path)}";

            foreach (var workbookWorksheet in workbook.Worksheets)
                workbookWorksheet.SheetView.FreezeRows(1);

            memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }

    public static MemoryStream GenerateNatureTypeAssessment2025Export(IEnumerable<NatureTypeAssessmentExport> assessments, string displayUrl)
    {
        MemoryStream memoryStream;
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.AddWorksheet("Vurderinger");

            worksheet.FirstCell().InsertTable(assessments);
            worksheet.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            var exportColumns = typeof(NatureTypeAssessmentExport).GetProperties().Select(p => new
            {
                p.GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>().Single().DisplayName,
                p.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().Single().Description
            }).ToList();

            var firstRow = worksheet.FirstRow();
            var columnNumber = 1;

            foreach (var column in exportColumns)
            {
                firstRow.Cell(columnNumber).Value = column.DisplayName;
                columnNumber++;
            }

            var table = new DataTable("Feltnavn og beskrivelser");
            table.Columns.Add("Feltnavn");
            table.Columns.Add("Beskrivelse");

            foreach (var element in exportColumns)
                table.Rows.Add(element.DisplayName, element.Description);

            workbook.Worksheets.Add(table);

            var citeAsWorksheet = workbook.AddWorksheet("Siteres som");
            var referringUrl = new Uri(displayUrl);

            var citation = $"{NatureTypesConstants.Citation} Utvalg {CleanQueryString(referringUrl)}. {referringUrl.GetLeftPart(UriPartial.Path)}";
            
            citeAsWorksheet.FirstCell().Value = citation;

            foreach (var workbookWorksheet in workbook.Worksheets)
            {
                workbookWorksheet.SheetView.FreezeRows(1);
                workbookWorksheet.Columns().AdjustToContents();
            }

            memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }

    private static string CleanQueryString(Uri uri) => uri.RemoveQueryStringKeys(["SortBy", "Meta", "IsCheck"]);
}