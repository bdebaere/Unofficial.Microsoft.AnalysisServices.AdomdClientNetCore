using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Guid("6F7BA4C2-72EC-4588-988B-D1CF8E6A267B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMDExternalConnStream
	{
		void Read([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [Out] byte[] pv, [In] int cb, [Out] IntPtr pcbRead);

		void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] [In] byte[] pv, [In] int cb, [Out] IntPtr pcbWritten);

		void Close();
	}
}
