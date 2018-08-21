using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces
{
	internal interface IASSPClientProxy
	{
		[return: MarshalAs(UnmanagedType.VariantBool)]
		bool IsWorkbookInFarm([MarshalAs(UnmanagedType.BStr)] string in_bstrWorkbookPath);

		[return: MarshalAs(UnmanagedType.VariantBool)]
		bool IsFarmRunning();

		ILinkFile GetLinkFile([MarshalAs(UnmanagedType.BStr)] string in_bstrLinkFilePath);

		IWorkbookSession OpenWorkbookModel([MarshalAs(UnmanagedType.BStr)] string in_bstrWorkbookPath);

		IWorkbookSession OpenWorkbookModelForRefresh([MarshalAs(UnmanagedType.BStr)] string in_bstrWorkbookPath);

		IWorkbookSession OpenWorkbookSession([MarshalAs(UnmanagedType.BStr)] string in_bstrWorkbookPath, [MarshalAs(UnmanagedType.BStr)] string in_bstrSessionId);

		void Log(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg);

		void Log1(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1);

		void Log2(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2);

		void Log3(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam3);

		void Log4(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam3, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam4);

		[ComVisible(false)]
		IWorkbookSession OpenWorkbookModel(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy);

		[ComVisible(false)]
		IWorkbookSession OpenWorkbookModelForRefresh(string in_bstrWorkbookPath, SessionLifetimePolicy in_lifetimePolicy);

		[ComVisible(false)]
		IWorkbookSession OpenWorkbookSession(string in_bstrWorkbookPath, string in_bstrSessionId, SessionLifetimePolicy in_lifetimePolicy);

		[ComVisible(false)]
		bool IsRunningInFarm(int majorVersion);

		[ComVisible(false)]
		WindowsIdentity GetWindowsIdentityFromCurrentPrincipal();

		[ComVisible(false)]
		void TraceError(string message, params object[] args);

		[ComVisible(false)]
		void TraceVerbose(string message, params object[] args);

		[ComVisible(false)]
		void TraceWarning(string message, params object[] args);
	}
}
