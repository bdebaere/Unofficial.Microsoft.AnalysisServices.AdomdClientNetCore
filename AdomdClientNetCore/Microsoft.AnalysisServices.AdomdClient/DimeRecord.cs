using System;
using System.IO;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class DimeRecord
	{
		[Flags]
		private enum HeaderFlagsEnum : byte
		{
			BeginOfMessage = 4,
			EndOfMessage = 2,
			ChunkedRecord = 1
		}

		private const int PaddingMultiple = 4;

		private const int FixedHeaderSize = 12;

		private const byte HeaderFlagMask = 7;

		private const byte VersionFlagMask = 248;

		private const byte TypeFlagMask = 240;

		private const byte ReservedFlagMask = 15;

		private const int MaxMetadataLength = 65535;

		private IOModeEnum m_ioMode;

		private Stream m_stream;

		private MemoryStream m_bodyStreamBuffer;

		private bool m_chunked;

		private bool m_firstChunk;

		private bool m_headerWritten;

		private bool m_beginOfMessage;

		private bool m_endOfMessage;

		private bool m_closed;

		private int m_chunkSize;

		private Uri m_id;

		private string m_type;

		private TypeFormatEnum m_typeFormat;

		private byte m_reserved;

		private byte m_version;

		private TransportCapabilities m_Options = new TransportCapabilities();

		private int m_contentLength;

		private int m_bytesReadWritten;

		internal bool CanRead
		{
			get
			{
				return this.m_ioMode == IOModeEnum.ReadOnly && !this.m_closed;
			}
		}

		internal bool CanWrite
		{
			get
			{
				return this.m_ioMode == IOModeEnum.WriteOnly && !this.m_closed;
			}
		}

		public int ChunkSize
		{
			get
			{
				return this.m_chunkSize;
			}
		}

		public bool EndOfMessage
		{
			get
			{
				if (this.CanWrite)
				{
					throw new InvalidOperationException(XmlaSR.DimeRecord_PropertyOnlyAvailableForReadRecords);
				}
				return this.m_endOfMessage;
			}
		}

		public string Type
		{
			get
			{
				return this.m_type;
			}
		}

		public TransportCapabilities Options
		{
			get
			{
				return this.m_Options;
			}
			set
			{
				this.m_Options = value;
			}
		}

		public TypeFormatEnum TypeFormat
		{
			get
			{
				return this.m_typeFormat;
			}
		}

		private static int RoundUp(int length)
		{
			return length + 3 & -4;
		}

		internal DimeRecord(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException(XmlaSR.DimeRecord_StreamShouldBeReadable, "stream");
			}
			this.m_ioMode = IOModeEnum.ReadOnly;
			this.m_stream = stream;
			this.m_firstChunk = true;
			this.ReadHeader();
		}

		internal DimeRecord(Stream stream, Uri id, string type, TypeFormatEnum typeFormat, bool beginOfMessage, int contentLength, int chunkSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException(XmlaSR.DimeRecord_StreamShouldBeWriteable, "stream");
			}
			if (contentLength < -1)
			{
				throw new ArgumentException(XmlaSR.DimeRecord_InvalidContentLength, "contentLength");
			}
			this.SetType(type, typeFormat);
			this.m_id = id;
			this.m_contentLength = contentLength;
			this.m_chunked = (contentLength == -1);
			this.m_firstChunk = this.m_chunked;
			this.m_beginOfMessage = beginOfMessage;
			this.m_stream = stream;
			this.m_ioMode = IOModeEnum.WriteOnly;
			this.m_chunkSize = chunkSize;
		}

		public void Close()
		{
			this.Close(false);
		}

		internal void Close(bool endOfMessage)
		{
			if (this.m_closed)
			{
				return;
			}
			this.m_closed = true;
			if (this.m_ioMode != IOModeEnum.ReadOnly)
			{
				if (this.m_ioMode == IOModeEnum.WriteOnly)
				{
					if (this.m_chunked)
					{
						this.WriteChunkedPayload(true, endOfMessage);
						return;
					}
					if (endOfMessage)
					{
						this.WriteMessageEndRecord();
					}
				}
				return;
			}
			if (this.m_bytesReadWritten == this.m_contentLength)
			{
				return;
			}
			byte[] array = new byte[this.m_contentLength - this.m_bytesReadWritten];
			while (this.m_stream.Read(array, 0, array.Length) > 0)
			{
			}
		}

		private void ReadHeader()
		{
			byte[] array = new byte[12];
			DimeRecord.ForceRead(this.m_stream, array, 12);
			this.m_version = (byte)((array[0] & 248) >> 3);
			if (this.m_version != 1)
			{
				throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_VersionNotSupported((int)this.m_version), "");
			}
			DimeRecord.HeaderFlagsEnum headerFlagsEnum = (DimeRecord.HeaderFlagsEnum)(array[0] & 7);
			this.m_chunked = ((byte)(headerFlagsEnum & DimeRecord.HeaderFlagsEnum.ChunkedRecord) != 0);
			this.m_beginOfMessage = ((byte)(headerFlagsEnum & DimeRecord.HeaderFlagsEnum.BeginOfMessage) != 0);
			this.m_endOfMessage = ((byte)(headerFlagsEnum & DimeRecord.HeaderFlagsEnum.EndOfMessage) != 0);
			if (this.m_chunked && this.m_endOfMessage)
			{
				throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_InvalidHeaderFlags(this.m_beginOfMessage ? 1 : 0, 1, 1), "");
			}
			if ((!this.m_chunked && !this.m_endOfMessage) || (!this.m_firstChunk && this.m_beginOfMessage))
			{
				throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_OnlySingleRecordMessagesAreSupported, "");
			}
			this.m_typeFormat = (TypeFormatEnum)((array[1] & 240) >> 4);
			this.m_reserved = (byte)(array[1] & 15);
			int num = ((int)array[2] << 8) + (int)array[3];
			int num2 = ((int)array[4] << 8) + (int)array[5];
			int num3 = ((int)array[6] << 8) + (int)array[7];
			this.m_contentLength = ((int)array[8] << 24) + ((int)array[9] << 16) + ((int)array[10] << 8) + (int)array[11];
			if (this.m_firstChunk)
			{
				if (this.m_typeFormat != TypeFormatEnum.MediaType)
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_TypeFormatShouldBeMedia(this.m_typeFormat.ToString()), "");
				}
				if (num3 <= 0)
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_DataTypeShouldBeSpecifiedOnTheFirstChunk, "");
				}
			}
			else
			{
				if (this.m_typeFormat != TypeFormatEnum.Unchanged)
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_TypeFormatShouldBeUnchanged(this.m_typeFormat.ToString()), "");
				}
				if (num3 != 0)
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_DataTypeIsOnlyForTheFirstChunk, "");
				}
				if (num2 != 0)
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_IDIsOnlyForFirstChunk, "");
				}
				if (num != 0)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, "Unexpected non-zero options length");
				}
			}
			if (this.m_reserved != 0)
			{
				throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_ReservedFlagShouldBeZero(this.m_reserved), "");
			}
			if (num > 0)
			{
				array = new byte[DimeRecord.RoundUp(num)];
				DimeRecord.ForceRead(this.m_stream, array, array.Length);
				this.m_Options.FromBytes(array);
			}
			if (num2 > 0)
			{
				array = new byte[DimeRecord.RoundUp(num2)];
				DimeRecord.ForceRead(this.m_stream, array, array.Length);
			}
			if (num3 > 0)
			{
				array = new byte[DimeRecord.RoundUp(num3)];
				DimeRecord.ForceRead(this.m_stream, array, array.Length);
				this.m_type = Encoding.ASCII.GetString(array, 0, num3);
				if (!DataTypes.IsSupportedDataType(this.m_type))
				{
					throw new AdomdUnknownResponseException(XmlaSR.DimeRecord_DataTypeNotSupported(this.m_type), "");
				}
			}
			this.m_firstChunk = false;
		}

		private static void ForceRead(Stream stream, byte[] buffer, int length)
		{
			int num;
			for (int i = 0; i < length; i += num)
			{
				num = stream.Read(buffer, i, length - i);
				if (num == 0)
				{
					throw new IOException(XmlaSR.DimeRecord_UnableToReadFromStream);
				}
			}
		}

		internal int ReadBody(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (this.m_closed)
			{
				throw new Exception(XmlaSR.DimeRecord_StreamIsClosed);
			}
			if (this.m_ioMode != IOModeEnum.ReadOnly)
			{
				throw new InvalidOperationException(XmlaSR.DimeRecord_ReadNotAllowed);
			}
			int num = 0;
			int num2 = this.m_contentLength - this.m_bytesReadWritten;
			if (this.m_chunked && num2 == 0)
			{
				do
				{
					this.ReadHeader();
				}
				while (this.m_contentLength == 0 && this.m_chunked);
				num2 = this.m_contentLength;
				this.m_bytesReadWritten = 0;
			}
			if (num2 > 0)
			{
				num = this.m_stream.Read(buffer, offset, (num2 < count) ? num2 : count);
				this.m_bytesReadWritten += num;
				if (this.m_bytesReadWritten == this.m_contentLength)
				{
					DimeRecord.ReadPadding(this.m_stream, this.m_bytesReadWritten);
				}
			}
			return num;
		}

		private void SetType(string type, TypeFormatEnum typeFormat)
		{
			switch (typeFormat)
			{
			case TypeFormatEnum.Unchanged:
				throw new ArgumentException(XmlaSR.DimeRecord_TypeFormatEnumUnchangedNotAllowed, "typeFormat");
			case TypeFormatEnum.MediaType:
				if (type == null || type.Length == 0)
				{
					throw new ArgumentException(XmlaSR.DimeRecord_MediaTypeNotDefined, "type");
				}
				break;
			case TypeFormatEnum.None:
				if (type != null || type.Length != 0)
				{
					throw new ArgumentException(XmlaSR.DimeRecord_NameMustNotBeDefinedForFormatNone, "type");
				}
				break;
			}
			this.m_typeFormat = typeFormat;
			this.m_type = type;
		}

		private void WriteHeader(bool endOfRecord, bool endOfMessage, long contentLength)
		{
			byte[] array = new byte[12];
			byte[] array2 = null;
			byte[] array3 = null;
			DimeRecord.HeaderFlagsEnum headerFlagsEnum = (DimeRecord.HeaderFlagsEnum)0;
			if (this.m_chunked && !endOfRecord)
			{
				headerFlagsEnum = DimeRecord.HeaderFlagsEnum.ChunkedRecord;
			}
			if (this.m_beginOfMessage)
			{
				headerFlagsEnum |= DimeRecord.HeaderFlagsEnum.BeginOfMessage;
				this.m_beginOfMessage = false;
			}
			if (endOfMessage)
			{
				headerFlagsEnum |= DimeRecord.HeaderFlagsEnum.EndOfMessage;
			}
			TypeFormatEnum typeFormatEnum;
			Uri uri;
			string text;
			if (!this.m_chunked || this.m_firstChunk)
			{
				typeFormatEnum = this.m_typeFormat;
				uri = this.m_id;
				text = this.m_type;
			}
			else
			{
				typeFormatEnum = TypeFormatEnum.Unchanged;
				uri = null;
				text = null;
			}
			array[0] = (byte)(headerFlagsEnum | (DimeRecord.HeaderFlagsEnum)8);
			array[1] = (byte)((byte)typeFormatEnum << 4);
			if (!this.m_chunked || this.m_firstChunk)
			{
				array[2] = 0;
				array[3] = 4;
			}
			int length;
			if (uri != null && (length = uri.AbsoluteUri.Length) > 0)
			{
				int byteCount = Encoding.ASCII.GetByteCount(uri.AbsoluteUri);
				if (byteCount > 65535)
				{
					throw new Exception(XmlaSR.DimeRecord_EncodedTypeLengthExceeds8191);
				}
				array3 = new byte[DimeRecord.PaddedCount(byteCount)];
				Encoding.ASCII.GetBytes(this.m_id.AbsoluteUri, 0, length, array3, 0);
				array[4] = (byte)(byteCount >> 8);
				array[5] = (byte)byteCount;
			}
			if (text != null && text.Length > 0)
			{
				int byteCount = Encoding.ASCII.GetByteCount(text);
				if (byteCount > 65535)
				{
					throw new Exception(XmlaSR.DimeRecord_EncodedTypeLengthExceeds8191);
				}
				array2 = new byte[DimeRecord.PaddedCount(byteCount)];
				Encoding.ASCII.GetBytes(text, 0, text.Length, array2, 0);
				array[6] = (byte)(byteCount >> 8);
				array[7] = (byte)byteCount;
			}
			if (contentLength > 0L)
			{
				array[8] = (byte)(contentLength >> 24 & 255L);
				array[9] = (byte)(contentLength >> 16 & 255L);
				array[10] = (byte)(contentLength >> 8 & 255L);
				array[11] = (byte)(contentLength & 255L);
			}
			this.m_stream.Write(array, 0, 12);
			if (array3 != null && array3.Length > 0)
			{
				this.m_stream.Write(array3, 0, array3.Length);
			}
			if (!this.m_chunked || this.m_firstChunk)
			{
				this.m_stream.Write(this.m_Options.GetBytes(), 0, 4);
			}
			if (array2 != null && array2.Length > 0)
			{
				this.m_stream.Write(array2, 0, array2.Length);
			}
			this.m_firstChunk = false;
		}

		private static int PaddedCount(int byteCount)
		{
			return byteCount + ((byteCount % 4 > 0) ? (4 - byteCount % 4) : 0);
		}

		private void WriteMessageEndRecord()
		{
			this.m_stream.WriteByte(2);
			this.m_stream.WriteByte(0);
			this.m_stream.WriteByte(128);
			for (int i = 0; i < 9; i++)
			{
				this.m_stream.WriteByte(0);
			}
		}

		internal void WriteBody(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (count < 0 || offset < 0)
			{
				throw new ArgumentOutOfRangeException(XmlaSR.DimeRecord_OffsetAndCountShouldBePositive);
			}
			if (this.m_closed)
			{
				throw new InvalidOperationException(XmlaSR.DimeRecord_StreamIsClosed);
			}
			if (this.m_ioMode != IOModeEnum.WriteOnly)
			{
				throw new InvalidOperationException(XmlaSR.DimeRecord_WriteNotAllowed);
			}
			if (this.m_chunked)
			{
				if (this.m_bytesReadWritten + count >= this.ChunkSize)
				{
					this.WriteChunkedPayload(false, false, buffer, offset, count);
					return;
				}
				if (this.m_bodyStreamBuffer == null)
				{
					this.m_bodyStreamBuffer = new MemoryStream((count < 512) ? 512 : count);
				}
				this.m_bodyStreamBuffer.Write(buffer, offset, count);
				this.m_bytesReadWritten += count;
				return;
			}
			else
			{
				if (this.m_bytesReadWritten + count > this.m_contentLength)
				{
					throw new Exception(XmlaSR.DimeRecord_ContentLengthExceeded);
				}
				if (!this.m_headerWritten)
				{
					this.WriteHeader(false, false, (long)this.m_contentLength);
					this.m_headerWritten = true;
				}
				this.m_stream.Write(buffer, offset, count);
				this.m_bytesReadWritten += count;
				if (this.m_bytesReadWritten == this.m_contentLength)
				{
					DimeRecord.WritePadding(this.m_stream, this.m_bytesReadWritten);
				}
				return;
			}
		}

		private static void WritePadding(Stream stream, int bytesWritten)
		{
			int num = DimeRecord.PaddedCount(bytesWritten) - bytesWritten;
			for (int i = 0; i < num; i++)
			{
				stream.WriteByte(0);
			}
		}

		private static void ReadPadding(Stream stream, int bytesRead)
		{
			int num = DimeRecord.PaddedCount(bytesRead) - bytesRead;
			for (int i = 0; i < num; i++)
			{
				stream.ReadByte();
			}
		}

		private void WriteChunkedPayload(bool endOfRecord, bool endOfMessage)
		{
			this.WriteChunkedPayload(endOfRecord, endOfMessage, null, 0, 0);
		}

		private void WriteChunkedPayload(bool endOfRecord, bool endOfMessage, byte[] bytes, int offset, int count)
		{
			byte[] array = null;
			int num = 0;
			if (this.m_bodyStreamBuffer != null && this.m_bodyStreamBuffer.Length > 0L)
			{
				array = this.m_bodyStreamBuffer.GetBuffer();
				this.m_bodyStreamBuffer = null;
				num = this.m_bytesReadWritten;
				this.m_bytesReadWritten = 0;
			}
			this.WriteHeader(endOfRecord, endOfMessage, (long)(count + num));
			if (array != null)
			{
				this.m_stream.Write(array, 0, num);
			}
			if (bytes != null)
			{
				this.m_stream.Write(bytes, offset, count);
			}
			DimeRecord.WritePadding(this.m_stream, count + num);
		}
	}
}
