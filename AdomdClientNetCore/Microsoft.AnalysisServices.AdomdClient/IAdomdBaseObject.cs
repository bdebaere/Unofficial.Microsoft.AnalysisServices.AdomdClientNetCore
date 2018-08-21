using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IAdomdBaseObject
	{
		AdomdConnection Connection
		{
			get;
		}

		bool IsMetadata
		{
			get;
			set;
		}

		object MetadataData
		{
			get;
			set;
		}

		IAdomdBaseObject ParentObject
		{
			get;
			set;
		}

		string InternalUniqueName
		{
			get;
		}

		string CubeName
		{
			get;
		}

		SchemaObjectType SchemaObjectType
		{
			get;
		}
	}
}
