using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Guid("91878576-7BF9-4395-A966-60292E3F233E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IMDExternalDataChannel
	{
		void Initialize([In] int in_dwTimeout, [MarshalAs(UnmanagedType.BStr)] [In] string in_bstrConfigurationProperties);

		void OpenConnection([MarshalAs(UnmanagedType.BStr)] [In] string in_bstrConnectionString, [MarshalAs(UnmanagedType.BStr)] [In] string in_bstrAuthInfo, [MarshalAs(UnmanagedType.Interface)] out object out_ppConnection);
	}
}
