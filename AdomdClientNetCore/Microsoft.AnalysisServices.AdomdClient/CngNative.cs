using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal static class CngNative
	{
		internal static class NCryptProperties
		{
			public const string AlgorithmGroup = "Algorithm Group";

			public const string AlgorithmName = "Algorithm Name";

			public const string KeyLength = "Length";

			public const string Name = "Name";
		}

		internal enum ErrorCode
		{
			Success,
			BadSignature = -2146893818,
			NotFound = -2146893807,
			KeyDoesNotExist = -2146893802,
			BufferTooSmall = -2146893784,
			NoMoreItems = -2146893782
		}

		internal struct BCRYPT_PKCS1_PADDING_INFO
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string pszAlgId;
		}

		internal enum AsymmetricPaddingMode
		{
			None = 1,
			Pkcs1,
			Oaep = 4,
			Pss = 8
		}

		internal const string CRYPT32 = "crypt32.dll";

		internal const string NCRYPT = "ncrypt.dll";

		internal const uint CRYPT_ACQUIRE_CACHE_FLAG = 1u;

		internal const uint CRYPT_ACQUIRE_USE_PROV_INFO_FLAG = 2u;

		internal const uint CRYPT_ACQUIRE_COMPARE_KEY_FLAG = 4u;

		internal const uint CRYPT_ACQUIRE_SILENT_FLAG = 64u;

		internal const uint CRYPT_ACQUIRE_ALLOW_NCRYPT_KEY_FLAG = 65536u;

		internal const uint CRYPT_ACQUIRE_PREFER_NCRYPT_KEY_FLAG = 131072u;

		internal const uint CRYPT_ACQUIRE_ONLY_NCRYPT_KEY_FLAG = 262144u;

		internal const uint CERT_NCRYPT_KEY_SPEC = 4294967295u;

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern bool CryptAcquireCertificatePrivateKey([In] IntPtr pCertContext, [In] uint dwFlags, [In] IntPtr pvReserved, [In] [Out] ref SafeCryptKeyHandle phKeyHandle, [In] [Out] ref uint pdwKeySpec, [In] [Out] ref bool pfCallerFreeProv);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern CngNative.ErrorCode NCryptFreeObject(IntPtr hObject);

		[DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
		internal static extern CngNative.ErrorCode NCryptGetProperty(SafeCryptKeyHandle hObject, string pszProperty, [MarshalAs(UnmanagedType.LPArray)] [Out] byte[] pbOutput, int cbOutput, out int pcbResult, int dwFlags);

		[DllImport("ncrypt.dll")]
		internal static extern CngNative.ErrorCode NCryptSignHash(SafeCryptKeyHandle hKey, IntPtr pPaddingInfo, [MarshalAs(UnmanagedType.LPArray)] byte[] pbHashValue, int cbHashValue, [MarshalAs(UnmanagedType.LPArray)] byte[] pbSignature, int cbSignature, out int pcbResult, int dwFlags);

		[DllImport("ncrypt.dll", EntryPoint = "NCryptSignHash")]
		internal static extern CngNative.ErrorCode NCryptSignHashPkcs1(SafeCryptKeyHandle hKey, [In] ref CngNative.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] pbHashValue, int cbHashValue, [MarshalAs(UnmanagedType.LPArray)] byte[] pbSignature, int cbSignature, out int pcbResult, CngNative.AsymmetricPaddingMode dwFlags);

		internal static void VerifyStatus(CngNative.ErrorCode status)
		{
			if (status != CngNative.ErrorCode.Success)
			{
				throw new Win32Exception((int)status);
			}
		}

		internal static int NCryptGetPropertyInt32(SafeCryptKeyHandle hObject, string propertyName)
		{
			byte[] array = new byte[4];
			int num;
			CngNative.ErrorCode errorCode = CngNative.NCryptGetProperty(hObject, propertyName, array, array.Length, out num, 0);
			if (errorCode != CngNative.ErrorCode.Success)
			{
				throw new Win32Exception((int)errorCode);
			}
			return BitConverter.ToInt32(array, 0);
		}

		internal static string NCryptGetPropertyString(SafeCryptKeyHandle hObject, string propertyName)
		{
			int num = 0;
			CngNative.ErrorCode errorCode = CngNative.NCryptGetProperty(hObject, propertyName, null, 0, out num, 0);
			if (errorCode != CngNative.ErrorCode.Success)
			{
				throw new Win32Exception((int)errorCode);
			}
			byte[] array = new byte[num];
			errorCode = CngNative.NCryptGetProperty(hObject, propertyName, array, array.Length, out num, 0);
			if (errorCode != CngNative.ErrorCode.Success)
			{
				throw new Win32Exception((int)errorCode);
			}
			string arg_4E_0 = Encoding.Unicode.GetString(array, 0, num);
			char[] trimChars = new char[1];
			return arg_4E_0.Trim(trimChars);
		}

		internal static SafeCryptKeyHandle GetCryptKeyHandleFromCertificatePrivateKey(X509Certificate2 cert)
		{
			SafeCryptKeyHandle result = new SafeCryptKeyHandle();
			uint num = 0u;
			bool flag = false;
			if (!CngNative.CryptAcquireCertificatePrivateKey(cert.Handle, 262208u, IntPtr.Zero, ref result, ref num, ref flag))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastWin32Error);
			}
			if (!flag || num != 4294967295u)
			{
				throw new InvalidOperationException();
			}
			return result;
		}
	}
}
