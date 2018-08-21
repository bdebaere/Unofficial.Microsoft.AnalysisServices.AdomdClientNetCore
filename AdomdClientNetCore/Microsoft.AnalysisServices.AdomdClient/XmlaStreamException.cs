using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaStreamException : IOException
	{
		private ConnectionExceptionCause connectionExceptionCause;

		internal ConnectionExceptionCause ConnectionExceptionCause
		{
			get
			{
				return this.connectionExceptionCause;
			}
		}

		internal XmlaStreamException(string message) : base(message)
		{
		}

		internal XmlaStreamException(string message, Exception innerException) : base(message, innerException)
		{
		}

		internal XmlaStreamException(Exception innerException) : base(string.Empty, innerException)
		{
		}

		internal XmlaStreamException(Exception innerException, ConnectionExceptionCause connectionExceptionCause) : this(innerException)
		{
			this.connectionExceptionCause = connectionExceptionCause;
		}
	}
}
