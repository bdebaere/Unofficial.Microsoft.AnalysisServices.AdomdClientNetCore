using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Hierarchy : IAdomdBaseObject, IMetadataObject, ISubordinateObject
	{
		private BaseObjectData baseData;

		private LevelCollection levels;

		private PropertyCollection propertyCollection;

		private Axis axis;

		private int hierarchyOrdinal = -1;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string hierarchyNameColumn = "HIERARCHY_NAME";

		internal static string descriptionColumn = "DESCRIPTION";

		internal static string uniqueNameColumn = HierarchyCollectionInternal.hierUNameRest;

		internal static string defaultMemberColumn = "DEFAULT_MEMBER";

		internal static string captionColumn = "HIERARCHY_CAPTION";

		internal static string isAttribHierColumn = "HIERARCHY_ORIGIN";

		internal static string displayFolderColumn = "HIERARCHY_DISPLAY_FOLDER";

		internal static string structureColumn = "STRUCTURE";

		private static int structureUnbalanced = 2;

		private static int MD_USER_DEFINED = 1;

		private static int MD_SYSTEM_ENABLED = 2;

		private static int PC_MASK = Hierarchy.MD_USER_DEFINED | Hierarchy.MD_SYSTEM_ENABLED;

		public string Name
		{
			get
			{
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				string text = AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.hierarchyNameColumn).ToString();
				if (text.Length == 0)
				{
					return ((Dimension)this.baseData.ParentObject).Name;
				}
				return text;
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
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				return AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.descriptionColumn).ToString();
			}
		}

		public Dimension ParentDimension
		{
			get
			{
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				if (this.baseData.ParentObject == null)
				{
					this.baseData.ParentObject = (this.Connection.GetObjectData(SchemaObjectType.ObjectTypeDimension, this.baseData.CubeName, this.DimensionUniqueName) as IAdomdBaseObject);
				}
				return (Dimension)this.baseData.ParentObject;
			}
		}

		public string DefaultMember
		{
			get
			{
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				return AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.defaultMemberColumn).ToString();
			}
		}

		public string DisplayFolder
		{
			get
			{
				if (this.Connection == null)
				{
					throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
				}
				AdomdUtils.CheckConnectionOpened(this.Connection);
				if (!this.Connection.IsPostYukonProvider())
				{
					throw new NotSupportedException(SR.NotSupportedByProvider);
				}
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				return AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.displayFolderColumn).ToString();
			}
		}

		public string Caption
		{
			get
			{
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				return AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.captionColumn).ToString();
			}
		}

		public HierarchyOrigin HierarchyOrigin
		{
			get
			{
				if (this.Connection == null)
				{
					throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
				}
				AdomdUtils.CheckConnectionOpened(this.Connection);
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				if (!this.Connection.IsPostYukonProvider())
				{
					int num = Convert.ToInt32(AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.structureColumn), CultureInfo.InvariantCulture);
					if (Hierarchy.structureUnbalanced == num)
					{
						return HierarchyOrigin.ParentChildHierarchy;
					}
					return HierarchyOrigin.UserHierarchy;
				}
				else
				{
					int num2 = (int)Convert.ToInt16(AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.isAttribHierColumn), CultureInfo.InvariantCulture);
					if ((num2 & Hierarchy.PC_MASK) == Hierarchy.PC_MASK)
					{
						return HierarchyOrigin.ParentChildHierarchy;
					}
					if ((num2 & Hierarchy.MD_SYSTEM_ENABLED) == Hierarchy.MD_SYSTEM_ENABLED)
					{
						return HierarchyOrigin.AttributeHierarchy;
					}
					if ((num2 & Hierarchy.MD_USER_DEFINED) == Hierarchy.MD_USER_DEFINED)
					{
						return HierarchyOrigin.UserHierarchy;
					}
					return HierarchyOrigin.UserHierarchy;
				}
			}
		}

		public LevelCollection Levels
		{
			get
			{
				if (this.levels != null)
				{
					this.levels.CollectionInternal.CheckCache();
				}
				if (!this.baseData.IsMetadata)
				{
					AdomdUtils.PopulateSymetry(this);
				}
				if (this.levels == null)
				{
					this.levels = new LevelCollection(this.Connection, this);
				}
				return this.levels;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					if (!this.baseData.IsMetadata)
					{
						AdomdUtils.PopulateSymetry(this);
					}
					this.propertyCollection = new PropertyCollection(this.HierarchyRow, this);
				}
				return this.propertyCollection;
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
				if (this.baseData.IsMetadata)
				{
					return ((Dimension)this.baseData.ParentObject).ParentCube.Name;
				}
				return this.baseData.CubeName;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeHierarchy;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				if (!this.baseData.IsMetadata)
				{
					return this.HierarchyTable.TableName;
				}
				return AdomdUtils.GetProperty(this.HierarchyRow, Hierarchy.uniqueNameColumn).ToString();
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return this.Connection;
				}
				return null;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return this.baseData.Catalog;
				}
				return null;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return this.baseData.SessionID;
				}
				return null;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return this.ParentDimension.ParentCube.Name;
				}
				return null;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return this.UniqueName;
				}
				return null;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(Hierarchy);
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return null;
				}
				return this.axis;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				if (!this.IsFromCellSet)
				{
					return -1;
				}
				return this.hierarchyOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Hierarchy);
			}
		}

		internal DataTable HierarchyTable
		{
			get
			{
				return (DataTable)this.baseData.AxisData;
			}
		}

		internal DataRow HierarchyRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		internal string DimensionUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.HierarchyRow, Dimension.uniqueNameColumn).ToString();
			}
		}

		private AdomdConnection Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		private bool IsFromCellSet
		{
			get
			{
				return this.HierarchyTable != null;
			}
		}

		internal Hierarchy(AdomdConnection connection, DataTable hierarchyTable, string cubeName, Axis axis, int hierarchyOrdinal)
		{
			this.baseData = new BaseObjectData(connection, false, hierarchyTable, null, null, cubeName, null, null);
			this.axis = axis;
			this.hierarchyOrdinal = hierarchyOrdinal;
		}

		internal Hierarchy(AdomdConnection connection, DataRow hierarchyRow, Dimension parentDimension, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, hierarchyRow, parentDimension, null, catalog, sessionId);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				if (this.IsFromCellSet)
				{
					//this.hashCode = AdomdUtils.GetHashCode(this);
				}
				else
				{
					//this.hashCode = AdomdUtils.GetHashCode(this);
				}
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			if (!this.IsFromCellSet)
			{
				return AdomdUtils.Equals(this, obj as IMetadataObject);
			}
			return AdomdUtils.Equals(this, obj as ISubordinateObject);
		}

		public static bool operator ==(Hierarchy o1, Hierarchy o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Hierarchy o1, Hierarchy o2)
		{
			return !(o1 == o2);
		}
	}
}
