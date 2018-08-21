using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IMetadataObject
	{
		AdomdConnection Connection
		{
			get;
		}

		string SessionId
		{
			get;
		}

		string Catalog
		{
			get;
		}

		string CubeName
		{
			get;
		}

		string UniqueName
		{
			get;
		}

		Type Type
		{
			get;
		}
	}
}
