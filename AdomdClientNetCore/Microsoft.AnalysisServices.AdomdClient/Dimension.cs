using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Dimension : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		internal HierarchyCollection hierarchies;

		private HierarchyCollection attribHierarchies;

		private PropertyCollection propertiesCollection;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string dimensionNameColumn = "DIMENSION_NAME";

		private static string descriptionColumn = "DESCRIPTION";

		internal static string uniqueNameColumn = "DIMENSION_UNIQUE_NAME";

		private static string typeColumn = "DIMENSION_TYPE";

		private static string writeEnabledColumn = "IS_READWRITE";

		private static string captionColumn = "DIMENSION_CAPTION";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.DimensionRow, Dimension.dimensionNameColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return ((IAdomdBaseObject)this).InternalUniqueName;
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.DimensionRow, Dimension.descriptionColumn).ToString();
			}
		}

		public CubeDef ParentCube
		{
			get
			{
				return (CubeDef)this.baseData.ParentObject;
			}
		}

		public DimensionTypeEnum DimensionType
		{
			get
			{
				long num = (long)Convert.ToInt32(AdomdUtils.GetProperty(this.DimensionRow, Dimension.typeColumn), CultureInfo.InvariantCulture);
				if (num >= 0L && num <= 17L)
				{
					return (DimensionTypeEnum)num;
				}
				return DimensionTypeEnum.Other;
			}
		}

		public bool WriteEnabled
		{
			get
			{
				return Convert.ToBoolean(AdomdUtils.GetProperty(this.DimensionRow, Dimension.writeEnabledColumn), CultureInfo.InvariantCulture);
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.DimensionRow, Dimension.captionColumn).ToString();
			}
		}

		public HierarchyCollection Hierarchies
		{
			get
			{
				if (this.hierarchies == null)
				{
					this.hierarchies = new HierarchyCollection(this.baseData.Connection, this, false);
				}
				else
				{
					this.hierarchies.CollectionInternal.CheckCache();
				}
				return this.hierarchies;
			}
		}

		public HierarchyCollection AttributeHierarchies
		{
			get
			{
				if (!this.baseData.Connection.IsPostYukonProvider())
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				if (this.attribHierarchies == null)
				{
					this.attribHierarchies = new HierarchyCollection(this.baseData.Connection, this, true);
				}
				return this.attribHierarchies;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.DimensionRow, this);
				}
				return this.propertiesCollection;
			}
		}

		AdomdConnection IAdomdBaseObject.Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		bool IAdomdBaseObject.IsMetadata
		{
			get
			{
				return this.baseData.IsMetadata;
			}
			set
			{
				this.baseData.IsMetadata = value;
			}
		}

		object IAdomdBaseObject.MetadataData
		{
			get
			{
				return this.baseData.MetadataData;
			}
			set
			{
				this.baseData.MetadataData = value;
			}
		}

		IAdomdBaseObject IAdomdBaseObject.ParentObject
		{
			get
			{
				return this.baseData.ParentObject;
			}
			set
			{
				this.baseData.ParentObject = value;
			}
		}

		string IAdomdBaseObject.CubeName
		{
			get
			{
				return ((CubeDef)this.baseData.ParentObject).Name;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeDimension;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.DimensionRow, Dimension.uniqueNameColumn).ToString();
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				return this.baseData.Catalog;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				return this.baseData.SessionID;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				return ((IAdomdBaseObject)this).CubeName;
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
				return typeof(Dimension);
			}
		}

		internal DataRow DimensionRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal Dimension(AdomdConnection connection, DataRow dimensionRow, CubeDef parentCube, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, dimensionRow, parentCube, null, catalog, sessionId);
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

		public static bool operator ==(Dimension o1, Dimension o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Dimension o1, Dimension o2)
		{
			return !(o1 == o2);
		}
	}
}
