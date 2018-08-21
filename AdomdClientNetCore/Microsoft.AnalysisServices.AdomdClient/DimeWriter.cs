using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class DimeWriter
	{
		private const int ChunkSizeDefault = 1024;

		private Stream m_stream;

		private DimeRecord m_currentRecord;

		private TransportCapabilities m_Options;

		private bool m_closed;

		private bool m_firstRecord;

		private int m_defaultChunkSize;

		internal int DefaultChunkSize
		{
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException(XmlaSR.DimeWriter_InvalidDefaultChunkSize);
				}
				this.m_defaultChunkSize = value;
			}
		}

		public TransportCapabilities Options
		{
			set
			{
				this.m_Options = value;
			}
		}

		public DimeWriter(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(XmlaSR.DimeWriter_CannotWriteToStream, "stream");
			}
			this.m_stream = stream;
			this.m_firstRecord = true;
			this.m_defaultChunkSize = 1024;
			this.m_Options = null;
		}

		public DimeRecord CreateRecord(Uri id, string type, TypeFormatEnum typeFormat, int contentLength)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(XmlaSR.DimeWriter_WriterIsClosed);
			}
			if (this.m_currentRecord != null)
			{
				this.m_currentRecord.Close(false);
			}
			this.m_currentRecord = new DimeRecord(this.m_stream, id, type, typeFormat, this.m_firstRecord, contentLength, this.m_defaultChunkSize);
			this.m_currentRecord.Options = this.m_Options;
			this.m_firstRecord = false;
			return this.m_currentRecord;
		}

		public void Close()
		{
			if (this.m_closed)
			{
				return;
			}
			if (this.m_currentRecord != null)
			{
				this.m_currentRecord.Close(true);
				this.m_currentRecord = null;
			}
			this.m_closed = true;
		}
	}
}
