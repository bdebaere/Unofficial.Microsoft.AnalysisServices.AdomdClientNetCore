using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IMetadataCache
	{
		IObjectCache GetObjectCache(InternalObjectType objectType);

		void Populate(InternalObjectType objectType);

		void Refresh(InternalObjectType objectType);

		void CheckCacheIsValid();

		void MarkNeedCheckForValidness();

		void MarkAbandoned();

		DataRow FindObjectByUniqueName(SchemaObjectType objectType, string nameUnique);
	}
}
