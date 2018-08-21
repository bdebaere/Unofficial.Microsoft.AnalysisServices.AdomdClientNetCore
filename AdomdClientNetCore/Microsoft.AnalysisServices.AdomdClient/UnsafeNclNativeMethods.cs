using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[ComVisible(false)]
	internal class UnsafeNclNativeMethods
	{
		[ComVisible(false)]
		internal class NativeNTSSPI
		{
			private const string SECURITY = "security.dll";

			private const string SECUR32 = "secur32.dll";

			private NativeNTSSPI()
			{
			}

			[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int DsMakeSpnW([In] string strClass, [In] string strName, [In] string strInstance, [In] ushort iPort, [In] string strReferrer, [In] [Out] ref uint cchSPN, [Out] StringBuilder strSPN);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int EnumerateSecurityPackagesW(out int pkgnum, out IntPtr arrayptr);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int FreeContextBuffer([In] IntPtr contextBuffer);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int AcquireCredentialsHandleW([In] string principal, [In] string moduleName, [In] int usage, [In] IntPtr logonID, [In] IntPtr nullAuthData, [In] IntPtr keyCallback, [In] IntPtr keyArgument, [In] [Out] ref SecurityHandle credentialsHandle, [In] [Out] ref long expiry);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int FreeCredentialsHandle([In] ref SecurityHandle credentialsHandle);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int InitializeSecurityContextW([In] [Out] ref SecurityHandle credentialHandle, [In] IntPtr contextHandle, [In] string targetName, [In] int requirements, [In] int reservedI, [In] int endianness, [In] IntPtr inputBuffer, [In] int reservedII, [In] [Out] ref SecurityHandle newContext, [In] [Out] ref SecurityBufferDescriptor outputBuffer, [In] [Out] ref int attributes, [In] [Out] ref long timestamp);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int InitializeSecurityContextW([In] [Out] ref SecurityHandle credentialHandle, [In] [Out] ref SecurityHandle contextHandle, [In] string targetName, [In] int requirements, [In] int reservedI, [In] int endianness, [In] [Out] ref SecurityBufferDescriptor inputBuffer, [In] int reservedII, [In] [Out] ref SecurityHandle newContext, [In] [Out] ref SecurityBufferDescriptor outputBuffer, [In] [Out] ref int attributes, [In] [Out] ref long timestamp);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int DeleteSecurityContext([In] ref SecurityHandle handle);

			[DllImport("secur32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int EncryptMessage([In] ref SecurityHandle contextHandle, [In] int qualityOfProtection, [In] [Out] ref SecurityBufferDescriptor input, [In] int sequenceNumber);

			[DllImport("secur32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int DecryptMessage([In] ref SecurityHandle contextHandle, [In] [Out] ref SecurityBufferDescriptor input, [In] int sequenceNumber, [In] [Out] ref int qualityOfProtection);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int QueryContextAttributes([In] ref SecurityHandle phContext, [In] int attribute, [In] IntPtr buffer);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int MakeSignature([In] ref SecurityHandle contextHandle, [In] int qualityOfProtection, [In] [Out] ref SecurityBufferDescriptor input, [In] int sequenceNumber);

			[DllImport("security.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern int VerifySignature([In] ref SecurityHandle contextHandle, [In] [Out] ref SecurityBufferDescriptor input, [In] int sequenceNumber, [In] int qualityOfProtection);
		}

		private UnsafeNclNativeMethods()
		{
		}

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool ImpersonateAnonymousToken([In] IntPtr handle);

		[DllImport("advapi32.dll", SetLastError = true)]
		public static extern bool RevertToSelf();

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetCurrentThread();

		[DllImport("kernel32.dll")]
		public static extern uint GetLastError();

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr CreateActCtxW(ref ACTCTX actctx);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ActivateActCtx(IntPtr hCtx, out IntPtr lpCookie);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeactivateActCtx(int dwFlags, IntPtr lpCookie);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern void ReleaseActCtx(IntPtr hCtx);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern uint GetModuleFileName(IntPtr hModule, [Out] StringBuilder lpBaseName, [MarshalAs(UnmanagedType.U4)] [In] int nSize);
	}
}
