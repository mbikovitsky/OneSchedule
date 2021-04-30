using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Xml.Linq;
using Microsoft.Office.Interop.OneNote;

namespace OneNoteDotNet
{
    /// <summary>
    /// Encapsulates access to the OneNote Application object.
    /// </summary>
    /// <remarks>
    /// <para>This class is <b>not</b> thread-safe. Violating this constraint may lead to
    /// <b>memory corruption</b>.</para>
    /// <para>Dispose each instance of this class to avoid leaving the OneNote application running
    /// more than necessary and impacting user experience.</para>
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public sealed class OneNote : IDisposable
    {
        private Application? _application;

        public OneNote()
        {
            _application = new Application();
        }

        public void Dispose()
        {
            // ReSharper disable once InvertIf
            if (_application != null)
            {
                // ATTENTION
                // First, see here: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal.releasecomobject?view=net-5.0#remarks
                // We're assuming that the Application object is not a singleton, so this call
                // will not affect any other instances of this class. However, Disposing
                // an instance while another thread tries to use the object will definitely
                // corrupt memory.
                Marshal.FinalReleaseComObject(_application);
                _application = null;
            }
        }

        public Hierarchy Hierarchy
        {
            get
            {
                if (_application == null)
                {
                    throw new ObjectDisposedException(nameof(OneNote));
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
                throw new ObjectDisposedException(nameof(OneNote));
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
