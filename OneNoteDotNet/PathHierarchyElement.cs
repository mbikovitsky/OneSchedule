using System.Xml.Linq;

namespace OneNoteDotNet
{
    public abstract class PathHierarchyElement : HierarchyElement
    {
        protected internal PathHierarchyElement(XElement xml) : base(xml)
        {
        }

        public string? Path => Xml.Attribute("path")?.Value;
    }
}
