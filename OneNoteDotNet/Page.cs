using System;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class Page : HierarchyElement
    {
        internal Page(XElement xml) : base(xml)
        {
        }

        public DateTimeOffset? DateTime => ParseDateAttribute("dateTime");

        public int? PageLevel => ParseIntAttribute("pageLevel");

        public bool IsInRecycleBin => ParseBoolAttribute("isInRecycleBin").GetValueOrDefault(false);
    }
}
