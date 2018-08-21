using System;
using System.Collections;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaException : Exception
	{
		private XmlaResultCollection m_results;

		private string message;

		public XmlaResultCollection Results
		{
			get
			{
				return this.m_results;
			}
		}

		public override string Message
		{
			get
			{
				if (this.message == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (XmlaResult xmlaResult in ((IEnumerable)this.m_results))
					{
						foreach (XmlaMessage xmlaMessage in ((IEnumerable)xmlaResult.Messages))
						{
							stringBuilder.AppendLine(xmlaMessage.Description);
						}
					}
					this.message = stringBuilder.ToString();
				}
				return this.message;
			}
		}

		internal XmlaException(XmlaResultCollection results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_results = results;
		}

		internal XmlaException(XmlaResult result)
		{
			this.m_results = new XmlaResultCollection();
			this.m_results.Add(result);
		}
	}
}
