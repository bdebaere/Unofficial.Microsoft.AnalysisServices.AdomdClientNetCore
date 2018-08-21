using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class RsaSigningKey : SigningKey
	{
		internal RsaSigningKey(SafeCryptKeyHandle keyHandle, X509Certificate2 certificate) : base(keyHandle, certificate)
		{
		}

		public override byte[] Sign(byte[] data, SignatureHashAlgorithm hashAlgorithm)
		{
			HashAlgorithm hashAlgorithm2 = hashAlgorithm.CreateAlgorithm();
			byte[] array = hashAlgorithm2.ComputeHash(data);
			CngNative.BCRYPT_PKCS1_PADDING_INFO bCRYPT_PKCS1_PADDING_INFO = default(CngNative.BCRYPT_PKCS1_PADDING_INFO);
			bCRYPT_PKCS1_PADDING_INFO.pszAlgId = hashAlgorithm.Name;
			int num;
			CngNative.ErrorCode status = CngNative.NCryptSignHashPkcs1(base.KeyHandle, ref bCRYPT_PKCS1_PADDING_INFO, array, array.Length, null, 0, out num, CngNative.AsymmetricPaddingMode.Pkcs1);
			CngNative.VerifyStatus(status);
			byte[] array2 = new byte[num];
			status = CngNative.NCryptSignHashPkcs1(base.KeyHandle, ref bCRYPT_PKCS1_PADDING_INFO, array, array.Length, array2, array2.Length, out num, CngNative.AsymmetricPaddingMode.Pkcs1);
			CngNative.VerifyStatus(status);
			return array2;
		}
	}
}
