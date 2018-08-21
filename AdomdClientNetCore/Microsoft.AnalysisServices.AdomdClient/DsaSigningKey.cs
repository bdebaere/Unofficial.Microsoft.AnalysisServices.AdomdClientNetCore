using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class DsaSigningKey : SigningKey
	{
		internal DsaSigningKey(SafeCryptKeyHandle keyHandle, X509Certificate2 certificate) : base(keyHandle, certificate)
		{
		}

		public override byte[] Sign(byte[] data, SignatureHashAlgorithm hashAlgorithm)
		{
			HashAlgorithm hashAlgorithm2 = hashAlgorithm.CreateAlgorithm();
			byte[] array = hashAlgorithm2.ComputeHash(data);
			int num;
			CngNative.ErrorCode status = CngNative.NCryptSignHash(base.KeyHandle, IntPtr.Zero, array, array.Length, null, 0, out num, 0);
			CngNative.VerifyStatus(status);
			byte[] array2 = new byte[num];
			status = CngNative.NCryptSignHash(base.KeyHandle, IntPtr.Zero, array, array.Length, array2, array2.Length, out num, 0);
			CngNative.VerifyStatus(status);
			return array2;
		}
	}
}
