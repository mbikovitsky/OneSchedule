using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace OneNoteDotNet
{
    public class PageContent
    {
        internal PageContent(XElement xml)
        {
            Xml = xml;
        }

        public XElement Xml { get; }

        public IEnumerable<string> TextElements =>
            Xml.Descendants(Xml.Name.Namespace + "T").Select(element => element.Value);

        public IEnumerable<string> PlainTextElements => TextElements.Select(text =>
        {
            var html = new HtmlDocument();
            html.LoadHtml(text);
            var encodedText = html.DocumentNode.InnerText.Trim();
            var decodedText = HttpUtility.HtmlDecode(encodedText);
            return decodedText;
        });
    }
}
