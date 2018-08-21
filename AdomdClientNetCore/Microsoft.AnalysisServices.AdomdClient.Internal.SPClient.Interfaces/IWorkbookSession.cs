using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces
{
	internal interface IWorkbookSession : IDisposable
	{
		ConnectionStyle ConnectionStyle
		{
			get;
		}

		string Database
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		string Server
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		string SessionId
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		string UserName
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		WorkbookFileFormat WorkbookFormatVersion
		{
			get;
		}

		string WorkbookPath
		{
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		void BeginActivity();

		IStream CreateNativeStream();

		void EndActivity();

		void EndSession();

		void Refresh([MarshalAs(UnmanagedType.BStr)] string in_bstrTargetApplicationId, [MarshalAs(UnmanagedType.BStr)] string in_bstrConnectionName);

		void ReportQueryExecution(int elapsedTime, [MarshalAs(UnmanagedType.BStr)] string in_bstrQuery);

		void Log(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg);

		void Log1(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1);

		void Log2(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2);

		void Log3(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam3);

		void Log4(TraceLevel e_inTraceLevel, [MarshalAs(UnmanagedType.BStr)] string bstr_inMsg, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam1, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam2, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam3, [MarshalAs(UnmanagedType.BStr)] string bstr_inParam4);

		void EnsureValidSession();

		[ComVisible(false)]
		Stream CreateManagedStream();

		[ComVisible(false)]
		string[] GetWorkbookConnections();

		[ComVisible(false)]
		void RefreshEmbeddedModel();

		[ComVisible(false)]
		void Save();

		[ComVisible(false)]
		void TraceError(string message, params object[] args);

		[ComVisible(false)]
		void TraceVerbose(string message, params object[] args);

		[ComVisible(false)]
		void TraceWarning(string message, params object[] args);
	}
}
