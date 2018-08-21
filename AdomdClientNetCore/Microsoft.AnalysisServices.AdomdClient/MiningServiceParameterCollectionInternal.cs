using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MiningServiceParameterCollectionInternal : CacheBasedNotFilteredCollection
	{
		internal static string schemaName = "DMSCHEMA_MINING_SERVICE_PARAMETERS";

		internal static string columnNameRest = "PARAMETER_NAME";

		internal static string serviceNameRest = "SERVICE_NAME";

		private MiningService parentService;

		public MiningServiceParameter this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				DataRow row = this.internalCollection[index];
				return MiningServiceParameterCollectionInternal.GetMiningServiceParameterByRow(base.Connection, row, this.parentService, base.Catalog, base.SessionId);
			}
		}

		public MiningServiceParameter this[string index]
		{
			get
			{
				MiningServiceParameter miningServiceParameter = this.Find(index);
				if (null == miningServiceParameter)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "index");
				}
				return miningServiceParameter;
			}
		}

		internal MiningServiceParameterCollectionInternal(AdomdConnection connection, MiningService parentService) : base(connection)
		{
			string name = parentService.Name;
			this.parentService = parentService;
			ListDictionary listDictionary = new ListDictionary();
			listDictionary.Add(MiningServiceParameterCollectionInternal.serviceNameRest, name);
			ObjectMetadataCache objectCache = new ObjectMetadataCache(connection, InternalObjectType.InternalTypeMiningServiceParameter, MiningServiceParameterCollectionInternal.schemaName, listDictionary);
			base.Initialize(objectCache);
		}

		public MiningServiceParameter Find(string index)
		{
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			DataRow dataRow = base.FindObjectByName(index, null, MiningServiceParameter.serviceParameterColumn);
			if (dataRow == null)
			{
				return null;
			}
			return MiningServiceParameterCollectionInternal.GetMiningServiceParameterByRow(base.Connection, dataRow, this.parentService, base.Catalog, base.SessionId);
		}

		public override IEnumerator GetEnumerator()
		{
			return new MiningServiceParametersEnumerator(this);
		}

		internal static MiningServiceParameter GetMiningServiceParameterByRow(AdomdConnection connection, DataRow row, IAdomdBaseObject parentObject, string catalog, string sessionId)
		{
			MiningServiceParameter miningServiceParameter;
			if (row[0] is DBNull)
			{
				miningServiceParameter = new MiningServiceParameter(connection, row, parentObject, catalog, sessionId);
				row[0] = miningServiceParameter;
			}
			else
			{
				miningServiceParameter = (MiningServiceParameter)row[0];
			}
			return miningServiceParameter;
		}

		internal override void CheckCache()
		{
		}
	}
}
