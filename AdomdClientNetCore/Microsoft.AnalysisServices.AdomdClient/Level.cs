using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class Level : IAdomdBaseObject, IMetadataObject
	{
		private BaseObjectData baseData;

		private Hierarchy parentHierarchy;

		private LevelPropertyCollection levelProperties;

		private PropertyCollection propertiesCollection;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string levelNameColumn = "LEVEL_NAME";

		private static string descriptionColumn = "DESCRIPTION";

		internal static string uniqueNameColumn = LevelCollectionInternal.levelUNameRest;

		private static string typeColumn = "LEVEL_TYPE";

		private static string captionColumn = "LEVEL_CAPTION";

		private static string levelNumberColumn = "LEVEL_NUMBER";

		private static string memberCountColumn = "LEVEL_CARDINALITY";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.LevelRow, Level.levelNameColumn).ToString();
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
				return AdomdUtils.GetProperty(this.LevelRow, Level.descriptionColumn).ToString();
			}
		}

		public Hierarchy ParentHierarchy
		{
			get
			{
				return this.parentHierarchy;
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.LevelRow, Level.captionColumn).ToString();
			}
		}

		public long MemberCount
		{
			get
			{
				return (long)Convert.ToInt32(AdomdUtils.GetProperty(this.LevelRow, Level.memberCountColumn), CultureInfo.InvariantCulture);
			}
		}

		public int LevelNumber
		{
			get
			{
				return (int)Convert.ToInt16(AdomdUtils.GetProperty(this.LevelRow, Level.levelNumberColumn), CultureInfo.InvariantCulture);
			}
		}

		public LevelTypeEnum LevelType
		{
			get
			{
				long num = (long)Convert.ToInt32(AdomdUtils.GetProperty(this.LevelRow, Level.typeColumn), CultureInfo.InvariantCulture);
				if (num >= 0L && num <= 8200L)
				{
					return (LevelTypeEnum)num;
				}
				return LevelTypeEnum.Regular;
			}
		}

		public LevelPropertyCollection LevelProperties
		{
			get
			{
				return this.levelProperties;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.LevelRow, this);
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
				return ((IAdomdBaseObject)this.parentHierarchy).CubeName;
			}
		}

		SchemaObjectType IAdomdBaseObject.SchemaObjectType
		{
			get
			{
				return SchemaObjectType.ObjectTypeLevel;
			}
		}

		string IAdomdBaseObject.InternalUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.LevelRow, Level.uniqueNameColumn).ToString();
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.Connection;
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
				return ((IAdomdBaseObject)this.parentHierarchy).CubeName;
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
				return typeof(Level);
			}
		}

		internal DataRow LevelRow
		{
			get
			{
				return (DataRow)this.baseData.MetadataData;
			}
		}

		private AdomdConnection Connection
		{
			get
			{
				return this.baseData.Connection;
			}
		}

		internal Level(AdomdConnection connection, DataRow levelRow, Hierarchy hierarchy, string catalog, string sessionId)
		{
			this.baseData = new BaseObjectData(connection, true, null, levelRow, hierarchy, null, catalog, sessionId);
			this.parentHierarchy = hierarchy;
			this.levelProperties = new LevelPropertyCollection(connection, this);
		}

		public override string ToString()
		{
			return this.Name;
		}

		public MemberCollection GetMembers()
		{
			return this.InternalGetMembers(0L, -1L, new string[0], new MemberFilter[0]);
		}

		public MemberCollection GetMembers(long start, long count)
		{
			return this.GetMembers(start, count, new MemberFilter[0]);
		}

		public MemberCollection GetMembers(long start, long count, params MemberFilter[] filters)
		{
			return this.GetMembers(start, count, new string[0], filters);
		}

		public MemberCollection GetMembers(long start, long count, string[] properties, params MemberFilter[] filters)
		{
			if (start < 0L)
			{
				throw new ArgumentOutOfRangeException("start");
			}
			if (count < 0L)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			return this.InternalGetMembers(start, count, properties, filters);
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

		public static bool operator ==(Level o1, Level o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(Level o1, Level o2)
		{
			return !(o1 == o2);
		}

		private MemberCollection InternalGetMembers(long start, long count, string[] properties, params MemberFilter[] filters)
		{
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			if (filters == null)
			{
				throw new ArgumentNullException("filters");
			}
			CubeDef parentCube = this.ParentHierarchy.ParentDimension.ParentCube;
			string baseSetLevelMembers = MemberQueryGenerator.GetBaseSetLevelMembers(this);
			string filteredAndRangedMemberSet = MemberQueryGenerator.GetFilteredAndRangedMemberSet(baseSetLevelMembers, this.ParentHierarchy.UniqueName, start, count, filters);
			return parentCube.GetMembers(filteredAndRangedMemberSet, properties, this, null);
		}
	}
}
