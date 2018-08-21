using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IXmlaDataReaderOwner
	{
		void CloseConnection(bool endSession);
	}
}
