using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class Notebook : PathHierarchyElement
    {
        internal Notebook(XElement xml) : base(xml)
        {
        }

        public string Color => Xml.Attribute("color")?.Value;

        public string Nickname => Xml.Attribute("nickname")?.Value;

        public IEnumerable<Section> Sections => XmlElements(SectionTag).Select(element => new Section(element));

        public IEnumerable<SectionGroup> SectionGroups =>
            XmlElements(SectionGroupTag).Select(element => new SectionGroup(element));
    }
}
