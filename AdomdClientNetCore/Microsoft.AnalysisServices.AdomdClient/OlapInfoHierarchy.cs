using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class OlapInfoHierarchy
	{
		private DataTable hierarchyTable;

		private OlapInfoPropertyCollection properties;

		public string Name
		{
			get
			{
				return this.hierarchyTable.TableName;
			}
		}

		public OlapInfoPropertyCollection HierarchyProperties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new OlapInfoPropertyCollection(this.hierarchyTable);
				}
				return this.properties;
			}
		}

		internal OlapInfoHierarchy(DataTable hierarchyTable)
		{
			this.hierarchyTable = hierarchyTable;
			this.properties = null;
		}
	}
}
