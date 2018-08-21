using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IDSFDataSet : ICollection, IEnumerable
	{
		string DataSetName
		{
			get;
		}

		DataTable this[int index]
		{
			get;
		}

		DataTable this[string index]
		{
			get;
		}

		bool Contains(string tableName);
	}
}
