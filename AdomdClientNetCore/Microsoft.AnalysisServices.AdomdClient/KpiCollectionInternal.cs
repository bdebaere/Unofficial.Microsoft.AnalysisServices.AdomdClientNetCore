using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class KpiCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "MDSCHEMA_KPIS";

		private CubeDef parentCube;

		public Kpi this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return KpiCollectionInternal.GetKpiByRow(base.Connection, row, this.parentCube, base.Catalog, base.SessionId);
			}
		}

		public Kpi this[string index]
		{
			get
			{
				Kpi kpi = this.Find(index);
				if (null == kpi)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return kpi;
			}
		}

		internal KpiCollectionInternal(AdomdConnection connection, CubeDef parentCube) : base(connection, InternalObjectType.InternalTypeKpi, parentCube.metadataCache)
		{
			this.parentCube = parentCube;
		}

		public Kpi Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, Kpi.kpiNameColumn);
			if (dataRow == null)
			{
				return null;
			}
			return KpiCollectionInternal.GetKpiByRow(base.Connection, dataRow, this.parentCube, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new KpisEnumerator(this);
		}

		internal static Kpi GetKpiByRow(AdomdConnection connection, DataRow row, CubeDef parentCube, string catalog, string sessionId)
		{
			Kpi kpi;
			if (row[0] is DBNull)
			{
				kpi = new Kpi(connection, row, parentCube, catalog, sessionId);
				row[0] = kpi;
			}
			else
			{
				kpi = (Kpi)row[0];
			}
			return kpi;
		}
	}
}
