using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class ClearTextXmlaStream : XmlaStream
	{
		public override DataType GetResponseDataType()
		{
			return DataType.TextXml;
		}

		public override DataType GetRequestDataType()
		{
			return DataType.TextXml;
		}
	}
}
