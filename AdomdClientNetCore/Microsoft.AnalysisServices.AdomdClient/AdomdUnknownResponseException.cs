using System;
using System.Runtime.Serialization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdUnknownResponseException : AdomdException
	{
		internal AdomdUnknownResponseException(Exception e) : base(XmlaSR.UnknownServerResponseFormat, e)
		{
		}

		private AdomdUnknownResponseException()
		{
		}

		internal AdomdUnknownResponseException(string message, string debugMessage) : base(AdomdUnknownResponseException.GetExceptionMessage(message, debugMessage))
		{
		}

		internal AdomdUnknownResponseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		private AdomdUnknownResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string GetExceptionMessage(string message, string debugMessage)
		{
			return message;
		}
	}
}
