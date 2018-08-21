using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoAxis
	{
		private IDSFDataSet axisDataSet;

		private OlapInfoHierarchyCollection hierarchies;

		public string Name
		{
			get
			{
				return this.axisDataSet.DataSetName;
			}
		}

		public OlapInfoHierarchyCollection Hierarchies
		{
			get
			{
				if (this.hierarchies == null)
				{
					this.hierarchies = new OlapInfoHierarchyCollection(this.axisDataSet);
				}
				return this.hierarchies;
			}
		}

		internal OlapInfoAxis(IDSFDataSet axisDataSet)
		{
			this.axisDataSet = axisDataSet;
		}
	}
}
