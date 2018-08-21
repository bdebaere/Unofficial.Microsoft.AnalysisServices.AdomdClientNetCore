using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class NamedSet : IMetadataObject
	{
		internal const string namedsetNameColumn = "SET_NAME";

		private const string descriptionColumn = "DESCRIPTION";

		private const string expressionColumn = "EXPRESSION";

		private const string captionColumn = "SET_CAPTION";

		private const string displayFolderColumn = "SET_DISPLAY_FOLDER";

		private DataRow namedSetRow;

		private CubeDef parentCube;

		private AdomdConnection connection;

		private PropertyCollection propertiesCollection;

		private string catalog;

		private string sessionId;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.namedSetRow, "SET_NAME").ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.namedSetRow, "DESCRIPTION").ToString();
			}
		}

		public string Expression
		{
			get
			{
				return AdomdUtils.GetProperty(this.namedSetRow, "EXPRESSION").ToString();
			}
		}

		public string Caption
		{
			get
			{
				if (!this.namedSetRow.Table.Columns.Contains("SET_CAPTION"))
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				return AdomdUtils.GetProperty(this.namedSetRow, "SET_CAPTION").ToString();
			}
		}

		public string DisplayFolder
		{
			get
			{
				if (!this.namedSetRow.Table.Columns.Contains("SET_DISPLAY_FOLDER"))
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				return AdomdUtils.GetProperty(this.namedSetRow, "SET_DISPLAY_FOLDER").ToString();
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
					this.propertiesCollection = new PropertyCollection(this.namedSetRow, this);
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
				return typeof(NamedSet);
			}
		}

		internal NamedSet(AdomdConnection connection, DataRow namedSetRow, CubeDef parentCube, string catalog, string sessionId)
		{
			this.connection = connection;
			this.namedSetRow = namedSetRow;
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

		public static bool operator ==(NamedSet o1, NamedSet o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(NamedSet o1, NamedSet o2)
		{
			return !(o1 == o2);
		}
	}
}
