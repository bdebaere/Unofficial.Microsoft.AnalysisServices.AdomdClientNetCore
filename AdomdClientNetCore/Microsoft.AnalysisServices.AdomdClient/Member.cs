using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Member : IAdomdBaseObject, IMetadataObject, ISubordinateObject
	{
		internal const string memberNameColumn = "MEMBER_NAME";

		internal const string uniqueNameColumn = "MEMBER_UNIQUE_NAME";

		internal const string MemberUNameRest = "MEMBER_UNIQUE_NAME";

		private const int memberUnknown = 0;

		private const int memberRegular = 1;

		private const int memberAll = 2;

		private const int memberMeasure = 3;

		private const int memberFormula = 4;

		private const int drilledDownBitMask = 65536;

		private const int parentAsPrevBitMask = 131072;

		private const int childCountBitMask = 65535;

		internal const string SchemaName = "MDSCHEMA_MEMBERS";

		private const string treeOpRest = "TREE_OP";

		private const int treeOpParent = 4;

		private static string captionColumn = "MEMBER_CAPTION";

		private static string descriptionColumn = "DESCRIPTION";

		private static string levelDepthColumn = "LEVEL_NUMBER";

		private static string typeColumn = "MEMBER_TYPE";

		private static string childCountColumn = "CHILDREN_CARDINALITY";

		private static string levelUNameColumn = LevelCollectionInternal.levelUNameRest;

		private BaseObjectData baseData;

		private MemberPropertyCollection memberProperties;

		private Member parent;

		private Tuple parentTuple;

		private int memberOrdinal = -1;

		private Level parentLevel;

		private PropertyCollection propertyCollection;

		private MemberOrigin memberOrigin;

		private int hashCode;

		private bool hashCodeCalculated;

		public string Name
		{
			get
			{
				DataRow dataRow;
				string propertyName;
				if (this.baseData.IsMetadata)
				{
					dataRow = (DataRow)this.baseData.MetadataData;
					propertyName = ((this.memberOrigin == MemberOrigin.UserQuery) ? "MEMBER_UNIQUE_NAME" : "MEMBER_NAME");
				}
				else
				{
					dataRow = (DataRow)this.baseData.AxisData;
					propertyName = ((this.memberOrigin == MemberOrigin.UserQuery) ? "UName" : "MEMBER_NAME");
				}
				return AdomdUtils.GetProperty(dataRow, propertyName).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return ((IAdomdBaseObject)this).InternalUniqueName;
			}
		}

		public string LevelName
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return AdomdUtils.GetProperty(dataRow, Member.levelUNameColumn).ToString();
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("LName"))
				{
					return AdomdUtils.GetProperty(dataRow2, "LName").ToString();
				}
				throw new NotSupportedException(SR.Member_MissingLevelName);
			}
		}

		public int LevelDepth
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return Convert.ToInt32(AdomdUtils.GetProperty(dataRow, Member.levelDepthColumn), CultureInfo.InvariantCulture);
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("LNum"))
				{
					return Convert.ToInt32(AdomdUtils.GetProperty(dataRow2, "LNum"), CultureInfo.InvariantCulture);
				}
				throw new NotSupportedException(SR.Member_MissingLevelDepth);
			}
		}

		public Level ParentLevel
		{
			get
			{
				if (null == this.parentLevel)
				{
					this.PopulateParentLevel();
				}
				return this.parentLevel;
			}
		}

		public string Caption
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return AdomdUtils.GetProperty(dataRow, Member.captionColumn).ToString();
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("Caption"))
				{
					return AdomdUtils.GetProperty(dataRow2, "Caption").ToString();
				}
				return string.Empty;
			}
		}

		public string Description
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return AdomdUtils.GetProperty(dataRow, Member.descriptionColumn).ToString();
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("Description"))
				{
					return AdomdUtils.GetProperty(dataRow2, "Description").ToString();
				}
				return string.Empty;
			}
		}

		public Member Parent
		{
			get
			{
				if (null == this.parent)
				{
					if (this.baseData.CubeName == null)
					{
						throw new NotSupportedException(SR.NotSupportedByProvider);
					}
					if (this.Connection == null)
					{
						throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
					}
					AdomdUtils.CheckConnectionOpened(this.Connection);
					ListDictionary listDictionary = new ListDictionary();
					listDictionary.Add(CubeCollectionInternal.cubeNameRest, this.baseData.CubeName);
					AdomdUtils.AddCubeSourceRestrictionIfApplicable(this.Connection, listDictionary);
					listDictionary.Add("MEMBER_UNIQUE_NAME", this.UniqueName);
					listDictionary.Add("TREE_OP", 4);
					DataRowCollection rows = AdomdUtils.GetRows(this.Connection, "MDSCHEMA_MEMBERS", listDictionary);
					if (rows.Count > 0)
					{
						this.parent = new Member(this.Connection, rows[0], null, null, MemberOrigin.Metadata, this.baseData.CubeName, null, -1, this.baseData.Catalog, this.baseData.SessionID);
					}
				}
				return this.parent;
			}
		}

		public MemberPropertyCollection MemberProperties
		{
			get
			{
				if (this.memberProperties == null)
				{
					this.memberProperties = new MemberPropertyCollection((DataRow)this.baseData.AxisData, this, this.GetInternallyAddedDimensionPropertyCount());
				}
				return this.memberProperties;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertyCollection == null)
				{
					this.propertyCollection = new PropertyCollection((DataRow)this.baseData.MetadataData, this, 0);
				}
				return this.propertyCollection;
			}
		}

		public long ChildCount
		{
			get
			{
				if (this.baseData.AxisData == null)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return (long)Convert.ToInt32(AdomdUtils.GetProperty(dataRow, Member.childCountColumn), CultureInfo.InvariantCulture);
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("DisplayInfo"))
				{
					return (long)(Convert.ToInt32(AdomdUtils.GetProperty(dataRow2, "DisplayInfo"), CultureInfo.InvariantCulture) & 65535);
				}
				throw new NotSupportedException(SR.Member_MissingDisplayInfo);
			}
		}

		public MemberTypeEnum Type
		{
			get
			{
				int num;
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					num = Convert.ToInt32(AdomdUtils.GetProperty(dataRow, Member.typeColumn), CultureInfo.InvariantCulture);
				}
				else
				{
					DataRow dataRow = (DataRow)this.baseData.AxisData;
					if (dataRow.Table.Columns.Contains(Member.typeColumn))
					{
						num = Convert.ToInt32(AdomdUtils.GetProperty(dataRow, Member.typeColumn), CultureInfo.InvariantCulture);
					}
					else
					{
						Hashtable orCreateNamesHashtable = MemberPropertyCollection.GetOrCreateNamesHashtable(dataRow.Table, this.GetInternallyAddedDimensionPropertyCount());
						if (!(orCreateNamesHashtable[Member.typeColumn] is int))
						{
							throw new InvalidOperationException(SR.InvalidOperationPriorToFetchAllProperties);
						}
						num = Convert.ToInt32(AdomdUtils.GetProperty(dataRow, (int)orCreateNamesHashtable[Member.typeColumn]), CultureInfo.InvariantCulture);
					}
				}
				switch (num)
				{
				case 0:
					return MemberTypeEnum.Unknown;
				case 1:
					return MemberTypeEnum.Regular;
				case 2:
					return MemberTypeEnum.All;
				case 3:
					return MemberTypeEnum.Measure;
				case 4:
					return MemberTypeEnum.Formula;
				default:
					return MemberTypeEnum.Unknown;
				}
			}
		}

		public bool DrilledDown
		{
			get
			{
				if (this.baseData.AxisData == null || this.memberOrigin != MemberOrigin.UserQuery)
				{
					throw new NotSupportedException(SR.NotSupportedOnNonCellsetMember);
				}
				DataRow dataRow = (DataRow)this.baseData.AxisData;
				if (dataRow.Table.Columns.Contains("DisplayInfo"))
				{
					return 0 != (Convert.ToInt32(AdomdUtils.GetProperty(dataRow, "DisplayInfo"), CultureInfo.InvariantCulture) & 65536);
				}
				throw new NotSupportedException(SR.Member_MissingDisplayInfo);
			}
		}

		public bool ParentSameAsPrevious
		{
			get
			{
				if (this.baseData.AxisData == null || this.memberOrigin != MemberOrigin.UserQuery)
				{
					throw new NotSupportedException(SR.NotSupportedOnNonCellsetMember);
				}
				DataRow dataRow = (DataRow)this.baseData.AxisData;
				if (dataRow.Table.Columns.Contains("DisplayInfo"))
				{
					return 0 != (Convert.ToInt32(AdomdUtils.GetProperty(dataRow, "DisplayInfo"), CultureInfo.InvariantCulture) & 131072);
				}
				throw new NotSupportedException(SR.Member_MissingDisplayInfo);
			}
		}

		private string ParentLevelUName
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return AdomdUtils.GetProperty(dataRow, Member.levelUNameColumn).ToString();
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("LName"))
				{
					return AdomdUtils.GetProperty(dataRow2, "LName").ToString();
				}
				throw new NotSupportedException(SR.Property_UnknownProperty("LName"));
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
					return this.parentLevel.ParentHierarchy.ParentDimension.ParentCube.Name;
				}
				return this.baseData.CubeName;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeMember;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				if (this.baseData.IsMetadata)
				{
					DataRow dataRow = (DataRow)this.baseData.MetadataData;
					return AdomdUtils.GetProperty(dataRow, "MEMBER_UNIQUE_NAME").ToString();
				}
				DataRow dataRow2 = (DataRow)this.baseData.AxisData;
				if (dataRow2.Table.Columns.Contains("UName"))
				{
					return AdomdUtils.GetProperty(dataRow2, "UName").ToString();
				}
				throw new NotSupportedException(SR.Property_UnknownProperty("UName"));
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
					return this.baseData.CubeName;
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
				return typeof(Member);
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
				return this.parentTuple;
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
				return this.memberOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(Member);
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
				return this.memberOrigin == MemberOrigin.UserQuery;
			}
		}

		internal Member(AdomdConnection connection, DataRow memberRow, Level parentLevel, Member parentMember, MemberOrigin memberOrigin, string cubeName, Tuple parentTuple, int memberOrdinal, string catalog, string sessionId)
		{
			bool flag = memberOrigin == MemberOrigin.Metadata;
			this.baseData = new BaseObjectData(connection, flag, flag ? null : memberRow, flag ? memberRow : null, parentLevel, cubeName, catalog, sessionId);
			this.parentLevel = parentLevel;
			this.parent = parentMember;
			this.memberProperties = null;
			this.parentTuple = parentTuple;
			this.memberOrdinal = memberOrdinal;
			this.memberOrigin = memberOrigin;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public MemberCollection GetChildren()
		{
			return this.InternalGetChildren(0L, -1L, new string[0], new MemberFilter[0]);
		}

		public MemberCollection GetChildren(long start, long count)
		{
			return this.GetChildren(start, count, new MemberFilter[0]);
		}

		public MemberCollection GetChildren(long start, long count, params MemberFilter[] filters)
		{
			return this.GetChildren(start, count, new string[0], filters);
		}

		public MemberCollection GetChildren(long start, long count, string[] properties, params MemberFilter[] filters)
		{
			if (start < 0L)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (count < 0L)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return this.InternalGetChildren(start, count, properties, filters);
		}

		public void FetchAllProperties()
		{
			if (!this.baseData.IsMetadata)
			{
				AdomdUtils.PopulateSymetry(this);
				this.propertyCollection = null;
			}
		}

		internal void PopulateParentLevel()
		{
			if (this.Connection == null)
			{
				throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
			}
			AdomdUtils.CheckConnectionOpened(this.Connection);
			string parentLevelUName = this.ParentLevelUName;
			this.parentLevel = (this.Connection.GetObjectData(SchemaObjectType.ObjectTypeLevel, this.baseData.CubeName, parentLevelUName) as Level);
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				if (!this.IsFromCellSet)
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

		public static bool operator ==(Member o1, Member o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Member o1, Member o2)
		{
			return !(o1 == o2);
		}

		private MemberCollection InternalGetChildren(long start, long count, string[] properties, params MemberFilter[] filters)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			Hierarchy parentHierarchy = this.ParentLevel.ParentHierarchy;
			CubeDef parentCube = parentHierarchy.ParentDimension.ParentCube;
			string baseSetMemberChilden = MemberQueryGenerator.GetBaseSetMemberChilden(this);
			string filteredAndRangedMemberSet = MemberQueryGenerator.GetFilteredAndRangedMemberSet(baseSetMemberChilden, parentHierarchy.UniqueName, start, count, filters);
			return parentCube.GetMembers(filteredAndRangedMemberSet, properties, null, this);
		}

		private int GetInternallyAddedDimensionPropertyCount()
		{
			if (this.memberOrigin != MemberOrigin.InternalMemberQuery)
			{
				return 0;
			}
			return 2;
		}
	}
}
