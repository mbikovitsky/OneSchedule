using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace OneNoteDotNet
{
    public class OneNote
    {
        private readonly Application _application;

        public OneNote()
        {
            _application = new Application();
        }

        public Hierarchy Hierarchy
        {
            get
            {
                _application.GetHierarchy(null, HierarchyScope.hsPages, out var xmlString, XMLSchema.xs2013);
                var xml = XElement.Parse(xmlString);
                return new Hierarchy(xml);
            }
        }
    }
}
