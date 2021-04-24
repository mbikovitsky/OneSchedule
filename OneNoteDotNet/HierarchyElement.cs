using System;
using System.Globalization;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public abstract class HierarchyElement : HierarchyBase
    {
        protected internal HierarchyElement(XElement xml) : base(xml)
        {
        }

        public string Name => Xml.Attribute("name")?.Value;

        public string Id => Xml.Attribute("ID")?.Value;

        public DateTime? LastModifiedTime => ParseDateAttribute("lastModifiedTime");

        public bool IsCurrentlyViewed => ParseBoolAttribute("isCurrentlyViewed").GetValueOrDefault(false);

        protected bool? ParseBoolAttribute(XName name)
        {
            var boolString = Xml.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(boolString))
            {
                return null;
            }

            return bool.Parse(boolString);
        }

        protected int? ParseIntAttribute(XName name)
        {
            var intString = Xml.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(intString))
            {
                return null;
            }

            return int.Parse(intString);
        }

        protected DateTime? ParseDateAttribute(XName name)
        {
            var timeString = Xml.Attribute(name)?.Value;
            if (string.IsNullOrWhiteSpace(timeString))
            {
                return null;
            }

            return DateTime.ParseExact(timeString, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture);
        }
    }
}
