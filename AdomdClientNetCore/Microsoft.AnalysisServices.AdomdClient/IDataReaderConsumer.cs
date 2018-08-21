using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IDataReaderConsumer
	{
		void SetDataReader(AdomdDataReader reader);
	}
}
