using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Guid("33B2B132-69BE-4ade-A90E-939972B93FD5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IXASC
	{
		void ConvertProperties(object in_varProperties, ref uint out_pcPropSets, IntPtr out_ppPropSets);

		void InitializeDataSource([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object ioppIDBInitialize);

		void InitializeSession([MarshalAs(UnmanagedType.Interface)] object in_pIDBInitialize, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object io_ppIDBSession);

		void InitializeCommand([MarshalAs(UnmanagedType.Interface)] object in_pIDBInitialize, [MarshalAs(UnmanagedType.Interface)] object in_pIDBCreateSession, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object io_ppIDBCommand);

		void Execute([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.BStr)] string in_strQuery, int in_eResultType, out ulong out_pcRowsAffected, [MarshalAs(UnmanagedType.IUnknown)] out object out_ppIUnknown);

		void GetSchemaRowset([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, Guid in_rguidSchema, int in_cRestrictions, [MarshalAs(UnmanagedType.LPArray)] object[] in_rgRestrictions, Guid in_rIID, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.IUnknown)] out object out_ppRowset);

		void ResultToXML([MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkData, int in_eAxisFormat, [MarshalAs(UnmanagedType.BStr)] string in_bstrRequestType, int in_iStart, int in_iEnd, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrRows);

		void ResultToXMLStream([MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkStream, [MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkData, int in_eSchemaData, int in_eAxisFormat, [MarshalAs(UnmanagedType.BStr)] string in_bstrRequestType, int in_iStart, int in_iEnd);

		void GetPropertiesRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void GetSchemasRowset([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void GetDatasrcRowset([MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.IUnknown)] object in_pIDatasources, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void GetKeywordsRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void GetLiteralsRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void GetEnumRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		void InitStruct([MarshalAs(UnmanagedType.BStr)] string in_bstrStructName, [MarshalAs(UnmanagedType.Interface)] out object out_ppXDStruct);

		void XMLArrayToSafeArray([MarshalAs(UnmanagedType.BStr)] string in_bstrXML, [MarshalAs(UnmanagedType.SafeArray)] out object out_pvarSafeArray);

		void XMLDOMArrayToSafeArray([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLArrayNode, [MarshalAs(UnmanagedType.SafeArray)] out object out_pvarSafeArray);

		void CreatePropertyManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLNodeProperties, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDPropertyMgr);

		void CreatePropertyManagerEx([MarshalAs(UnmanagedType.Interface)] object in_pIXDPropMgrSrc, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLNodeProperties, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDPropertyMgr);

		void CreateEnumManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDEnumMgr);

		void CreateRestManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDRestMgr);

		[return: MarshalAs(UnmanagedType.IUnknown)]
		object CreatePool();

		object GetPropertyValue([MarshalAs(UnmanagedType.IUnknown)] object in_pObject, Guid in_PropSetGuid, int in_PropID);

		void DeletePropertySets(ref IntPtr in_ppPropSet, int in_cPropSets);

		[return: MarshalAs(UnmanagedType.BStr)]
		string GetDatabaseRoles([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.BStr)] string in_bstrDatabaseName);

		[return: MarshalAs(UnmanagedType.BStr)]
		string GetMdxFromMdXml([MarshalAs(UnmanagedType.BStr)] string in_bstrXML, [MarshalAs(UnmanagedType.BStr)] string in_bstrXSL);

		[return: MarshalAs(UnmanagedType.BStr)]
		string GetConfigLocation();

		[return: MarshalAs(UnmanagedType.IUnknown)]
		object ProcessRequest([MarshalAs(UnmanagedType.IUnknown)] object RequestStream);
	}
}
