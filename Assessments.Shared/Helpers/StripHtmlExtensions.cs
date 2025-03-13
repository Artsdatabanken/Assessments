using HtmlAgilityPack;
using System.Text;

namespace Assessments.Shared.Helpers
{
    public static class StripHtmlExtensions
    {
        public static string StripHtml(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var html = GetHtml(input);

            return html.DocumentNode.InnerText.Replace("&nbsp;", " ", StringComparison.OrdinalIgnoreCase);
        }

        public static string StripUnwantedHtml(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var nodeWhiteList = new[] { "p", "i", "b", "a" };
            var attributeWhiteList = new[] { "href" };

            var html = GetHtml(input);
            var allNodes = html.DocumentNode;

            CleanNodes(allNodes, nodeWhiteList, attributeWhiteList);

            var innerHtml = allNodes.InnerHtml;
            innerHtml = HtmlEntity.DeEntitize(innerHtml);

            return innerHtml.Replace("&nbsp;", " ", StringComparison.OrdinalIgnoreCase);
        }

        private static HtmlNode CleanNodes(HtmlNode node, IEnumerable<string> nodeWhitelist, string[] attributeWhitelist)
        {
            if (SkipNode(node))
            {
                var nextNode = node.NextSibling;
                node.ParentNode.RemoveChild(node);

                return nextNode;
            }

            var whitelist = nodeWhitelist.ToList();

            if (node.HasChildNodes)
            {
                var childNode = node.FirstChild;
                while (childNode != null)
                {
                    childNode = CleanNodes(childNode, whitelist, attributeWhitelist);
                }
            }

            if (node.NodeType != HtmlNodeType.Element)
                return node.NextSibling;

            var attribs = node.Attributes.ToList();
            
            foreach (var attrib in attribs.Where(attrib => !attributeWhitelist.Contains(attrib.Name)))
                node.Attributes.Remove(attrib);

            if (whitelist.Contains(node.Name))
                return node.NextSibling;

            var nodeList = node.ChildNodes.ToList();
            
            foreach (var child in nodeList)
                node.ParentNode.InsertBefore(child, node);

            var nextSibling = node.NextSibling;

            node.ParentNode.RemoveChild(node);

            return nextSibling;
        }

        private static bool SkipNode(HtmlNode node)
        {
            if (node.NodeType == HtmlNodeType.Comment)
                return true;

            if (node.Name is "script" or "style")
                return true;

            return node.NodeType == HtmlNodeType.Text && string.IsNullOrWhiteSpace(node.InnerText);
        }

        private static HtmlDocument GetHtml(string source)
        {
            var html = new HtmlDocument
            {
                OptionFixNestedTags = true,
                OptionAutoCloseOnEnd = true,
                OptionDefaultStreamEncoding = Encoding.UTF8,
                OptionWriteEmptyNodes = true,
                OptionOutputAsXml = true
            };

            html.LoadHtml(source);

            return html;
        }
    }
}