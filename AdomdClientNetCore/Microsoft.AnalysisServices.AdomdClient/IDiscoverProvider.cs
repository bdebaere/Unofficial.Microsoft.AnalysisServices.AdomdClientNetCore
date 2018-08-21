using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IDiscoverProvider
	{
		void Discover(string requestType, IDictionary restrictions, DataTable table);

		void DiscoverData(string requestType, IDictionary restrictions, DataTable table);

		RowsetFormatter Discover(string requestType, IDictionary restrictions);
	}
}
