using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdErrorResponseException : AdomdException
	{
		private const string errorsSerializeName = "Errors";

		private string completeErrorMessage;

		private AdomdErrorCollection errors;

		public AdomdErrorCollection Errors
		{
			get
			{
				if (this.errors == null)
				{
					this.errors = new AdomdErrorCollection();
				}
				return this.errors;
			}
		}

		public int ErrorCode
		{
			get
			{
				if (this.Errors.Count > 0)
				{
					return this.Errors[this.Errors.Count - 1].ErrorCode;
				}
				return 0;
			}
		}

		public override string Source
		{
			get
			{
				if (this.Errors.Count > 0)
				{
					return this.Errors[this.Errors.Count - 1].Source;
				}
				return string.Empty;
			}
		}

		public override string Message
		{
			get
			{
				if (this.completeErrorMessage == null)
				{
					if (this.Errors.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						for (int i = this.Errors.Count - 1; i >= 1; i--)
						{
							stringBuilder.Append(this.Errors[i].Message);
							stringBuilder.Append(Environment.NewLine);
						}
						stringBuilder.Append(this.Errors[0].Message);
						this.completeErrorMessage = stringBuilder.ToString();
					}
					else
					{
						this.completeErrorMessage = XmlaSR.ServerDidNotProvideErrorInfo;
					}
				}
				return this.completeErrorMessage;
			}
		}

		public override string HelpLink
		{
			get
			{
				if (this.Errors.Count > 0)
				{
					return this.Errors[this.Errors.Count - 1].HelpLink;
				}
				return string.Empty;
			}
		}

		internal AdomdErrorResponseException(XmlaException innerException)
		{
			this.TranslateException(innerException);
		}

		internal AdomdErrorResponseException(XmlaError innerError)
		{
			this.TranslateException(innerError);
		}

		internal AdomdErrorResponseException()
		{
		}

		internal AdomdErrorResponseException(string message) : base(message)
		{
			this.Errors.Add(new AdomdError(0, "adomd.net", message, string.Empty));
		}

		internal AdomdErrorResponseException(string message, Exception innerException) : base(message, innerException)
		{
			this.Errors.Add(new AdomdError(0, "adomd.net", message, string.Empty));
		}

		private AdomdErrorResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errors = (AdomdErrorCollection)info.GetValue("Errors", typeof(AdomdErrorCollection));
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Errors", this.errors, typeof(AdomdErrorCollection));
			base.GetObjectData(info, context);
		}

		private void TranslateException(XmlaException innerException)
		{
			foreach (XmlaResult xmlaResult in ((IEnumerable)innerException.Results))
			{
				foreach (XmlaMessage xmlaMessage in ((IEnumerable)xmlaResult.Messages))
				{
					if (xmlaMessage is XmlaError)
					{
						AdomdError error = new AdomdError((XmlaError)xmlaMessage);
						if (this.errors == null)
						{
							this.errors = new AdomdErrorCollection();
						}
						this.errors.Add(error);
					}
				}
			}
		}

		private void TranslateException(XmlaError innerError)
		{
			AdomdError error = new AdomdError(innerError);
			if (this.errors == null)
			{
				this.errors = new AdomdErrorCollection();
			}
			this.errors.Add(error);
		}
	}
}
