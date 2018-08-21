using System;

namespace Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces
{
	[Flags]
	internal enum SessionLifetimePolicy
	{
		None = 0,
		CloseSessionOnDispose = 1,
		KeepActiveSessionAlive = 2,
		VersionCheckOnSave = 4
	}
}
