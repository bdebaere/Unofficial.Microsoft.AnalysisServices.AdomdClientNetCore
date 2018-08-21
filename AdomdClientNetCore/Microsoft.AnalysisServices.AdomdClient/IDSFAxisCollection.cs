using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IDSFAxisCollection : ICollection, IEnumerable
	{
		IDSFDataSet this[int index]
		{
			get;
		}
	}
}
