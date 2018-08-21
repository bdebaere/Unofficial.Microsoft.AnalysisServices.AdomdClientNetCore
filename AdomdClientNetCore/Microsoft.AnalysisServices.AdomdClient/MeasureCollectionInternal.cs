using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MeasureCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_MEASURES";

		private CubeDef parentCube;

		public Measure this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return MeasureCollectionInternal.GetMeasureByRow(base.Connection, row, this.parentCube, base.Catalog, base.SessionId);
			}
		}

		public Measure this[string index]
		{
			get
			{
				Measure measure = this.Find(index);
				if (null == measure)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return measure;
			}
		}

		internal MeasureCollectionInternal(AdomdConnection connection, CubeDef parentCube) : base(connection, InternalObjectType.InternalTypeMeasure, parentCube.metadataCache)
		{
			this.parentCube = parentCube;
		}

		public Measure Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, Measure.measureNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return MeasureCollectionInternal.GetMeasureByRow(base.Connection, dataRow, this.parentCube, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new MeasuresEnumerator(this);
		}

		internal static Measure GetMeasureByRow(AdomdConnection connection, DataRow row, CubeDef parentCube, string catalog, string sessionId)
		{
			Measure measure;
			if (row[0] is DBNull)
			{
				measure = new Measure(connection, row, parentCube, catalog, sessionId);
				row[0] = measure;
			}
			else
			{
				measure = (Measure)row[0];
			}
			return measure;
		}
	}
}
