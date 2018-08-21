using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IObjectCache
	{
		bool IsPopulated
		{
			get;
		}

		DataSet CacheDataSet
		{
			get;
		}

		void Populate();

		DataRowCollection GetNonFilteredRows();

		DataRow[] GetFilteredRows(DataRow parentRow, string filter);

		void Refresh();

		void CheckCacheIsValid();

		void MarkNeedCheckForValidness();

		void MarkAbandoned();
	}
}
