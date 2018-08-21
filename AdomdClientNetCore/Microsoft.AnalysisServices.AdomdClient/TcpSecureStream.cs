using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class TcpSecureStream : TcpStream
	{
		protected byte[] outBufferForPackageSize = new byte[4];

		protected byte[] inBufferForPackageSize = new byte[4];

		protected SecurityContext securityContext;

		protected SecurityBufferClass[] securityBuffers;

		protected int maxTokenSize;

		protected int maxEncryptionBufferSize;

		protected StreamSizes streamSizes;

		protected ArraySegment<byte> streamHeaderForWrite;

		protected ArraySegment<byte> streamTrailerForWrite;

		protected List<ArraySegment<byte>> streamEncryptedDataAccumulatorForRead;

		protected List<ArraySegment<byte>> streamEncryptedDataAccumulatorForReadFreeBuffers;

		protected byte[] contiguosEncryptedByteArrayCache;

		protected List<ArraySegment<byte>> streamDecryptedDataForRead;

		protected byte[] dataBufferForRead = new byte[65535];

		protected ushort dataOffsetForRead;

		protected ushort dataSizeForRead;

		protected byte[] tokenBufferForRead;

		protected ushort tokenSizeForRead;

		protected int sequenceNumberForRead;

		protected byte[] tokenBufferForWrite;

		protected int sequenceNumberForWrite;

		internal TcpSecureStream(TcpStream tcpStream, SecurityContext securityContext) : base(tcpStream)
		{
			if (securityContext == null)
			{
				throw new ArgumentNullException("securityContext");
			}
			this.securityContext = securityContext;
			try
			{
				switch (this.securityContext.SecurityContextMode)
				{
				case SecurityContextMode.block:
				{
					Sizes sizes = (Sizes)SSPIWrapper.QueryContextAttributes(securityContext, ContextAttribute.Sizes);
					this.maxTokenSize = Math.Max(sizes.cbSecurityTrailer, sizes.cbMaxSignature);
					this.maxEncryptionBufferSize = Math.Min(sizes.cbMaxToken, 65535);
					if (this.maxTokenSize > 65535)
					{
						throw new XmlaStreamException(XmlaSR.TcpStream_MaxSignatureExceedsProtocolLimit);
					}
					this.tokenBufferForWrite = new byte[this.maxTokenSize];
					this.tokenBufferForRead = new byte[this.maxTokenSize];
					this.securityBuffers = new SecurityBufferClass[]
					{
						new SecurityBufferClass(null, BufferType.Data),
						new SecurityBufferClass(this.tokenBufferForWrite, BufferType.Token)
					};
					break;
				}
				case SecurityContextMode.stream:
				{
					this.streamSizes = (StreamSizes)SSPIWrapper.QueryContextAttributes(securityContext, ContextAttribute.StreamSizes);
					if (this.streamSizes.cbMaxMessage > 65535)
					{
						throw new XmlaStreamException(XmlaSR.TcpStream_MaxSignatureExceedsProtocolLimit);
					}
					this.streamHeaderForWrite = new ArraySegment<byte>(new byte[this.streamSizes.cbHeader]);
					this.streamTrailerForWrite = new ArraySegment<byte>(new byte[this.streamSizes.cbTrailer]);
					this.streamEncryptedDataAccumulatorForRead = new List<ArraySegment<byte>>();
					this.streamEncryptedDataAccumulatorForReadFreeBuffers = new List<ArraySegment<byte>>();
					this.streamDecryptedDataForRead = new List<ArraySegment<byte>>();
					SecurityBufferClass securityBufferClass = new SecurityBufferClass(this.streamHeaderForWrite.Array, BufferType.Header);
					SecurityBufferClass securityBufferClass2 = new SecurityBufferClass(null, BufferType.Data);
					SecurityBufferClass securityBufferClass3 = new SecurityBufferClass(this.streamTrailerForWrite.Array, BufferType.Trailer);
					SecurityBufferClass securityBufferClass4 = new SecurityBufferClass(null, BufferType.Empty);
					this.securityBuffers = new SecurityBufferClass[]
					{
						securityBufferClass,
						securityBufferClass2,
						securityBufferClass3,
						securityBufferClass4
					};
					break;
				}
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

		public override void Dispose()
		{
			try
			{
				this.securityContext.Close();
			}
			catch (Win32Exception)
			{
			}
			finally
			{
				base.Dispose();
			}
		}

		protected void Write()
		{
			switch (this.securityContext.SecurityContextMode)
			{
			case SecurityContextMode.block:
				this.WriteInBlockMode();
				return;
			case SecurityContextMode.stream:
				this.WriteInStreamMode();
				return;
			default:
				throw new XmlaStreamException("SecurityContextMode " + this.securityContext.SecurityContextMode + " not configured!");
			}
		}

		private void WriteInStreamMode()
		{
			base.Write(this.securityBuffers[0].token, this.securityBuffers[0].offset, this.securityBuffers[0].size);
			base.Write(this.securityBuffers[1].token, this.securityBuffers[1].offset, this.securityBuffers[1].size);
			base.Write(this.securityBuffers[2].token, this.securityBuffers[2].offset, this.securityBuffers[2].size);
		}

		private void WriteInBlockMode()
		{
			int size = this.securityBuffers[0].size;
			int size2 = this.securityBuffers[1].size;
			if (this.securityBuffers[0].type != 1)
			{
				throw new XmlaStreamException(XmlaSR.InternalErrorAndInvalidBufferType);
			}
			if (this.securityBuffers[1].type != 2)
			{
				throw new XmlaStreamException(XmlaSR.InternalErrorAndInvalidBufferType);
			}
			this.WriteHeader((ushort)size, (ushort)size2);
			base.Write(this.securityBuffers[0].token, this.securityBuffers[0].offset, size);
			base.Write(this.securityBuffers[1].token, this.securityBuffers[1].offset, size2);
		}

		public override void Skip()
		{
			try
			{
				base.Skip();
				this.dataSizeForRead = 0;
				this.dataOffsetForRead = 0;
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

		protected void WriteHeader(ushort dataSize, ushort tokenSize)
		{
			this.outBufferForPackageSize[0] = (byte)(dataSize & 255);
			this.outBufferForPackageSize[1] = (byte)(dataSize >> 8 & 255);
			this.outBufferForPackageSize[2] = (byte)(tokenSize & 255);
			this.outBufferForPackageSize[3] = (byte)(tokenSize >> 8 & 255);
			base.Write(this.outBufferForPackageSize, 0, 4);
		}

		protected bool ReadHeader()
		{
			int i = 0;
			while (i < 4)
			{
				int num = base.Read(this.inBufferForPackageSize, i, 4 - i);
				if (num == 0)
				{
					if (i == 0)
					{
						return false;
					}
					throw new Exception(XmlaSR.UnknownServerResponseFormat);
				}
				else
				{
					i += num;
				}
			}
			this.dataSizeForRead = (ushort)((int)this.inBufferForPackageSize[0] + ((int)this.inBufferForPackageSize[1] << 8));
			this.tokenSizeForRead = (ushort)((int)this.inBufferForPackageSize[2] + ((int)this.inBufferForPackageSize[3] << 8));
			return true;
		}
	}
}
