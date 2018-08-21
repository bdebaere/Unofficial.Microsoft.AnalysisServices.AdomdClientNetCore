using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal interface ICommandContentProvider
	{
		string CommandText
		{
			get;
		}

		Stream CommandStream
		{
			get;
		}

		bool IsContentMdx
		{
			get;
		}
	}
}
