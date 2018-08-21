using System;
using System.Runtime.Serialization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public class AdomdException : Exception
	{
		internal AdomdException()
		{
		}

		protected AdomdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal AdomdException(string message) : base(message)
		{
		}

		internal AdomdException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
