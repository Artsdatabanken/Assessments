using Assessments.Shared.Helpers;

namespace Assessments.Tests.Extensions;

public class StripHtmlExtensionsTests
{
    [Fact]
    public void StripHtmlExtensionShouldRemoveHtmlTags()
    {
        var stripHtml = """<p style="font-size: 28px">Lorem ipsum</p>""".StripHtml();

        Assert.Equal("Lorem ipsum", stripHtml);
    }

    [Fact]
    public void StripUnwantedHtmlTagsExtensionShouldRemoveUnwantedHtmlTags()
    {
        var stripUnwantedHtml = """<p style="font-size: 28px">Lorem ipsum</p><div>Lorem ipsum</div>""".StripUnwantedHtml();

        Assert.Equal("<p>Lorem ipsum</p>Lorem ipsum", stripUnwantedHtml);
    }

    [Fact]
    public void StripUnwantedHtmlTagsExtensionShouldRemoveUnwantedHtmlTagsButKeepLinks()
    {
        var stripUnwantedHtml = """<a style="font-size: 28px" href="https://artsdatabanken.no">artsdatabanken</a>""".StripUnwantedHtml();

        Assert.Equal(("""<a href="https://artsdatabanken.no">artsdatabanken</a>"""), stripUnwantedHtml);
    }

    [Fact]
    public void StripHtmlExtensionShouldReplaceNonBreakingSpace()
    {
        var stripHtml = """<p style="font-size: 28px">Lorem&nbsp;ipsum</p>""".StripHtml();

        Assert.Equal("Lorem ipsum", stripHtml);
    }

    [Fact]
    public void StripUnwantedHtmlExtensionShouldReplaceNonBreakingSpace()
    {
        var stripUnwantedHtml = """<p style="font-size: 28px">Lorem&nbsp;ipsum</p>""".StripUnwantedHtml();

        Assert.Equal("<p>Lorem ipsum</p>", stripUnwantedHtml);
    }
}