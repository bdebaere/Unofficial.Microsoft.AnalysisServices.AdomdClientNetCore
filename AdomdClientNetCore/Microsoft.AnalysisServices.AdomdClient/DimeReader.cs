using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class DimeReader
	{
		private Stream m_stream;

		private DimeRecord m_currentRecord;

		private TransportCapabilities m_Options;

		private bool m_closed;

		public TransportCapabilities Options
		{
			get
			{
				return this.m_Options;
			}
		}

		public DimeReader(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException(XmlaSR.DimeReader_CannotReadFromStream);
			}
			this.m_stream = stream;
			this.m_Options = null;
		}

		public DimeRecord ReadRecord()
		{
			if (this.m_closed)
			{
				throw new InvalidOperationException(XmlaSR.DimeReader_IsClosed);
			}
			if (this.m_currentRecord != null)
			{
				if (this.m_currentRecord.EndOfMessage)
				{
					return null;
				}
				this.m_currentRecord.Close();
			}
			this.m_currentRecord = new DimeRecord(this.m_stream);
			if (this.m_Options == null)
			{
				this.m_Options = this.m_currentRecord.Options;
			}
			if (this.m_currentRecord.TypeFormat == TypeFormatEnum.None && this.m_currentRecord.EndOfMessage)
			{
				this.m_currentRecord.Close();
				return null;
			}
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
				if (this.m_currentRecord.CanRead)
				{
					throw new InvalidOperationException(XmlaSR.DimeReader_PreviousRecordStreamStillOpened);
				}
				while (!this.m_currentRecord.EndOfMessage)
				{
					if (this.ReadRecord() != null)
					{
						this.m_currentRecord.Close(false);
					}
				}
				this.m_currentRecord.Close();
			}
			this.m_closed = true;
		}
	}
}
