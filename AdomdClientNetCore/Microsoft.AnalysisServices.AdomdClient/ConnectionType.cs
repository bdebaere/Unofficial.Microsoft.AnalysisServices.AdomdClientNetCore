using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum ConnectionType
	{
		Native,
		Http,
		LocalServer,
		LocalCube,
		Wcf,
		LocalFarm,
		OnPremFromCloudAccess
	}
}
