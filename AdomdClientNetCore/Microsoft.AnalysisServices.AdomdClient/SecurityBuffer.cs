using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SecurityBuffer
	{
		public int count;

		public int type;

		public IntPtr buffer;

		public static readonly int Size = Marshal.SizeOf(typeof(SecurityBuffer));
	}
}
