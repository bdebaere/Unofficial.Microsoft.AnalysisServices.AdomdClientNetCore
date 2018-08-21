using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum ContextAttribute
	{
		Sizes,
		Names,
		Lifespan,
		DceInfo,
		StreamSizes,
		Authority = 6,
		KeyInfo = 5,
		PackageInfo = 10,
		RemoteCertificate = 83,
		LocalCertificate,
		RootStore,
		IssuerListInfoEx = 89,
		ConnectionInfo
	}
}
