using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class SigningKey
	{
		internal SafeCryptKeyHandle KeyHandle
		{
			get;
			private set;
		}

		public X509Certificate2 Certificate
		{
			get;
			private set;
		}

		public byte[] CertificateThumbprint
		{
			get;
			private set;
		}

		public static SigningKey CreateFromCertificate(X509Certificate2 certificate)
		{
			SafeCryptKeyHandle cryptKeyHandleFromCertificatePrivateKey = CngNative.GetCryptKeyHandleFromCertificatePrivateKey(certificate);
			string text = CngNative.NCryptGetPropertyString(cryptKeyHandleFromCertificatePrivateKey, "Algorithm Group");
			if (text.Equals("RSA", StringComparison.OrdinalIgnoreCase))
			{
				return new RsaSigningKey(cryptKeyHandleFromCertificatePrivateKey, certificate);
			}
			return new DsaSigningKey(cryptKeyHandleFromCertificatePrivateKey, certificate);
		}

		internal SigningKey(SafeCryptKeyHandle keyHandle, X509Certificate2 certificate)
		{
			this.KeyHandle = keyHandle;
			this.Certificate = certificate;
			this.CertificateThumbprint = SigningKey.StringToByteArray(this.Certificate.Thumbprint);
		}

		public abstract byte[] Sign(byte[] data, SignatureHashAlgorithm hashAlgorithm);

		private static byte[] StringToByteArray(string hex)
		{
			int length = hex.Length;
			byte[] array = new byte[length / 2];
			for (int i = 0; i < length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return array;
		}
	}
}
