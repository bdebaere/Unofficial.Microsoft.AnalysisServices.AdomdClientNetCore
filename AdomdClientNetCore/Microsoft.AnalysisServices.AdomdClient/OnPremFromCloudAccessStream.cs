using System;
using System.IO;
using System.Net;
using System.Security;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class OnPremFromCloudAccessStream : TransportCapabilitiesAwareXmlaStream
	{
		private IMDExternalConnStream RequestStream;

		private IMDExternalConnStream ResponseStream;

		private IMDExternalConnection ExternalCnnection;

		public override bool CanRead
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

		public override bool CanWrite
		{
			get
			{
				return true;
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

		public override bool CanTimeout
		{
			get
			{
				return false;
			}
		}

		public OnPremFromCloudAccessStream(DataType desiredRequestType, DataType desiredResponseType, IMDExternalConnection externalCnnection) : base(desiredRequestType, desiredResponseType)
		{
			if (externalCnnection == null)
			{
				throw new ArgumentNullException("ExternalCnnection");
			}
			this.ExternalCnnection = externalCnnection;
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public unsafe override int Read(byte[] buffer, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			int result;
			try
			{
				if (this.ResponseStream == null)
				{
					object obj;
					this.ExternalCnnection.CreateResponseStream(out obj);
					this.ResponseStream = (IMDExternalConnStream)obj;
				}
				int num = 0;
				byte[] array = new byte[count];
				IntPtr pcbRead = new IntPtr((void*)(&num));
				this.ResponseStream.Read(array, count, pcbRead);
				Array.Copy(array, 0, buffer, offset, num);
				result = num;
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (WebException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (ProtocolViolationException innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			return result;
		}

		public unsafe override void Write(byte[] buffer, int offset, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			try
			{
				if (this.RequestStream == null)
				{
					if (this.ResponseStream != null)
					{
						this.ResponseStream.Close();
						this.ResponseStream = null;
					}
					object obj;
					this.ExternalCnnection.CreateRequestStream(1, out obj);
					this.RequestStream = (IMDExternalConnStream)obj;
				}
				int i = 0;
				int num = 0;
				IntPtr pcbWritten = new IntPtr((void*)(&num));
				while (i < count)
				{
					byte[] array = new byte[count - i];
					Array.Copy(buffer, offset + i, array, 0, count - i);
					this.RequestStream.Write(array, count - i, pcbWritten);
					if (num > 0)
					{
						i += num;
					}
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			catch (WebException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (SecurityException innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			catch (ProtocolViolationException innerException4)
			{
				throw new XmlaStreamException(innerException4);
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
				if (this.RequestStream != null)
				{
					this.RequestStream.Close();
					this.RequestStream = null;
				}
			}
			catch (XmlaStreamException)
			{
				throw;
			}
			catch (IOException innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Skip()
		{
		}

		public override void Close()
		{
		}

		public override DataType GetResponseDataType()
		{
			return DataType.TextXml;
		}

		public override void Dispose()
		{
			try
			{
				if (this.RequestStream != null)
				{
					this.RequestStream.Close();
					this.RequestStream = null;
				}
			}
			catch
			{
			}
			try
			{
				if (this.ResponseStream != null)
				{
					this.ResponseStream.Close();
					this.ResponseStream = null;
				}
			}
			catch
			{
			}
			try
			{
				if (this.ExternalCnnection != null)
				{
					this.ExternalCnnection.Close();
					this.ExternalCnnection = null;
				}
			}
			catch
			{
			}
			try
			{
				base.Dispose(true);
			}
			catch
			{
			}
		}

		protected override void DetermineNegotiatedOptions()
		{
			if (!base.NegotiatedOptions)
			{
				base.SetTransportCapabilities(new TransportCapabilities
				{
					ContentTypeNegotiated = true
				});
			}
		}
	}
}
