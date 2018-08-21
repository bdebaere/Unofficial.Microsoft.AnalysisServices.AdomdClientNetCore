using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class ActCtxHelper : IDisposable
	{
		private IntPtr InvalidHandleValue = (IntPtr)(-1);

		private IntPtr hCtx;

		private IntPtr lpCookie;

		private bool disposed;

		public ActCtxHelper(string manifestFile)
		{
			this.lpCookie = this.InvalidHandleValue;
			this.hCtx = this.InvalidHandleValue;
			StringBuilder stringBuilder = new StringBuilder(1024);
			if (UnsafeNclNativeMethods.GetModuleFileName(IntPtr.Zero, stringBuilder, stringBuilder.Capacity) == 0u)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			string lpSource = Path.Combine(Path.GetDirectoryName(stringBuilder.ToString()), manifestFile);
			ACTCTX aCTCTX = default(ACTCTX);
			aCTCTX.cbSize = Marshal.SizeOf(typeof(ACTCTX));
			aCTCTX.lpSource = lpSource;
			this.hCtx = UnsafeNclNativeMethods.CreateActCtxW(ref aCTCTX);
			if (this.hCtx == this.InvalidHandleValue)
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
			if (!UnsafeNclNativeMethods.ActivateActCtx(this.hCtx, out this.lpCookie))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		~ActCtxHelper()
		{
			this.Dispose(false);
		}

		private void ReleaseResources()
		{
			if (this.lpCookie != this.InvalidHandleValue)
			{
				UnsafeNclNativeMethods.DeactivateActCtx(0, this.lpCookie);
			}
			this.lpCookie = this.InvalidHandleValue;
			if (this.hCtx == this.InvalidHandleValue)
			{
				UnsafeNclNativeMethods.ReleaseActCtx(this.hCtx);
			}
			this.hCtx = this.InvalidHandleValue;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.ReleaseResources();
				this.disposed = true;
			}
		}
	}
}
