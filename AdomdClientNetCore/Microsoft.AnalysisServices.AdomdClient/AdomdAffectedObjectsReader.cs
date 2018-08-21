using System;
using System.Data;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdAffectedObjectsReader : AdomdDataReader
	{
		public int BaseVersion
		{
			get;
			private set;
		}

		public int CurrentVersion
		{
			get;
			private set;
		}

		public string Database
		{
			get;
			private set;
		}

		internal AdomdAffectedObjectsReader(XmlReader xmlReader, CommandBehavior commandBehavior, AdomdConnection connection) : base(xmlReader, commandBehavior, connection, true)
		{
			this.GetAttributes();
		}

		private void GetAttributes()
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			if (base.XmlaDataReader.TopLevelAttributes != null)
			{
				base.XmlaDataReader.TopLevelAttributes.TryGetValue("BaseVersion", out text);
				base.XmlaDataReader.TopLevelAttributes.TryGetValue("CurrentVersion", out text2);
				base.XmlaDataReader.TopLevelAttributes.TryGetValue("name", out text3);
			}
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2))
			{
				if (string.IsNullOrEmpty(text3))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Missing name attribute");
				}
				int baseVersion;
				if (string.IsNullOrEmpty(text) || !int.TryParse(text, out baseVersion))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Invalid or missing BaseVersion attribute");
				}
				int currentVersion;
				if (string.IsNullOrEmpty(text2) || !int.TryParse(text2, out currentVersion))
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Invalid or missing CurrentVersion attribute");
				}
				this.Database = text3;
				this.BaseVersion = baseVersion;
				this.CurrentVersion = currentVersion;
			}
		}
	}
}
