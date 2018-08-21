using System;
using System.Runtime.Serialization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public sealed class AdomdCacheExpiredException : AdomdException
	{
		internal AdomdCacheExpiredException()
		{
		}

		internal AdomdCacheExpiredException(string message) : base(message)
		{
		}

		internal AdomdCacheExpiredException(string message, Exception innerException) : base(message, innerException)
		{
		}

		private AdomdCacheExpiredException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
