using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface IXmlaProperty : IXmlaPropertyKey
	{
		object Value
		{
			get;
			set;
		}

		object Parent
		{
			get;
			set;
		}
	}
}
