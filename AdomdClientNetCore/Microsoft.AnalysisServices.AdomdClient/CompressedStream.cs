using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class CompressedStream : XmlaStream
	{
		private const ushort WriteBufferFlushThreshold = 65535;

		private XmlaStream baseXmlaStream;

		private IntPtr compressHandle = IntPtr.Zero;

		private IntPtr decompressHandle = IntPtr.Zero;

		private byte[] compressionHeader;

		private byte[] compressedBuffer;

		private byte[] decompressedBuffer;

		private ushort decompressedBufferOffset;

		private ushort decompressedBufferSize;

		private ushort writeCacheOffset = 8;

		private int compressionLevel;

		private XpressMethodsWrapper xpressWrapper;

		private XpressMethodsWrapper XpressWrapper
		{
			get
			{
				if (this.xpressWrapper == null)
				{
					this.xpressWrapper = XpressMethodsWrapper.XpressWrapper;
				}
				return this.xpressWrapper;
			}
		}

		internal XmlaStream BaseXmlaStream
		{
			get
			{
				return this.baseXmlaStream;
			}
		}

		public override bool IsCompressionEnabled
		{
			get
			{
				return this.baseXmlaStream.IsCompressionEnabled;
			}
			set
			{
				this.baseXmlaStream.IsCompressionEnabled = value;
			}
		}

		public override string SessionID
		{
			get
			{
				return this.baseXmlaStream.SessionID;
			}
			set
			{
				this.baseXmlaStream.SessionID = value;
			}
		}

		public override bool IsSessionTokenNeeded
		{
			get
			{
				return this.baseXmlaStream.IsSessionTokenNeeded;
			}
			set
			{
				this.baseXmlaStream.IsSessionTokenNeeded = value;
			}
		}

		public override Guid ActivityID
		{
			get
			{
				return this.baseXmlaStream.ActivityID;
			}
			set
			{
				this.baseXmlaStream.ActivityID = value;
			}
		}

		public override Guid RequestID
		{
			get
			{
				return this.baseXmlaStream.RequestID;
			}
			set
			{
				this.baseXmlaStream.RequestID = value;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return this.baseXmlaStream.CanTimeout;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return this.baseXmlaStream.ReadTimeout;
			}
			set
			{
				this.baseXmlaStream.ReadTimeout = value;
			}
		}

		private bool CompressedWriteEnabled
		{
			get
			{
				if (this.baseXmlaStream != null)
				{
					DataType requestDataType = this.baseXmlaStream.GetRequestDataType();
					return requestDataType == DataType.CompressedXml || requestDataType == DataType.CompressedBinaryXml;
				}
				return false;
			}
		}

		internal CompressedStream(XmlaStream xmlaStream, int compressionLevel)
		{
			try
			{
				this.baseXmlaStream = xmlaStream;
				this.compressionLevel = compressionLevel;
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public virtual void SetBaseXmlaStream(XmlaStream xmlaStream)
		{
			try
			{
				if (this.baseXmlaStream != xmlaStream)
				{
					if (this.baseXmlaStream != null)
					{
						this.baseXmlaStream.Dispose();
					}
					this.baseXmlaStream = xmlaStream;
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override DataType GetResponseDataType()
		{
			DataType result;
			try
			{
				switch (this.baseXmlaStream.GetResponseDataType())
				{
				case DataType.BinaryXml:
				case DataType.CompressedBinaryXml:
					result = DataType.BinaryXml;
					return result;
				}
				result = DataType.TextXml;
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			return result;
		}

		public override DataType GetRequestDataType()
		{
			DataType result;
			try
			{
				switch (this.baseXmlaStream.GetRequestDataType())
				{
				case DataType.BinaryXml:
				case DataType.CompressedBinaryXml:
					result = DataType.BinaryXml;
					return result;
				}
				result = DataType.TextXml;
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			return result;
		}

		public override void WriteSoapActionHeader(string action)
		{
			if (this.baseXmlaStream != null)
			{
				this.baseXmlaStream.WriteSoapActionHeader(action);
			}
		}

		public override string GetExtendedErrorInfo()
		{
			if (this.baseXmlaStream == null)
			{
				return string.Empty;
			}
			return this.baseXmlaStream.GetExtendedErrorInfo();
		}

		public override void Close()
		{
			this.baseXmlaStream.Close();
		}

		public override void Dispose()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			try
			{
				this.CloseCompressionHandlesAndBuffers();
				if (disposing)
				{
					this.baseXmlaStream.Dispose();
				}
				this.xpressWrapper = null;
				this.disposed = true;
				if (disposing)
				{
					GC.SuppressFinalize(this);
				}
			}
			catch (IOException)
			{
			}
			catch (Win32Exception)
			{
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		~CompressedStream()
		{
			this.Dispose(false);
		}

		public override void WriteEndOfMessage()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.CompressedWriteEnabled && this.writeCacheOffset > 8)
				{
					this.FlushCache();
				}
				this.baseXmlaStream.WriteEndOfMessage();
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override void Write(byte[] buffer, int offset, int size)
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
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			if (!this.CompressedWriteEnabled)
			{
				this.baseXmlaStream.Write(buffer, offset, size);
				return;
			}
			try
			{
				if (this.compressHandle == IntPtr.Zero)
				{
					this.InitCompress();
				}
				int i = size;
				int num = offset;
				while (i > 0)
				{
					if (this.writeCacheOffset >= 65535)
					{
						this.FlushCache();
					}
					ushort num2 = (ushort)Math.Min((int)(65535 - this.writeCacheOffset), i);
					Buffer.BlockCopy(buffer, num, this.decompressedBuffer, (int)this.writeCacheOffset, (int)num2);
					this.writeCacheOffset += num2;
					num += (int)num2;
					i -= (int)num2;
				}
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
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
			if (size < 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (size + offset > buffer.Length)
			{
				throw new ArgumentException(XmlaSR.InvalidArgument, "buffer");
			}
			if (size == 0)
			{
				return 0;
			}
			int result;
			try
			{
				int num = 0;
				if (this.decompressedBufferSize > 0)
				{
					num = this.ReadFromCache(buffer, offset, size);
				}
				else
				{
					DataType responseDataType = this.baseXmlaStream.GetResponseDataType();
					switch (responseDataType)
					{
					case DataType.Undetermined:
						break;
					case DataType.TextXml:
					case DataType.BinaryXml:
						num = this.baseXmlaStream.Read(buffer, offset, size);
						break;
					case DataType.CompressedXml:
					case DataType.CompressedBinaryXml:
						if (this.decompressHandle == IntPtr.Zero)
						{
							this.InitDecompress();
						}
						while (this.decompressedBufferSize <= 0 && this.ReadCompressedPacket())
						{
						}
						if (this.decompressedBufferSize > 0)
						{
							num = this.ReadFromCache(buffer, offset, size);
						}
						break;
					default:
						throw new NotImplementedException(XmlaSR.UnsupportedDataFormat(responseDataType.ToString()));
					}
				}
				result = num;
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
			return result;
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			this.baseXmlaStream.Skip();
			this.decompressedBufferOffset = 0;
			this.decompressedBufferSize = 0;
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				if (this.CompressedWriteEnabled && this.writeCacheOffset > 8)
				{
					this.FlushCache();
				}
				this.baseXmlaStream.Flush();
			}
			catch (Win32Exception innerException)
			{
				throw new XmlaStreamException(innerException);
			}
		}

		private void InitDecompress()
		{
			if (this.decompressHandle != IntPtr.Zero)
			{
				this.XpressWrapper.DecompressClose(this.decompressHandle);
			}
			this.decompressHandle = this.XpressWrapper.DecompressInit();
			if (this.decompressHandle == IntPtr.Zero)
			{
				throw new XmlaStreamException(XmlaSR.Decompression_InitializationFailed);
			}
			if (this.decompressedBuffer == null)
			{
				this.InitCompressionBuffers();
			}
		}

		private void InitCompressionBuffers()
		{
			this.compressedBuffer = new byte[65535];
			this.decompressedBuffer = new byte[65535];
			this.compressionHeader = new byte[8];
		}

		private void Decompress(int compressedDataSize, int decompressedDataSize)
		{
			int num = this.XpressWrapper.Decompress(this.decompressHandle, this.compressedBuffer, compressedDataSize, this.decompressedBuffer, decompressedDataSize, decompressedDataSize);
			if (num != decompressedDataSize)
			{
				throw new XmlaStreamException(XmlaSR.Decompression_Failed(compressedDataSize, decompressedDataSize, num));
			}
		}

		private void CloseCompressionHandlesAndBuffers()
		{
			if (this.compressHandle != IntPtr.Zero)
			{
				this.XpressWrapper.CompressClose(this.compressHandle);
				this.compressHandle = IntPtr.Zero;
			}
			if (this.decompressHandle != IntPtr.Zero)
			{
				this.XpressWrapper.DecompressClose(this.decompressHandle);
				this.decompressHandle = IntPtr.Zero;
			}
			this.compressionHeader = null;
			this.compressedBuffer = null;
			this.decompressedBuffer = null;
			this.decompressedBufferOffset = 0;
			this.decompressedBufferSize = 0;
		}

		private bool ReadCompressedPacket()
		{
			int i = 0;
			while (i < 8)
			{
				int num = this.baseXmlaStream.Read(this.compressionHeader, i, 8 - i);
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
			ushort num2 = (ushort)((int)this.compressionHeader[0] + ((int)this.compressionHeader[1] << 8));
			ushort num3 = (ushort)((int)this.compressionHeader[4] + ((int)this.compressionHeader[5] << 8));
			bool flag = 0 < num3 && num3 < num2;
			ushort num4 = flag ? num3 : num2;
			byte[] buffer = flag ? this.compressedBuffer : this.decompressedBuffer;
			int num5;
			for (int j = 0; j < (int)num4; j += num5)
			{
				num5 = this.baseXmlaStream.Read(buffer, j, (int)num4 - j);
				if (num5 == 0)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Could not read all expected data");
				}
			}
			if (flag)
			{
				this.Decompress((int)num3, (int)num2);
			}
			this.decompressedBufferOffset = 0;
			this.decompressedBufferSize = num2;
			return true;
		}

		private int ReadFromCache(byte[] buffer, int offset, int size)
		{
			int num = Math.Min((int)this.decompressedBufferSize, size);
			Buffer.BlockCopy(this.decompressedBuffer, (int)this.decompressedBufferOffset, buffer, offset, num);
			this.decompressedBufferSize -= (ushort)num;
			this.decompressedBufferOffset += (ushort)num;
			return num;
		}

		private void InitCompress()
		{
			if (this.compressHandle == IntPtr.Zero)
			{
				this.compressHandle = this.XpressWrapper.CompressInit(65536, this.compressionLevel);
				if (this.compressHandle == IntPtr.Zero)
				{
					throw new XmlaStreamException(XmlaSR.Compression_InitializationFailed);
				}
				this.InitCompressionBuffers();
			}
		}

		private void FlushCache()
		{
			int num = (int)(this.writeCacheOffset - 8);
			int num2 = this.XpressWrapper.Compress(this.compressHandle, this.decompressedBuffer, 8, num, this.compressedBuffer, 8, num);
			byte[] array;
			int count;
			if (0 < num2 && num2 < num)
			{
				array = this.compressedBuffer;
				count = num2 + 8;
			}
			else
			{
				array = this.decompressedBuffer;
				count = (int)this.writeCacheOffset;
			}
			array[0] = (byte)(num & 255);
			array[1] = (byte)(num >> 8 & 255);
			array[2] = 0;
			array[3] = 0;
			array[4] = (byte)(num2 & 255);
			array[5] = (byte)(num2 >> 8 & 255);
			array[6] = 0;
			array[7] = 0;
			this.baseXmlaStream.Write(array, 0, count);
			this.writeCacheOffset = 8;
		}
	}
}
