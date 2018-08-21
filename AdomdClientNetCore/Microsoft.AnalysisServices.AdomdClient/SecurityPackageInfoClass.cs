using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SecurityPackageInfoClass
	{
		internal int Capabilities;

		internal short Version;

		internal short RPCID;

		internal int MaxToken;

		internal string Name;

		internal string Comment;

		internal SecurityPackageInfoClass(IntPtr unmanagedAddress)
		{
			if (unmanagedAddress == IntPtr.Zero)
			{
				return;
			}
			this.Capabilities = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "Capabilities"));
			this.Version = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "Version"));
			this.RPCID = Marshal.ReadInt16(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "RPCID"));
			this.MaxToken = Marshal.ReadInt32(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "MaxToken"));
			IntPtr intPtr = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "Name"));
			if (intPtr != IntPtr.Zero)
			{
				this.Name = Marshal.PtrToStringUni(intPtr);
			}
			intPtr = Marshal.ReadIntPtr(unmanagedAddress, (int)Marshal.OffsetOf(typeof(SecurityPackageInfo), "Comment"));
			if (intPtr != IntPtr.Zero)
			{
				this.Comment = Marshal.PtrToStringUni(intPtr);
			}
		}
	}
}
