using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class LibraryHandle : SafeHandle
	{
		public override bool IsInvalid
		{
			get
			{
				return this.handle == IntPtr.Zero || base.IsClosed;
			}
		}

		protected LibraryHandle() : base(IntPtr.Zero, true)
		{
		}

		protected override bool ReleaseHandle()
		{
			return LibraryHandle.FreeLibrary(this.handle) != 0;
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int FreeLibrary([In] IntPtr hModule);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetProcAddress([In] LibraryHandle hModule, [MarshalAs(UnmanagedType.LPStr)] [In] string lpProcName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint GetLastError();

		protected void ThrowOnError()
		{
			throw new Win32Exception((int)LibraryHandle.GetLastError());
		}

		protected IntPtr CheckEmptyHandle(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				this.ThrowOnError();
			}
			return handle;
		}

		protected Delegate GetDelegate(string functionName, Type delegateType)
		{
			IntPtr procAddress = LibraryHandle.GetProcAddress(this, functionName);
			if (procAddress == IntPtr.Zero)
			{
				throw new Win32Exception((int)LibraryHandle.GetLastError());
			}
			return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
		}
	}
}
