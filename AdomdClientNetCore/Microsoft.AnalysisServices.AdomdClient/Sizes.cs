using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class Sizes
	{
		public int cbMaxToken;

		public int cbMaxSignature;

		public int cbBlockSize;

		public int cbSecurityTrailer;

		internal Sizes(IntPtr unmanagedAddress)
		{
			this.cbMaxToken = Marshal.ReadInt32(unmanagedAddress);
			this.cbMaxSignature = Marshal.ReadInt32(unmanagedAddress, 4);
			this.cbBlockSize = Marshal.ReadInt32(unmanagedAddress, 8);
			this.cbSecurityTrailer = Marshal.ReadInt32(unmanagedAddress, 12);
		}
	}
}
