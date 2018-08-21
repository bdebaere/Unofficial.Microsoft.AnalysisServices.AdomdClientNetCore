using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class LocalServerStream : ClearTextXmlaStream
	{
		public override void Close()
		{
			LocalServerNativeMethods.MSMDLocalStreamClose();
		}

		public override void Dispose()
		{
			try
			{
				LocalServerNativeMethods.MSMDLocalStreamCloseBase();
				this.disposed = true;
			}
			finally
			{
				base.Dispose(true);
			}
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			LocalServerNativeMethods.MSMDLocalStreamWriteEndOfMessage();
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			return LocalServerNativeMethods.MSMDLocalStreamRead(buffer, offset, size);
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			LocalServerNativeMethods.MSMDLocalStreamWrite(buffer, offset, size);
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			LocalServerNativeMethods.MSMDLocalStreamSkip();
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			LocalServerNativeMethods.MSMDLocalStreamFlush();
		}
	}
}
