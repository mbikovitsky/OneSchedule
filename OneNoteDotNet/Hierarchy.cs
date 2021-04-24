using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class Hierarchy : HierarchyBase
    {
        internal Hierarchy(XElement xml) : base(xml)
        {
        }

        public IEnumerable<Notebook> Notebooks => XmlElements(NotebookTag).Select(element => new Notebook(element));

        public IEnumerable<Page> AllPages => XmlDescendants(PageTag).Select(element => new Page(element));
    }
}
