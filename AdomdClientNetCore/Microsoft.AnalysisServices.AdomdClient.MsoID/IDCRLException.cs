using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient.MsoID
{
	[Serializable]
	internal sealed class IDCRLException : Exception
	{
		internal int errorCode;

		internal string errorMsg;

		internal string customMessage = "IDCRL Exception";

		internal int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return this.errorMsg;
			}
		}

		internal IDCRLException(string message) : base(message)
		{
		}

		internal IDCRLException(int errorcode, string message, Exception innerException) : base(message, innerException)
		{
			this.SetException(errorcode);
		}

		internal IDCRLException(int errorcode)
		{
			this.SetException(errorcode);
		}

		private void SetException(int errorcode)
		{
			this.errorCode = errorcode;
			base.HResult = errorcode;
			StackTrace stackTrace = new StackTrace();
			string name = stackTrace.GetFrame(2).GetMethod().Name;
			string text = string.Concat(new string[]
			{
				this.customMessage,
				" In Method: ",
				name,
				" \r\nIDCRL ErrorCode : ",
				errorcode.ToString("X", CultureInfo.InvariantCulture)
			});
			this.errorMsg = text;
		}
	}
}
