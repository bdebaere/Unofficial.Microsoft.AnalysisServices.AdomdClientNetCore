using System;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class TcpSignedStream : TcpSecureStream
	{
		internal TcpSignedStream(TcpStream tcpStream, SecurityContext securityContext) : base(tcpStream, securityContext)
		{
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				for (int i = Math.Min(65535, size); i > 0; i = Math.Min(65535, size))
				{
					SecurityBufferClass securityBufferClass = this.securityBuffers[0];
					securityBufferClass.offset = offset;
					securityBufferClass.size = i;
					securityBufferClass.token = buffer;
					securityBufferClass.type = 1;
					securityBufferClass = this.securityBuffers[1];
					securityBufferClass.offset = 0;
					securityBufferClass.size = this.maxTokenSize;
					securityBufferClass.token = this.tokenBufferForWrite;
					securityBufferClass.type = 2;
					int num = SSPIWrapper.MakeSignature(ref this.securityContext.Handle, 0, this.securityBuffers, ++this.sequenceNumberForWrite);
					if (num != 0)
					{
						throw new XmlaStreamException(new Win32Exception(num).Message);
					}
					base.Write();
					offset += i;
					size -= i;
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
			catch (SocketException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (Win32Exception innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			int result;
			try
			{
				if (size == 0)
				{
					result = 0;
				}
				else
				{
					while (this.dataSizeForRead <= 0)
					{
						if (!this.Read())
						{
							result = 0;
							return result;
						}
						int num = SSPIWrapper.VerifySignature(ref this.securityContext.Handle, 0, this.securityBuffers, ++this.sequenceNumberForRead);
						if (num != 0)
						{
							throw new XmlaStreamException(new Win32Exception(num).Message);
						}
						SecurityBufferClass securityBufferClass = this.securityBuffers[0];
						if (securityBufferClass.offset + securityBufferClass.size > 65535)
						{
							throw new XmlaStreamException(XmlaSR.InternalError);
						}
						this.dataOffsetForRead = (ushort)securityBufferClass.offset;
						this.dataSizeForRead = (ushort)securityBufferClass.size;
					}
					ushort num2 = 0;
					int num3 = (int)this.dataOffsetForRead;
					int num4 = (int)(this.dataOffsetForRead + this.dataSizeForRead);
					int num5 = offset;
					int num6 = offset + size;
					while (num3 < num4 && num5 < num6)
					{
						buffer[num5] = this.dataBufferForRead[num3];
						num3++;
						num5++;
						num2 += 1;
					}
					this.dataSizeForRead -= num2;
					this.dataOffsetForRead += num2;
					result = (int)num2;
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
			catch (SocketException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			catch (Win32Exception innerException3)
			{
				throw new XmlaStreamException(innerException3);
			}
			return result;
		}

		private bool Read()
		{
			if (!base.ReadHeader())
			{
				return false;
			}
			for (int i = 0; i < (int)this.dataSizeForRead; i += base.Read(this.dataBufferForRead, i, (int)this.dataSizeForRead - i))
			{
			}
			for (int j = 0; j < (int)this.tokenSizeForRead; j += base.Read(this.tokenBufferForRead, j, (int)this.tokenSizeForRead - j))
			{
			}
			SecurityBufferClass securityBufferClass = this.securityBuffers[0];
			securityBufferClass.offset = 0;
			securityBufferClass.size = (int)this.dataSizeForRead;
			securityBufferClass.token = this.dataBufferForRead;
			securityBufferClass.type = 1;
			securityBufferClass = this.securityBuffers[1];
			securityBufferClass.offset = 0;
			securityBufferClass.size = (int)this.tokenSizeForRead;
			securityBufferClass.token = this.tokenBufferForRead;
			securityBufferClass.type = 2;
			return true;
		}
	}
}
