using System;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class Page : HierarchyElement
    {
        public Page(XElement xml) : base(xml)
        {
        }

        public DateTime? DateTime => ParseDateAttribute("dateTime");

        public int? PageLevel => ParseIntAttribute("pageLevel");

        public bool IsInRecycleBin => ParseBoolAttribute("isInRecycleBin").GetValueOrDefault(false);
    }
}
