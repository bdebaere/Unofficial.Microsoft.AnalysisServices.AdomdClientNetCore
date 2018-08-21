using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdError
	{
		private int errorCode;

		private string source;

		private string message;

		private string helpLink;

		private AdomdErrorLocation errorLocation;

		private string callStack;

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public string HelpLink
		{
			get
			{
				return this.helpLink;
			}
		}

		public AdomdErrorLocation Location
		{
			get
			{
				return this.errorLocation;
			}
		}

		public string CallStack
		{
			get
			{
				return this.callStack;
			}
		}

		internal AdomdError(int errorCode, string source, string message, string helpLink)
		{
			this.errorCode = errorCode;
			this.source = source;
			this.message = message;
			this.helpLink = helpLink;
			this.errorLocation = null;
		}

		internal AdomdError(int errorCode, string source, string message, string helpLink, XmlaMessageLocation location, string callStack) : this(errorCode, source, message, helpLink, location)
		{
			this.callStack = callStack;
		}

		internal AdomdError(int errorCode, string source, string message, string helpLink, XmlaMessageLocation location) : this(errorCode, source, message, helpLink)
		{
			if (location != null)
			{
				this.errorLocation = new AdomdErrorLocation(location);
			}
		}

		internal AdomdError(XmlaError error) : this(error.ErrorCode, error.Source, error.Description, error.HelpFile, error.Location, error.CallStack)
		{
		}

		public override string ToString()
		{
			return this.message;
		}
	}
}
