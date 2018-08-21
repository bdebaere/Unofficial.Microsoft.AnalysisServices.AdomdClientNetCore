using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal struct MDXMLAProp
	{
		internal string strOleDbName;

		internal string strXmlAName;

		internal MDXMLAProp(string theOleDbName, string theXmlAName)
		{
			this.strOleDbName = theOleDbName;
			this.strXmlAName = theXmlAName;
		}
	}
}
