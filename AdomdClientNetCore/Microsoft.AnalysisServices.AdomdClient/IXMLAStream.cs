using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class IXMLAStream : ClearTextXmlaStream
	{
		private const int bufferForSkipSize = 8192;

		private const int memoryStreamInitialSize = 4096;

		private static byte[] bufferForSkip = new byte[8192];

		private MemoryStream writeStream;

		private Stream readStream;

		private XASC iXmlaComClass;

		public IXMLAStream()
		{
			try
			{
				//this.iXmlaComClass = new XASC();
			}
			catch (COMException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Close()
		{
		}

		public override void Dispose()
		{
			try
			{
				try
				{
					if (this.writeStream != null)
					{
						this.writeStream.Close();
						this.writeStream = null;
					}
					if (this.readStream != null)
					{
						this.readStream.Close();
						this.readStream = null;
					}
				}
				catch (XmlaStreamException)
				{
				}
				catch (COMException)
				{
				}
				catch (IOException)
				{
				}
				this.disposed = true;
			}
			finally
			{
				base.Dispose(true);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			int result = 0;
			try
			{
				result = this.readStream.Read(buffer, offset, size);
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (COMException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (IOException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			return result;
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.writeStream == null)
				{
					this.writeStream = new MemoryStream(4096);
				}
				this.writeStream.Write(buffer, offset, size);
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (COMException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (IOException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.writeStream != null)
				{
					this.writeStream.Position = 0L;
					this.readStream = StreamInteropHelper.ProcessRequest(this.iXmlaComClass, this.writeStream);
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (COMException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (IOException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
		}

		public override void WriteSoapActionHeader(string action)
		{
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.readStream != null)
				{
					while (this.readStream.Read(IXMLAStream.bufferForSkip, 0, 8192) > 0)
					{
					}
					this.readStream.Close();
					this.readStream = null;
				}
				if (this.writeStream != null)
				{
					this.writeStream.Close();
					this.writeStream = null;
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (COMException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (IOException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
		}
	}
}
