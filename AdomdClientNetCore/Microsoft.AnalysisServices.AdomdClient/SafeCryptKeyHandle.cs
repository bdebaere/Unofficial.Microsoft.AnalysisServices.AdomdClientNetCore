using Microsoft.Win32.SafeHandles;
using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SafeCryptKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeCryptKeyHandle() : base(true)
		{
		}

		internal SafeCryptKeyHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		protected override bool ReleaseHandle()
		{
			return CngNative.NCryptFreeObject(this.handle) == CngNative.ErrorCode.Success;
		}
	}
}
