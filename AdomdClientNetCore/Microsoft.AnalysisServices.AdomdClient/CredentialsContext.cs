using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class CredentialsContext
	{
		public SecurityHandle Handle;

		public long TimeStamp;

		public CredentialsContext(string package, CredentialUse intent)
		{
			this.Handle.Reset();
			int num = UnsafeNclNativeMethods.NativeNTSSPI.AcquireCredentialsHandleW(null, package, (int)intent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, ref this.Handle, ref this.TimeStamp);
			if (num != 0)
			{
				throw new Win32Exception(num);
			}
		}

		public CredentialsContext(X509Certificate2 clientCertificate)
		{
			this.Handle.Reset();
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			IntPtr zero = IntPtr.Zero;
			try
			{
				SchannelCred schannelCred;
				schannelCred.Version = SchannelCredVersion.VERSION;
				if (clientCertificate != null && clientCertificate.Handle != IntPtr.Zero)
				{
					intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));
					Marshal.WriteIntPtr(intPtr, clientCertificate.Handle);
					schannelCred.CredsCount = 1u;
					schannelCred.CredsPtr = intPtr;
				}
				else
				{
					schannelCred.CredsCount = 0u;
					schannelCred.CredsPtr = IntPtr.Zero;
				}
				schannelCred.RootStoreHandle = IntPtr.Zero;
				schannelCred.MappersCount = 0u;
				schannelCred.MapperHandlesPtr = IntPtr.Zero;
				schannelCred.SupportedAlgsCount = 0u;
				schannelCred.SupportedAlgsPtr = IntPtr.Zero;
				schannelCred.EnabledProtocolsBits = SchannelProtocols.SP_PROT_TLS1_X_CLIENT;
				schannelCred.MinimumCipherStrength = 0u;
				schannelCred.MaximumCipherStrength = 0u;
				schannelCred.SessionLifespan = 0u;
				schannelCred.Flags = (SchannelCredFlags.SCH_CRED_AUTO_CRED_VALIDATION | SchannelCredFlags.SCH_CRED_NO_DEFAULT_CREDS);
				schannelCred.CredFormat = 0u;
				intPtr2 = Marshal.AllocHGlobal(SchannelCred.Size);
				Marshal.StructureToPtr(schannelCred, intPtr2, false);
				int num = UnsafeNclNativeMethods.NativeNTSSPI.AcquireCredentialsHandleW(null, "Microsoft Unified Security Protocol Provider", 2, IntPtr.Zero, intPtr2, IntPtr.Zero, IntPtr.Zero, ref this.Handle, ref this.TimeStamp);
				GC.KeepAlive(clientCertificate);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(zero);
				Marshal.FreeHGlobal(intPtr);
				Marshal.FreeHGlobal(intPtr2);
			}
		}

		~CredentialsContext()
		{
			try
			{
				this.Close();
			}
			catch (Win32Exception)
			{
			}
		}

		public void Close()
		{
			if (this.Handle.Initialized)
			{
				int num = UnsafeNclNativeMethods.NativeNTSSPI.FreeCredentialsHandle(ref this.Handle);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
				this.Handle.Reset();
			}
		}
	}
}
