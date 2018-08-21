using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Guid("967EDE4B-BE83-4354-9C26-DE0AF1BAD38E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMDExternalConnection
	{
		void GetConnectionStreamProperties(out string out_pbstrStreamProperties);

		void CreateRequestStream([In] int in_eRequestType, [MarshalAs(UnmanagedType.Interface)] out object out_ppWriteStream);

		void CreateResponseStream([MarshalAs(UnmanagedType.Interface)] out object out_ppReadStream);

		void Close();
	}
}
