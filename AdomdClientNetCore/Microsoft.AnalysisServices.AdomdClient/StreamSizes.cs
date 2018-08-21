using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class StreamSizes
	{
		public int cbHeader
		{
			get;
			private set;
		}

		public int cbTrailer
		{
			get;
			private set;
		}

		public int cbMaxMessage
		{
			get;
			private set;
		}

		public int cBuffers
		{
			get;
			private set;
		}

		public int cbBlockSz
		{
			get;
			private set;
		}

		internal StreamSizes(IntPtr unmanagedAddress)
		{
			int num = Marshal.ReadInt32(unmanagedAddress, 0);
			this.cbHeader = num;
			num = Marshal.ReadInt32(unmanagedAddress, 4);
			this.cbTrailer = num;
			num = Marshal.ReadInt32(unmanagedAddress, 8);
			this.cbMaxMessage = num;
			num = Marshal.ReadInt32(unmanagedAddress, 12);
			this.cBuffers = num;
			num = Marshal.ReadInt32(unmanagedAddress, 16);
			this.cbBlockSz = num;
		}
	}
}
