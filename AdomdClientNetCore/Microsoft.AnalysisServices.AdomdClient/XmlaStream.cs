using System;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class XmlaStream : Stream
	{
		private class Lock
		{
		}

		protected bool disposed;

		private string sessionID = string.Empty;

		private bool isCompressionEnabled;

		private bool isSessionTokenNeeded;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual Guid ActivityID
		{
			get;
			set;
		}

		public virtual Guid RequestID
		{
			get;
			set;
		}

		public virtual Guid CurrentActivityID
		{
			get;
			set;
		}

		public virtual string SessionID
		{
			get
			{
				return this.sessionID;
			}
			set
			{
				this.sessionID = value;
			}
		}

		public virtual bool IsSessionTokenNeeded
		{
			get
			{
				return this.isSessionTokenNeeded;
			}
			set
			{
				this.isSessionTokenNeeded = value;
			}
		}

		public virtual bool IsCompressionEnabled
		{
			get
			{
				return this.isCompressionEnabled;
			}
			set
			{
				this.isCompressionEnabled = value;
			}
		}

		public new virtual void Dispose()
		{
			base.Dispose();
		}

		public abstract void WriteEndOfMessage();

		public abstract void Skip();

		public abstract DataType GetResponseDataType();

		public abstract DataType GetRequestDataType();

		public virtual void WriteSoapActionHeader(string action)
		{
		}

		public virtual string GetExtendedErrorInfo()
		{
			return string.Empty;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}
	}
}
