using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class TcpEncryptedStream : TcpSecureStream
	{
		internal TcpEncryptedStream(TcpStream tcpStream, SecurityContext securityContext) : base(tcpStream, securityContext)
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
				switch (this.securityContext.SecurityContextMode)
				{
				case SecurityContextMode.block:
					this.WriteInBlockMode(buffer, offset, size);
					break;
				case SecurityContextMode.stream:
					this.WriteInStreamMode(buffer, offset, size);
					break;
				default:
					throw new XmlaStreamException("SecurityContextMode " + this.securityContext.SecurityContextMode + " not configured!");
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

		private void WriteInStreamMode(byte[] buffer, int offset, int size)
		{
			if (TcpStream.TRACESWITCH.TraceVerbose)
			{
				StackTrace stackTrace = new StackTrace();
				stackTrace.GetFrame(1).GetMethod();
			}
			for (int i = Math.Min(this.streamSizes.cbMaxMessage, size); i > 0; i = Math.Min(this.streamSizes.cbMaxMessage, size))
			{
				this.securityBuffers[0].offset = this.streamHeaderForWrite.Offset;
				this.securityBuffers[0].size = this.streamHeaderForWrite.Count;
				this.securityBuffers[0].token = this.streamHeaderForWrite.Array;
				this.securityBuffers[0].type = 7;
				this.securityBuffers[1].offset = offset;
				this.securityBuffers[1].size = i;
				this.securityBuffers[1].token = buffer;
				this.securityBuffers[1].type = 1;
				this.securityBuffers[2].offset = this.streamTrailerForWrite.Offset;
				this.securityBuffers[2].size = this.streamTrailerForWrite.Count;
				this.securityBuffers[2].token = this.streamTrailerForWrite.Array;
				this.securityBuffers[2].type = 6;
				this.securityBuffers[3].offset = 0;
				this.securityBuffers[3].size = 0;
				this.securityBuffers[3].token = null;
				this.securityBuffers[3].type = 0;
				int num = SSPIWrapper.EncryptMessage(ref this.securityContext.Handle, this.securityBuffers, ++this.sequenceNumberForWrite);
				if (num != 0)
				{
					throw new XmlaStreamException(new Win32Exception(num).Message);
				}
				base.Write();
				offset += i;
				size -= i;
			}
		}

		private void WriteInBlockMode(byte[] buffer, int offset, int size)
		{
			for (int i = Math.Min(this.maxEncryptionBufferSize, size); i > 0; i = Math.Min(this.maxEncryptionBufferSize, size))
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
				int num = SSPIWrapper.EncryptMessage(ref this.securityContext.Handle, this.securityBuffers, ++this.sequenceNumberForWrite);
				if (num != 0)
				{
					throw new XmlaStreamException(new Win32Exception(num).Message);
				}
				base.Write();
				offset += i;
				size -= i;
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (size == 0)
			{
				return 0;
			}
			int result;
			try
			{
				switch (this.securityContext.SecurityContextMode)
				{
				case SecurityContextMode.block:
					result = this.ReadInBlockMode(buffer, offset, size);
					break;
				case SecurityContextMode.stream:
					result = this.ReadInStreamMode(buffer, offset, size);
					break;
				default:
					throw new XmlaStreamException("SecurityContextMode " + this.securityContext.SecurityContextMode + " not configured!");
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

		private int ReadInStreamMode(byte[] buffer, int offset, int size)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer can't be null!");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset can't be negative!");
			}
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size can't be negative!");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer is smaller than offset + size!");
			}
			if (this.streamDecryptedDataForRead.Count > 0)
			{
				return this.ReturnAvailableDecryptedData(buffer, offset, size);
			}
			this.ReleaseEncryptedDataAccumulatorForReadFreeBuffers();
			int i = 0;
			for (int j = 0; j < this.streamEncryptedDataAccumulatorForRead.Count; j++)
			{
				i += this.streamEncryptedDataAccumulatorForRead[j].Count;
			}
			bool value;
			if (i < this.streamSizes.cbMaxMessage)
			{
				ArraySegment<byte> orCreateFreeEncryptedBuffer = this.GetOrCreateFreeEncryptedBuffer(this.streamSizes.cbMaxMessage);
				int num = base.Read(orCreateFreeEncryptedBuffer.Array, orCreateFreeEncryptedBuffer.Offset, orCreateFreeEncryptedBuffer.Count);
				if (num > 0)
				{
					orCreateFreeEncryptedBuffer = new ArraySegment<byte>(orCreateFreeEncryptedBuffer.Array, orCreateFreeEncryptedBuffer.Offset, num);
					this.streamEncryptedDataAccumulatorForRead.Add(orCreateFreeEncryptedBuffer);
					i += num;
				}
				else
				{
					this.streamEncryptedDataAccumulatorForReadFreeBuffers.Add(orCreateFreeEncryptedBuffer);
				}
				value = true;
				if (TcpStream.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace = new StackTrace();
					stackTrace.GetFrame(1).GetMethod();
				}
			}
			else
			{
				value = false;
				if (TcpStream.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace2 = new StackTrace();
					stackTrace2.GetFrame(1).GetMethod();
				}
			}
			while (i > 0)
			{
				TcpEncryptedStream.CleanSecurityBuffers(this.securityBuffers);
				byte[] array;
				if (this.contiguosEncryptedByteArrayCache != null && this.contiguosEncryptedByteArrayCache.Length >= i)
				{
					array = this.contiguosEncryptedByteArrayCache;
				}
				else
				{
					array = (this.contiguosEncryptedByteArrayCache = new byte[i]);
				}
				int k = 0;
				int num2 = 0;
				while (k < this.streamEncryptedDataAccumulatorForRead.Count)
				{
					ArraySegment<byte> arraySegment = this.streamEncryptedDataAccumulatorForRead[k];
					Array.Copy(arraySegment.Array, arraySegment.Offset, array, num2, arraySegment.Count);
					num2 += arraySegment.Count;
					this.streamEncryptedDataAccumulatorForReadFreeBuffers.Add(new ArraySegment<byte>(arraySegment.Array));
					k++;
				}
				this.streamEncryptedDataAccumulatorForRead.Clear();
				this.securityBuffers[0].token = array;
				this.securityBuffers[0].offset = 0;
				this.securityBuffers[0].size = i;
				this.securityBuffers[0].type = 1;
				int num3 = SSPIWrapper.DecryptMessage(ref this.securityContext.Handle, this.securityBuffers, ++this.sequenceNumberForRead);
				if (num3 == -2146893032)
				{
					if (TcpStream.TRACESWITCH.TraceVerbose)
					{
						StackTrace stackTrace3 = new StackTrace();
						stackTrace3.GetFrame(1).GetMethod();
					}
					this.streamEncryptedDataAccumulatorForRead.Add(new ArraySegment<byte>(array, 0, i));
					byte[] array2 = this.contiguosEncryptedByteArrayCache = null;
				}
				else
				{
					if (num3 != 0)
					{
						throw new XmlaStreamException(new StringBuilder().Append("readFromTcp=").Append(value).Append("; ").Append("encryptedDataSz=").Append(i).ToString(), new Win32Exception(num3));
					}
					int num4 = this.ExtractDecryptedAndExtraData(this.securityBuffers, buffer, offset, size);
					if (num4 > 0)
					{
						return num4;
					}
				}
				ArraySegment<byte> orCreateFreeEncryptedBuffer2 = this.GetOrCreateFreeEncryptedBuffer(this.streamSizes.cbMaxMessage);
				int num5 = base.Read(orCreateFreeEncryptedBuffer2.Array, orCreateFreeEncryptedBuffer2.Offset, orCreateFreeEncryptedBuffer2.Count);
				if (num5 > 0)
				{
					orCreateFreeEncryptedBuffer2 = new ArraySegment<byte>(orCreateFreeEncryptedBuffer2.Array, orCreateFreeEncryptedBuffer2.Offset, num5);
					this.streamEncryptedDataAccumulatorForRead.Add(orCreateFreeEncryptedBuffer2);
				}
				else
				{
					this.streamEncryptedDataAccumulatorForReadFreeBuffers.Add(orCreateFreeEncryptedBuffer2);
					if (num3 == -2146893032)
					{
						throw new XmlaStreamException("Not complete Encrypted stream received from underlying layer of type " + base.GetType().Name + "! DecryptMessage returned SEC_E_INCOMPLETE_MESSAGE while underlying stream reported all data was read.");
					}
				}
				i = 0;
				for (int l = 0; l < this.streamEncryptedDataAccumulatorForRead.Count; l++)
				{
					i += this.streamEncryptedDataAccumulatorForRead[l].Count;
				}
				if (TcpStream.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace4 = new StackTrace();
					stackTrace4.GetFrame(1).GetMethod();
				}
			}
			return 0;
		}

		private static void CleanSecurityBuffers(SecurityBufferClass[] securityBuffers)
		{
			for (int i = 0; i < securityBuffers.Length; i++)
			{
				SecurityBufferClass securityBufferClass = securityBuffers[i];
				securityBufferClass.type = 0;
				securityBufferClass.offset = (securityBufferClass.size = 0);
				securityBufferClass.token = null;
			}
		}

		private void ReleaseEncryptedDataAccumulatorForReadFreeBuffers()
		{
			if (this.streamEncryptedDataAccumulatorForReadFreeBuffers.Count > 8)
			{
				if (TcpStream.TRACESWITCH.TraceVerbose)
				{
					StackTrace stackTrace = new StackTrace();
					stackTrace.GetFrame(1).GetMethod();
				}
				this.streamEncryptedDataAccumulatorForReadFreeBuffers.Sort((ArraySegment<byte> v1, ArraySegment<byte> v2) => v2.Count - v1.Count);
				for (int i = this.streamEncryptedDataAccumulatorForReadFreeBuffers.Count - 1; i >= 8; i--)
				{
					this.streamEncryptedDataAccumulatorForReadFreeBuffers.RemoveAt(i);
				}
			}
		}

		private ArraySegment<byte> GetOrCreateFreeEncryptedBuffer(int requiredSz)
		{
			ArraySegment<byte> result;
			if (this.streamEncryptedDataAccumulatorForReadFreeBuffers.Count > 0)
			{
				this.streamEncryptedDataAccumulatorForReadFreeBuffers.Sort((ArraySegment<byte> v1, ArraySegment<byte> v2) => v2.Count - v1.Count);
				for (int i = this.streamEncryptedDataAccumulatorForReadFreeBuffers.Count - 1; i >= 0; i--)
				{
					if (this.streamEncryptedDataAccumulatorForReadFreeBuffers[i].Count >= requiredSz)
					{
						result = this.streamEncryptedDataAccumulatorForReadFreeBuffers[i];
						this.streamEncryptedDataAccumulatorForReadFreeBuffers.RemoveAt(i);
						return result;
					}
				}
			}
			result = new ArraySegment<byte>(new byte[Math.Max(requiredSz, this.streamSizes.cbMaxMessage)]);
			return result;
		}

		private int ReturnAvailableDecryptedData(byte[] buffer, int offset, int size)
		{
			if (TcpStream.TRACESWITCH.TraceVerbose)
			{
				StackTrace stackTrace = new StackTrace();
				stackTrace.GetFrame(1).GetMethod();
				int num = 0;
				foreach (ArraySegment<byte> current in this.streamDecryptedDataForRead)
				{
					num += current.Count;
				}
			}
			int num2 = 0;
			int num3 = size;
			int num4 = offset;
			int num5 = 0;
			while (num3 > 0 && num5 < this.streamDecryptedDataForRead.Count)
			{
				ArraySegment<byte> arraySegment = this.streamDecryptedDataForRead[num5];
				int num6 = Math.Min(arraySegment.Count, num3);
				Array.Copy(arraySegment.Array, arraySegment.Offset, buffer, num4, num6);
				this.streamDecryptedDataForRead[num5] = new ArraySegment<byte>(arraySegment.Array, arraySegment.Offset + num6, arraySegment.Count - num6);
				num4 += num6;
				num3 -= num6;
				num2 += num6;
				num5++;
			}
			int i = 0;
			while (i < this.streamDecryptedDataForRead.Count)
			{
				if (this.streamDecryptedDataForRead[i].Count > 0)
				{
					i++;
				}
				else
				{
					this.streamDecryptedDataForRead.RemoveAt(i);
				}
			}
			if (TcpStream.TRACESWITCH.TraceVerbose)
			{
				StackTrace stackTrace2 = new StackTrace();
				stackTrace2.GetFrame(1).GetMethod();
				int num7 = 0;
				foreach (ArraySegment<byte> current2 in this.streamDecryptedDataForRead)
				{
					num7 += current2.Count;
				}
			}
			return num2;
		}

		private int ExtractDecryptedAndExtraData(SecurityBufferClass[] securityBuffers, byte[] readBuffer, int readOffset, int readSize)
		{
			int result = 0;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < securityBuffers.Length; i++)
			{
				SecurityBufferClass securityBufferClass = securityBuffers[i];
				if (securityBufferClass.type == 1)
				{
					num++;
					if (TcpStream.TRACESWITCH.TraceVerbose)
					{
						StackTrace stackTrace = new StackTrace();
						stackTrace.GetFrame(1).GetMethod();
					}
					if (securityBufferClass.token != null)
					{
						if (securityBufferClass.size <= readSize)
						{
							Array.Copy(securityBufferClass.token, securityBufferClass.offset, readBuffer, readOffset, securityBufferClass.size);
							result = securityBufferClass.size;
						}
						else
						{
							Array.Copy(securityBufferClass.token, securityBufferClass.offset, readBuffer, readOffset, readSize);
							result = readSize;
							this.streamDecryptedDataForRead.Add(new ArraySegment<byte>(securityBufferClass.token, securityBufferClass.offset + readSize, securityBufferClass.size - readSize));
						}
					}
					else
					{
						result = 0;
					}
				}
				else if (securityBufferClass.type == 5)
				{
					num2++;
					if (TcpStream.TRACESWITCH.TraceVerbose)
					{
						StackTrace stackTrace2 = new StackTrace();
						stackTrace2.GetFrame(1).GetMethod();
					}
					if (securityBufferClass.token != null)
					{
						this.streamEncryptedDataAccumulatorForRead.Add(new ArraySegment<byte>(securityBufferClass.token, securityBufferClass.offset, securityBufferClass.size));
					}
				}
			}
			return result;
		}

		private int ReadInBlockMode(byte[] buffer, int offset, int size)
		{
			while (this.dataSizeForRead <= 0)
			{
				if (!base.ReadHeader())
				{
					return 0;
				}
				for (int i = 0; i < (int)this.dataSizeForRead; i += base.Read(this.dataBufferForRead, i, (int)this.dataSizeForRead - i))
				{
				}
				for (int j = 0; j < (int)this.tokenSizeForRead; j += base.Read(this.tokenBufferForRead, j, (int)this.tokenSizeForRead - j))
				{
				}
				this.securityBuffers[0].offset = 0;
				this.securityBuffers[0].size = (int)this.dataSizeForRead;
				this.securityBuffers[0].token = this.dataBufferForRead;
				this.securityBuffers[0].type = 1;
				this.securityBuffers[1].offset = 0;
				this.securityBuffers[1].size = (int)this.tokenSizeForRead;
				this.securityBuffers[1].token = this.tokenBufferForRead;
				this.securityBuffers[1].type = 2;
				int num = SSPIWrapper.DecryptMessage(ref this.securityContext.Handle, this.securityBuffers, ++this.sequenceNumberForRead);
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
			return (int)num2;
		}
	}
}
