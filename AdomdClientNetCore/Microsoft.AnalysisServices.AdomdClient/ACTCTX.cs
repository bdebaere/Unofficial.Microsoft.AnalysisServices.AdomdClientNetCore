using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
	internal struct ACTCTX
	{
		public int cbSize;

		public uint dwFlags;

		public string lpSource;

		public ushort wProcessorArchitecture;

		public short wLangId;

		public string lpAssemblyDirectory;

		public string lpResourceName;

		public string lpApplicationName;

		public IntPtr hModule;
	}
}
