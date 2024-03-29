﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace OneNoteDotNet
{
    public class SectionGroup : PathHierarchyElement
    {
        internal SectionGroup(XElement xml) : base(xml)
        {
        }

        public bool? IsRecycleBin => ParseBoolAttribute("isRecycleBin");

        public IEnumerable<Section> Sections => XmlElements(SectionTag).Select(element => new Section(element));

        public IEnumerable<SectionGroup> SectionGroups =>
            XmlElements(SectionGroupTag).Select(element => new SectionGroup(element));
    }
}
