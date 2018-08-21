using System;
using System.Security.Cryptography;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class SignatureHashAlgorithm
	{
		internal delegate HashAlgorithm AlgorithmFactory();

		private SignatureHashAlgorithm.AlgorithmFactory m_algorithmFactory;

		public string Name
		{
			get;
			private set;
		}

		internal SignatureHashAlgorithm(SignatureHashAlgorithm.AlgorithmFactory algorithmFactory, string name)
		{
			this.m_algorithmFactory = algorithmFactory;
			this.Name = name;
		}

		internal HashAlgorithm CreateAlgorithm()
		{
			return this.m_algorithmFactory();
		}

		public static SignatureHashAlgorithm CreateSha256()
		{
			return new SignatureHashAlgorithm(() => new SHA256Managed(), "SHA256");
		}

		public static SignatureHashAlgorithm CreateSha384()
		{
			return new SignatureHashAlgorithm(() => new SHA384Managed(), "SHA384");
		}

		public static SignatureHashAlgorithm CreateSha512()
		{
			return new SignatureHashAlgorithm(() => new SHA512Managed(), "SHA512");
		}
	}
}
