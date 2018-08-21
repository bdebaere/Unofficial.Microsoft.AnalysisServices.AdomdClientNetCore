using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaResult
	{
		private string m_value = string.Empty;

		private XmlaMessageCollection m_messages = new XmlaMessageCollection();

		internal bool ContainsErrors
		{
			get
			{
				int i = 0;
				int count = this.m_messages.Count;
				while (i < count)
				{
					if (this.m_messages[i] is XmlaError)
					{
						return true;
					}
					i++;
				}
				return false;
			}
		}

		internal bool ContainsInvalidSessionError
		{
			get
			{
				int i = 0;
				int count = this.m_messages.Count;
				while (i < count)
				{
					if (this.m_messages[i] is XmlaError && ((XmlaError)this.m_messages[i]).IsInvalidSession)
					{
						return true;
					}
					i++;
				}
				return false;
			}
		}

		public string Value
		{
			get
			{
				return this.m_value;
			}
		}

		public XmlaMessageCollection Messages
		{
			get
			{
				return this.m_messages;
			}
		}

		internal XmlaResult()
		{
		}

		internal XmlaResult(XmlaError error)
		{
			this.Messages.Add(error);
		}

		internal void SetValue(string value)
		{
			this.m_value = value;
		}
	}
}
