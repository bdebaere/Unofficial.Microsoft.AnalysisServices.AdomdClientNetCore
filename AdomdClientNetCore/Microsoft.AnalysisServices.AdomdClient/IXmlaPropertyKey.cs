using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IXmlaPropertyKey
	{
		string Name
		{
			get;
			set;
		}

		string Namespace
		{
			get;
			set;
		}
	}
}
