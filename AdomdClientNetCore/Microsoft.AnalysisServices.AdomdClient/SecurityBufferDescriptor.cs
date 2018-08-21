using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct SecurityBufferDescriptor
	{
		public int version;

		public int count;

		public IntPtr securityBufferArray;

		public static readonly int Size = Marshal.SizeOf(typeof(SecurityBufferDescriptor));

		public SecurityBufferDescriptor(SecurityBufferClass[] buffers)
		{
			this.count = buffers.Length;
			this.version = 0;
			this.securityBufferArray = Marshal.AllocHGlobal((IntPtr)(SecurityBuffer.Size * this.count));
			for (int i = 0; i < this.count; i++)
			{
				buffers[i].unmanagedCopy(IntPtrHelper.Add(this.securityBufferArray, SecurityBuffer.Size * i));
			}
		}

		public SecurityBufferClass[] marshall()
		{
			SecurityBufferClass[] array = new SecurityBufferClass[this.count];
			for (int i = 0; i < this.count; i++)
			{
				array[i] = new SecurityBufferClass(IntPtrHelper.Add(this.securityBufferArray, SecurityBuffer.Size * i));
			}
			return array;
		}

		public void FreeAllBuffers(int flags)
		{
			if (this.securityBufferArray != IntPtr.Zero)
			{
				bool flag = 0 != (flags & 256);
				for (int i = 0; i < this.count; i++)
				{
					IntPtr ptr = IntPtrHelper.Add(this.securityBufferArray, SecurityBuffer.Size * i);
					IntPtr intPtr = Marshal.ReadIntPtr(ptr, 8);
					if (intPtr != IntPtr.Zero && flag)
					{
						int num = UnsafeNclNativeMethods.NativeNTSSPI.FreeContextBuffer(intPtr);
						if (num != 0)
						{
							throw new Win32Exception(num);
						}
					}
				}
				Marshal.FreeHGlobal(this.securityBufferArray);
				this.securityBufferArray = IntPtr.Zero;
			}
		}
	}
}
