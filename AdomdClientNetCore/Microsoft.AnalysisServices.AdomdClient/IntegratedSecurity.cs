using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal enum IntegratedSecurity
	{
		Sspi,
		Basic,
		Federated,
		ClaimsToken,
		Unspecified
	}
}
