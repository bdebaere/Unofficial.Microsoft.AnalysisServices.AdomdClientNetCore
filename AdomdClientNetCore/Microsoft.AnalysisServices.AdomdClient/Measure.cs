using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Measure : IMetadataObject
	{
		private DataRow measureRow;

		private CubeDef parentCube;

		private AdomdConnection connection;

		private PropertyCollection properties;

		private string catalog;

		private string sessionId;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string measureNameColumn = "MEASURE_NAME";

		private static string descriptionColumn = "DESCRIPTION";

		internal static string uniqueNameColumn = "MEASURE_UNIQUE_NAME";

		private static string captionColumn = "MEASURE_CAPTION";

		private static string displayFolderColumn = "MEASURE_DISPLAY_FOLDER";

		private static string precisionColumn = "NUMERIC_PRECISION";

		private static string scaleColumn = "NUMERIC_SCALE";

		private static string unitsColumn = "MEASURE_UNITS";

		private static string expressionColumn = "EXPRESSION";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.measureNameColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.uniqueNameColumn).ToString();
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.captionColumn).ToString();
			}
		}

		public string DisplayFolder
		{
			get
			{
				if (!this.connection.IsPostYukonProvider())
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				return AdomdUtils.GetProperty(this.measureRow, Measure.displayFolderColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.descriptionColumn).ToString();
			}
		}

		public int NumericPrecision
		{
			get
			{
				return Convert.ToInt32(AdomdUtils.GetProperty(this.measureRow, Measure.precisionColumn), CultureInfo.InvariantCulture);
			}
		}

		public short NumericScale
		{
			get
			{
				return Convert.ToInt16(AdomdUtils.GetProperty(this.measureRow, Measure.scaleColumn), CultureInfo.InvariantCulture);
			}
		}

		public string Units
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.unitsColumn).ToString();
			}
		}

		public string Expression
		{
			get
			{
				return AdomdUtils.GetProperty(this.measureRow, Measure.expressionColumn).ToString();
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
				if (this.properties == null)
				{
					this.properties = new PropertyCollection(this.measureRow, this);
				}
				return this.properties;
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
				return this.UniqueName;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(Measure);
			}
		}

		internal Measure(AdomdConnection connection, DataRow measureRow, CubeDef parentCube, string catalog, string sessionId)
		{
			this.connection = connection;
			this.measureRow = measureRow;
			this.parentCube = parentCube;
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

		public static bool operator ==(Measure o1, Measure o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Measure o1, Measure o2)
		{
			return !(o1 == o2);
		}
	}
}
