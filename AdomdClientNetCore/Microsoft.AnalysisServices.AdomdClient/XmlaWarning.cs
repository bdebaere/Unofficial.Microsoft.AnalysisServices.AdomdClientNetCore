using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaWarning : XmlaMessage
	{
		private int m_warningCode;

		internal XmlaWarning(int warningCode, string description, string source, string helpFile, XmlaMessageLocation location) : base(description, source, helpFile, location)
		{
			this.m_warningCode = warningCode;
		}
	}
}
