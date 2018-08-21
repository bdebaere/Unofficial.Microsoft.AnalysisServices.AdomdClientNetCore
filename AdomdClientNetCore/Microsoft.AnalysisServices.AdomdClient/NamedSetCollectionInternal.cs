using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class NamedSetCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_SETS";

		private CubeDef parentCube;

		public NamedSet this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return NamedSetCollectionInternal.GetNamedSetByRow(base.Connection, row, this.parentCube, base.Catalog, base.SessionId);
			}
		}

		public NamedSet this[string index]
		{
			get
			{
				NamedSet namedSet = this.Find(index);
				if (null == namedSet)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return namedSet;
			}
		}

		internal NamedSetCollectionInternal(AdomdConnection connection, CubeDef parentCube) : base(connection, InternalObjectType.InternalTypeNamedSet, parentCube.metadataCache)
		{
			this.parentCube = parentCube;
		}

		public NamedSet Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, "SET_NAME");
			if (dataRow == null)
			{
				return null;
			}
			return NamedSetCollectionInternal.GetNamedSetByRow(base.Connection, dataRow, this.parentCube, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new NamedSetsEnumerator(this);
		}

		internal static NamedSet GetNamedSetByRow(AdomdConnection connection, DataRow row, CubeDef parentCube, string catalog, string sessionId)
		{
			NamedSet namedSet;
			if (row[0] is DBNull)
			{
				namedSet = new NamedSet(connection, row, parentCube, catalog, sessionId);
				row[0] = namedSet;
			}
			else
			{
				namedSet = (NamedSet)row[0];
			}
			return namedSet;
		}
	}
}
