using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class LocalServerNativeMethods
	{
		private LocalServerNativeMethods()
		{
		}

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamClose();

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamCloseBase();

		[DllImport("msmgdsrv.dll")]
		public static extern int MSMDLocalStreamRead([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [In] [Out] byte[] buffer, [MarshalAs(UnmanagedType.I4)] [In] int offset, [MarshalAs(UnmanagedType.I4)] [In] int size);

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamWrite([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] [In] byte[] buffer, [MarshalAs(UnmanagedType.I4)] [In] int offset, [MarshalAs(UnmanagedType.I4)] [In] int size);

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamWriteEndOfMessage();

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamSkip();

		[DllImport("msmgdsrv.dll")]
		public static extern void MSMDLocalStreamFlush();
	}
}
