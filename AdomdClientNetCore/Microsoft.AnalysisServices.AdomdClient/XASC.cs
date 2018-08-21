using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[ClassInterface(ClassInterfaceType.None), Guid("B9776FC2-70D8-4664-A0DF-998114524D67")]
	[ComImport]
	internal class XASC : IXASC
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.ConvertProperties(object in_varProperties, ref uint out_pcPropSets, IntPtr out_ppPropSets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.InitializeDataSource([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object ioppIDBInitialize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.InitializeSession([MarshalAs(UnmanagedType.Interface)] object in_pIDBInitialize, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object io_ppIDBSession);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.InitializeCommand([MarshalAs(UnmanagedType.Interface)] object in_pIDBInitialize, [MarshalAs(UnmanagedType.Interface)] object in_pIDBCreateSession, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] out object io_ppIDBCommand);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.Execute([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.BStr)] string in_strQuery, int in_eResultType, out ulong out_pcRowsAffected, [MarshalAs(UnmanagedType.IUnknown)] out object out_ppIUnknown);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetSchemaRowset([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, Guid in_rguidSchema, int in_cRestrictions, [MarshalAs(UnmanagedType.LPArray)] object[] in_rgRestrictions, Guid in_rIID, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.IUnknown)] out object out_ppRowset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.ResultToXML([MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkData, int in_eAxisFormat, [MarshalAs(UnmanagedType.BStr)] string in_bstrRequestType, int in_iStart, int in_iEnd, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrRows);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.ResultToXMLStream([MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkStream, [MarshalAs(UnmanagedType.IUnknown)] object in_pIUnkData, int in_eSchemaData, int in_eAxisFormat, [MarshalAs(UnmanagedType.BStr)] string in_bstrRequestType, int in_iStart, int in_iEnd);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetPropertiesRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetSchemasRowset([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetDatasrcRowset([MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.IUnknown)] object in_pIDatasources, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetKeywordsRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetLiteralsRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIEnumMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.GetEnumRowset([MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.Interface)] object in_pIRestMgr, ref object in_pvarRestrictions, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrSchema, [MarshalAs(UnmanagedType.BStr)] out string out_pbstrData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.InitStruct([MarshalAs(UnmanagedType.BStr)] string in_bstrStructName, [MarshalAs(UnmanagedType.Interface)] out object out_ppXDStruct);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.XMLArrayToSafeArray([MarshalAs(UnmanagedType.BStr)] string in_bstrXML, [MarshalAs(UnmanagedType.SafeArray)] out object out_pvarSafeArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.XMLDOMArrayToSafeArray([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLArrayNode, [MarshalAs(UnmanagedType.SafeArray)] out object out_pvarSafeArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.CreatePropertyManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLNodeProperties, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDPropertyMgr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.CreatePropertyManagerEx([MarshalAs(UnmanagedType.Interface)] object in_pIXDPropMgrSrc, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLNodeProperties, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDPropertyMgr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.CreateEnumManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDEnumMgr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.CreateRestManager([MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig1, [MarshalAs(UnmanagedType.IUnknown)] object in_pIXMLDocConfig2, [MarshalAs(UnmanagedType.Interface)] out object out_ppIXDRestMgr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		extern object IXASC.CreatePool();

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern object IXASC.GetPropertyValue([MarshalAs(UnmanagedType.IUnknown)] object in_pObject, Guid in_PropSetGuid, int in_PropID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		extern void IXASC.DeletePropertySets(ref IntPtr in_ppPropSet, int in_cPropSets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.BStr)]
		extern string IXASC.GetDatabaseRoles([MarshalAs(UnmanagedType.Interface)] object in_pIDBCommand, [MarshalAs(UnmanagedType.Interface)] object in_pIPropMgr, [MarshalAs(UnmanagedType.BStr)] string in_bstrDatabaseName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.BStr)]
		extern string IXASC.GetMdxFromMdXml([MarshalAs(UnmanagedType.BStr)] string in_bstrXML, [MarshalAs(UnmanagedType.BStr)] string in_bstrXSL);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.BStr)]
		extern string IXASC.GetConfigLocation();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.IUnknown)]
		extern object IXASC.ProcessRequest([MarshalAs(UnmanagedType.IUnknown)] object RequestStream);

		//[MethodImpl(MethodImplOptions.InternalCall)]
		//public extern XASC();
	}
}
