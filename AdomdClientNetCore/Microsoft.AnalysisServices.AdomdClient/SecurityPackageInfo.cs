using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal struct SecurityPackageInfo
	{
		public int Capabilities;

		public short Version;

		public short RPCID;

		public int MaxToken;

		public IntPtr Name;

		public IntPtr Comment;

		public static readonly int Size = Marshal.SizeOf(typeof(SecurityPackageInfo));
	}
}
