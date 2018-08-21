using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaError : XmlaMessage
	{
		internal const int INVALID_SESSION_ERROR = -1056178166;

		private int m_errorCode;

		private string m_callStack;

		public int ErrorCode
		{
			get
			{
				return this.m_errorCode;
			}
		}

		internal bool IsInvalidSession
		{
			get
			{
				return this.m_errorCode == -1056178166;
			}
		}

		public string CallStack
		{
			get
			{
				return this.m_callStack;
			}
		}

		internal XmlaError(int errorCode, string description, string source, string helpFile, XmlaMessageLocation location, string callStack) : base(description, source, helpFile, location)
		{
			this.m_errorCode = errorCode;
			this.m_callStack = callStack;
		}

		internal XmlaError(int errorCode, string description, string source, string helpFile, XmlaMessageLocation location) : this(errorCode, description, source, helpFile, location, null)
		{
			this.m_errorCode = errorCode;
		}
	}
}
