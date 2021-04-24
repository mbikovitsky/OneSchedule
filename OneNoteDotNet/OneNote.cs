using System;
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

        public PageContent GetPageContent(string id, PageInfo pageInfo)
        {
            var mappedPagedInfo = MapPageInfo(pageInfo);
            _application.GetPageContent(id, out var xmlString, mappedPagedInfo, XMLSchema.xs2013);
            var xml = XElement.Parse(xmlString);
            return new PageContent(xml);
        }

        private static Microsoft.Office.Interop.OneNote.PageInfo MapPageInfo(PageInfo pageInfo)
        {
            switch (pageInfo)
            {
                case PageInfo.Basic:
                case PageInfo.BinaryData:
                case PageInfo.Selection:
                case PageInfo.BinaryDataSelection:
                case PageInfo.FileType:
                case PageInfo.BinaryDataFileType:
                case PageInfo.SelectionFileType:
                case PageInfo.All:
                    return (Microsoft.Office.Interop.OneNote.PageInfo) pageInfo;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pageInfo), pageInfo, null);
            }
        }
    }
}
