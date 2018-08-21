using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class TokenSigner
	{
		private SigningKey m_key;

		private SignatureHashAlgorithm m_hashAlgorithm;

		public byte[] CertificateThumbprint
		{
			get
			{
				return this.m_key.CertificateThumbprint;
			}
		}

		public string DigestMethod
		{
			get
			{
				return this.m_hashAlgorithm.Name;
			}
		}

		public TokenSigner(X509Certificate2 certificate, SignatureHashAlgorithm hashAlgorithm) : this(SigningKey.CreateFromCertificate(certificate), hashAlgorithm)
		{
		}

		public TokenSigner(SigningKey key, SignatureHashAlgorithm hashAlgorithm)
		{
			this.m_key = key;
			this.m_hashAlgorithm = hashAlgorithm;
		}

		public byte[] Sign(byte[] data)
		{
			return this.m_key.Sign(data, this.m_hashAlgorithm);
		}
	}
}
