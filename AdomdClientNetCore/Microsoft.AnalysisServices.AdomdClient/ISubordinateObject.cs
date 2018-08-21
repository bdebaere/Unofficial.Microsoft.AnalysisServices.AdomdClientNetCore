using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface ISubordinateObject
	{
		object Parent
		{
			get;
		}

		int Ordinal
		{
			get;
		}

		Type Type
		{
			get;
		}
	}
}
