using System;
using System.Runtime.Versioning;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace OneNoteDotNet
{
    /// <summary>
    /// Encapsulates access to the OneNote Application object.
    /// </summary>
    /// <remarks>
    /// <para>Dispose each instance of this class to avoid leaving the OneNote application running
    /// longer than necessary and impacting user experience.</para>
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public sealed class Application : IDisposable
    {
        private Microsoft.Office.Interop.OneNote.Application? _application = new();

        public void Dispose()
        {
            // https://stackoverflow.com/a/3938075/851560
            // https://stackoverflow.com/a/17131389/851560
            SetComObjectReferenceToNull();
            GC.Collect();
        }

        private void SetComObjectReferenceToNull()
        {
            _application = null;
        }

        public Hierarchy Hierarchy
        {
            get
            {
                if (_application == null)
                {
                    throw new ObjectDisposedException(nameof(Application));
                }

                _application.GetHierarchy(null, HierarchyScope.hsPages, out var xmlString, XMLSchema.xs2013);
                var xml = XElement.Parse(xmlString);
                return new Hierarchy(xml);
            }
        }

        public PageContent GetPageContent(string id, PageInfo pageInfo)
        {
            if (_application == null)
            {
                throw new ObjectDisposedException(nameof(Application));
            }

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
