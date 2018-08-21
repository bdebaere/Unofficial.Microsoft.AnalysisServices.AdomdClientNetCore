using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CellInfo
	{
		private DataTable cellsTable;

		private OlapInfoPropertyCollection properties;

		public OlapInfoPropertyCollection CellProperties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new OlapInfoPropertyCollection(this.cellsTable);
				}
				return this.properties;
			}
		}

		internal CellInfo(MDDatasetFormatter formatter)
		{
			this.cellsTable = formatter.CellTable;
			this.properties = null;
		}
	}
}
