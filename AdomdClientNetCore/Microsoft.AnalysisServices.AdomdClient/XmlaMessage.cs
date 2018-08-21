using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class XmlaMessage
	{
		private string m_description;

		private string m_source;

		private string m_helpFile;

		private XmlaMessageLocation location;

		public string Description
		{
			get
			{
				return this.m_description;
			}
		}

		public string Source
		{
			get
			{
				return this.m_source;
			}
		}

		public string HelpFile
		{
			get
			{
				return this.m_helpFile;
			}
		}

		public XmlaMessageLocation Location
		{
			get
			{
				return this.location;
			}
		}

		internal XmlaMessage(string description, string source, string helpFile, XmlaMessageLocation location)
		{
			this.m_description = description;
			this.m_source = source;
			this.m_helpFile = helpFile;
			this.location = location;
		}
	}
}
