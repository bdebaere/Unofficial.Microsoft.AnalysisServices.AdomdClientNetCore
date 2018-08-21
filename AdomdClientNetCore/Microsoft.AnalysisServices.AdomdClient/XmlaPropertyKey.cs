using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class XmlaPropertyKey : IXmlaPropertyKey
	{
		private string propertyName;

		private string propertyNamespace;

		public string Name
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.propertyNamespace;
			}
			set
			{
				this.propertyNamespace = value;
			}
		}

		internal XmlaPropertyKey(string propertyName, string propertyNamespace)
		{
			this.propertyName = propertyName;
			this.propertyNamespace = propertyNamespace;
		}
	}
}
