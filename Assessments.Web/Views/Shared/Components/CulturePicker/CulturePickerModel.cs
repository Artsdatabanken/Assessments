using System.Globalization;

namespace Assessments.Web.Views.Shared.Components.CulturePicker;

public class CulturePickerModel
{
    public CultureInfo CurrentUICulture { get; set; }

    public List<CultureInfo> SupportedCultures { get; set; }
}