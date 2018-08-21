using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class LevelPropertyCollectionInternal : CacheBasedFilteredCollection
	{
		internal const string schemaName = "MDSCHEMA_PROPERTIES";

		internal const string propertyType = "PROPERTY_TYPE";

		internal const int MDPROP_MEMBER = 1;

		private Level parentLevel;

		public LevelProperty this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return LevelPropertyCollectionInternal.GetLevelPropertyByRow(base.Connection, row, this.parentLevel, index);
			}
		}

		public LevelProperty this[string index]
		{
			get
			{
				LevelProperty levelProperty = this.Find(index);
				if (null == levelProperty)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return levelProperty;
			}
		}

		internal LevelPropertyCollectionInternal(AdomdConnection connection, Level parentLevel) : base(connection, InternalObjectType.InternalTypeLevelProperty, parentLevel.ParentHierarchy.ParentDimension.ParentCube.metadataCache)
		{
			this.parentLevel = parentLevel;
			base.Initialize((DataRow)((IAdomdBaseObject)parentLevel).MetadataData, null);
		}

		public LevelProperty Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, (DataRow)((IAdomdBaseObject)this.parentLevel).MetadataData, LevelProperty.levelPropNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			int num = 0;
			DataRow[] internalCollection = this.internalCollection;
			for (int i = 0; i < internalCollection.Length; i++)
			{
				DataRow dataRow2 = internalCollection[i];
				if (dataRow2 == dataRow)
				{
					break;
				}
				num++;
			}
			return LevelPropertyCollectionInternal.GetLevelPropertyByRow(base.Connection, dataRow, this.parentLevel, num);
		}

		public override IEnumerator GetEnumerator()
		{
			return new LevelPropsEnumerator(this);
		}

		internal static LevelProperty GetLevelPropertyByRow(AdomdConnection connection, DataRow row, Level parentLevel, int propOrdinal)
		{
			LevelProperty levelProperty;
			if (row[0] is DBNull)
			{
				levelProperty = new LevelProperty(connection, row, parentLevel, propOrdinal);
				row[0] = levelProperty;
			}
			else
			{
				levelProperty = (LevelProperty)row[0];
			}
			return levelProperty;
		}
	}
}
