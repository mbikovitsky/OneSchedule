using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class Section : PathHierarchyElement
    {
        internal Section(XElement xml) : base(xml)
        {
        }

        public string? Color => Xml.Attribute("color")?.Value;

        public bool IsInRecycleBin => ParseBoolAttribute("isInRecycleBin").GetValueOrDefault(false);

        public bool IsDeletedPages => ParseBoolAttribute("isDeletedPages").GetValueOrDefault(false);

        public IEnumerable<Page> Pages => XmlElements(PageTag).Select(element => new Page(element));
    }
}
