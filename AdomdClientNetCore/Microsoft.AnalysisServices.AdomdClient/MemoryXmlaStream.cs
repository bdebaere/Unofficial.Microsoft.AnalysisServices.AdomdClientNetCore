using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class MemoryXmlaStream : XmlaStream
	{
		private MemoryStream baseStream;

		private DataType streamDataType;

		public override long Length
		{
			get
			{
				return this.baseStream.Length;
			}
		}

		internal MemoryXmlaStream(DataType streamDataType)
		{
			this.baseStream = new MemoryStream();
			this.streamDataType = streamDataType;
		}

		~MemoryXmlaStream()
		{
			this.InternalDispose(false);
		}

		public override void WriteEndOfMessage()
		{
			throw new NotImplementedException();
		}

		public override void Skip()
		{
			throw new NotImplementedException();
		}

		public override DataType GetResponseDataType()
		{
			return this.streamDataType;
		}

		public override DataType GetRequestDataType()
		{
			return this.streamDataType;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.baseStream.Seek(offset, origin);
		}

		public override void Flush()
		{
			this.baseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.baseStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.baseStream.Write(buffer, offset, count);
		}

		public override void Dispose()
		{
			this.InternalDispose(true);
			GC.SuppressFinalize(this);
		}

		private void InternalDispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				try
				{
					if (this.baseStream != null)
					{
						this.baseStream.Dispose();
					}
				}
				catch
				{
				}
			}
			this.disposed = true;
		}
	}
}
