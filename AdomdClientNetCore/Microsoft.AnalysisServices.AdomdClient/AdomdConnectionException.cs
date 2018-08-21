using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdConnectionException : AdomdException
	{
		private const string exceptionCauseSerializeName = "ExceptionCauseProperty";

		private ConnectionExceptionCause exceptionCause;

		public ConnectionExceptionCause ExceptionCause
		{
			get
			{
				return this.exceptionCause;
			}
		}

		internal AdomdConnectionException()
		{
		}

		private AdomdConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exceptionCause = (ConnectionExceptionCause)info.GetValue("ExceptionCauseProperty", typeof(ConnectionExceptionCause));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ExceptionCauseProperty", this.exceptionCause, typeof(ConnectionExceptionCause));
			base.GetObjectData(info, context);
		}

		internal AdomdConnectionException(string message) : base(message)
		{
		}

		internal AdomdConnectionException(string message, Exception innerException) : base(message, (innerException is XmlaStreamException && (innerException.Message == null || innerException.Message.Length == 0)) ? innerException.InnerException : innerException)
		{
			if (innerException is XmlaStreamException)
			{
				this.exceptionCause = ((XmlaStreamException)innerException).ConnectionExceptionCause;
			}
		}

		internal AdomdConnectionException(string message, Exception innerException, ConnectionExceptionCause exceptionCause) : this(message, innerException)
		{
			this.exceptionCause = exceptionCause;
		}
	}
}
