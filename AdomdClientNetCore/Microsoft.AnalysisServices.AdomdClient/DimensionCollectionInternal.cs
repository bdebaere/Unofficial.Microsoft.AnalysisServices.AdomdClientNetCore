using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class DimensionCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_DIMENSIONS";

		internal static string dimUNameRest = "DIMENSION_UNIQUE_NAME";

		private CubeDef parentCube;

		public Dimension this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return DimensionCollectionInternal.GetDimensionByRow(base.Connection, row, this.parentCube, base.Catalog, base.SessionId);
			}
		}

		public Dimension this[string index]
		{
			get
			{
				Dimension dimension = this.Find(index);
				if (null == dimension)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index));
				}
				return dimension;
			}
		}

		internal DimensionCollectionInternal(AdomdConnection connection, CubeDef parentCube) : base(connection, InternalObjectType.InternalTypeDimension, parentCube.metadataCache)
		{
			this.parentCube = parentCube;
		}

		public Dimension Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, Dimension.dimensionNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return DimensionCollectionInternal.GetDimensionByRow(base.Connection, dataRow, this.parentCube, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new DimensionsEnumerator(this);
		}

		internal static Dimension GetDimensionByRow(AdomdConnection connection, DataRow row, CubeDef parentCube, string catalog, string sessionId)
		{
			Dimension dimension;
			if (row[0] is DBNull)
			{
				dimension = new Dimension(connection, row, parentCube, catalog, sessionId);
				row[0] = dimension;
			}
			else
			{
				dimension = (Dimension)row[0];
			}
			return dimension;
		}
	}
}
