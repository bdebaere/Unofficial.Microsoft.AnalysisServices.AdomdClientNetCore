using System;
using System.ComponentModel;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SecurityContext
	{
		public SecurityHandle Handle;

		public long TimeStamp;

		public SecurityContextMode SecurityContextMode
		{
			get;
			private set;
		}

		public SecurityContext(SecurityContextMode securityContextMode)
		{
			this.Handle.Reset();
			this.SecurityContextMode = securityContextMode;
		}

		~SecurityContext()
		{
			try
			{
				this.Close();
			}
			catch (Win32Exception)
			{
			}
		}

		internal void Close()
		{
			if (this.Handle.Initialized)
			{
				int num = UnsafeNclNativeMethods.NativeNTSSPI.DeleteSecurityContext(ref this.Handle);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
				this.Handle.Reset();
			}
		}
	}
}
