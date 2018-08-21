using System;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SecurityBufferClass
	{
		public int offset;

		public int size;

		public int type;

		public byte[] token;

		public SecurityBufferClass(byte[] data, BufferType tokentype)
		{
			this.offset = 0;
			this.size = ((data == null) ? 0 : data.Length);
			this.type = (int)tokentype;
			this.token = data;
		}

		public SecurityBufferClass(int size, BufferType tokentype)
		{
			this.offset = 0;
			this.size = size;
			this.type = (int)tokentype;
			this.token = ((size == 0) ? null : new byte[size]);
		}

		public SecurityBufferClass(IntPtr unmanagedAddress)
		{
			this.offset = 0;
			this.size = Marshal.ReadInt32(unmanagedAddress);
			this.type = Marshal.ReadInt32(unmanagedAddress, 4);
			if (this.size > 0)
			{
				this.token = new byte[this.size];
				IntPtr source = Marshal.ReadIntPtr(unmanagedAddress, 8);
				Marshal.Copy(source, this.token, 0, this.size);
				return;
			}
			this.token = null;
		}

		internal void unmanagedCopy(IntPtr unmanagedAddress)
		{
			IntPtr val = (this.token == null) ? IntPtr.Zero : Marshal.UnsafeAddrOfPinnedArrayElement(this.token, this.offset);
			Marshal.WriteInt32(unmanagedAddress, 0, this.size);
			Marshal.WriteInt32(unmanagedAddress, 4, this.type);
			Marshal.WriteIntPtr(unmanagedAddress, 8, val);
		}
	}
}
