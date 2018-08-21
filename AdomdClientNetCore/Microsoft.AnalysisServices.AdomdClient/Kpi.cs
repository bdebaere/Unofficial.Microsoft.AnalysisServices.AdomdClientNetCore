using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Kpi : IMetadataObject
	{
		private DataRow kpiRow;

		private CubeDef parentCube;

		private AdomdConnection connection;

		private PropertyCollection propertiesCollection;

		private string catalog;

		private string sessionId;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string kpiNameColumn = "KPI_NAME";

		private static string descriptionColumn = "KPI_DESCRIPTION";

		private static string displayFolderColumn = "KPI_DISPLAY_FOLDER";

		private static string trendGraphicColumn = "KPI_TREND_GRAPHIC";

		private static string statusGraphicColumn = "KPI_STATUS_GRAPHIC";

		private static string parentKpiNameColumn = "KPI_PARENT_KPI_NAME";

		internal static string kpiCaptionColumn = "KPI_CAPTION";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.kpiNameColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.descriptionColumn).ToString();
			}
		}

		public string DisplayFolder
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.displayFolderColumn).ToString();
			}
		}

		public string TrendGraphic
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.trendGraphicColumn).ToString();
			}
		}

		public string StatusGraphic
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.statusGraphicColumn).ToString();
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.kpiRow, Kpi.kpiCaptionColumn).ToString();
			}
		}

		public Kpi ParentKpi
		{
			get
			{
				string text = null;
				if (this.kpiRow.Table.Columns.Contains(Kpi.parentKpiNameColumn))
				{
					text = (AdomdUtils.GetProperty(this.kpiRow, Kpi.parentKpiNameColumn) as string);
				}
				if (text == null || text.Length == 0)
				{
					return null;
				}
				return this.ParentCube.InternalGetSchemaObject(SchemaObjectType.ObjectTypeKpi, text) as Kpi;
			}
		}

		public CubeDef ParentCube
		{
			get
			{
				return this.parentCube;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.kpiRow, this);
				}
				return this.propertiesCollection;
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.connection;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				return this.catalog;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				return this.ParentCube.Name;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(Kpi);
			}
		}

		internal Kpi(AdomdConnection connection, DataRow kpiRow, CubeDef parentCube, string catalog, string sessionId)
		{
			this.connection = connection;
			this.kpiRow = kpiRow;
			this.parentCube = parentCube;
			this.propertiesCollection = null;
			this.catalog = catalog;
			this.sessionId = sessionId;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				this.hashCode = AdomdUtils.GetHashCode(this);
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			return AdomdUtils.Equals(this, obj as IMetadataObject);
		}

		public static bool operator ==(Kpi o1, Kpi o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Kpi o1, Kpi o2)
		{
			return !(o1 == o2);
		}
	}
}
