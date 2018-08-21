using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum PackageCapabilities
	{
		Integrity = 1,
		Privacy,
		Connection = 16,
		Impersonation = 256,
		MutualAuthentication = 65536,
		Delegation = 131072,
		Stream = 1024
	}
}
