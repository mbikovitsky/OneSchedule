using System.Collections.Generic;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public abstract class HierarchyBase
    {
        public const string NotebookTag = "Notebook";
        public const string SectionTag = "Section";
        public const string SectionGroupTag = "SectionGroup";
        public const string PageTag = "Page";

        protected internal HierarchyBase(XElement xml)
        {
            Xml = xml;
        }

        public XElement Xml { get; }

        public IEnumerable<XElement> XmlElements(string tagName)
        {
            return Xml.Elements(CreateTag(tagName));
        }

        public IEnumerable<XElement> XmlDescendants(string tagName)
        {
            return Xml.Descendants(CreateTag(tagName));
        }

        public XName CreateTag(string tagName)
        {
            return Xml.Name.Namespace + tagName;
        }
    }
}
