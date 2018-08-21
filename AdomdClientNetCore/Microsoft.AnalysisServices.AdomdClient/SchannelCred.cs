using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal struct SchannelCred
	{
		public uint Version;

		public uint CredsCount;

		public IntPtr CredsPtr;

		public IntPtr RootStoreHandle;

		public uint MappersCount;

		public IntPtr MapperHandlesPtr;

		public uint SupportedAlgsCount;

		public IntPtr SupportedAlgsPtr;

		public uint EnabledProtocolsBits;

		public uint MinimumCipherStrength;

		public uint MaximumCipherStrength;

		public uint SessionLifespan;

		public uint Flags;

		public uint CredFormat;

		public static readonly int Size = Marshal.SizeOf(typeof(SchannelCred));
	}
}
