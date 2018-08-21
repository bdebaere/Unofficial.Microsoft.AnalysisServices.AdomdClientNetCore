using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class TcpStream : TransportCapabilitiesAwareXmlaStream
	{
		private const int BufferSizeForSkip = 8192;

		protected internal static readonly TraceSwitch TRACESWITCH = new TraceSwitch(typeof(TcpStream).FullName, typeof(TcpStream).FullName, TraceLevel.Off.ToString());

		private static byte[] BufferForSkip = new byte[8192];

		private DimeWriter dimeWriter;

		private DimeReader dimeReader;

		private DimeRecord dimeRecordForWrite;

		private DimeRecord dimeRecordForRead;

		private bool endOfStream = true;

		private int dimeChunkSize;

		private BufferedStream bufferedStream;

		internal TcpStream(BufferedStream bufferedStream, int packetSize, DataType desiredRequestType, DataType desiredResponseType) : base(desiredRequestType, desiredResponseType)
		{
			this.bufferedStream = bufferedStream;
			this.dimeChunkSize = packetSize;
		}

		protected TcpStream(TcpStream originalTcpStream) : base(originalTcpStream)
		{
			this.bufferedStream = originalTcpStream.bufferedStream;
			this.dimeChunkSize = originalTcpStream.dimeChunkSize;
		}

		public override DataType GetResponseDataType()
		{
			DataType result;
			try
			{
				if (this.endOfStream)
				{
					result = DataType.Undetermined;
				}
				else
				{
					if (this.dimeReader == null)
					{
						this.dimeReader = new DimeReader(this.bufferedStream);
						this.dimeRecordForRead = this.dimeReader.ReadRecord();
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace = new StackTrace();
							stackTrace.GetFrame(1).GetMethod();
						}
						this.DetermineNegotiatedOptions();
					}
					if (this.dimeRecordForRead == null)
					{
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace2 = new StackTrace();
							stackTrace2.GetFrame(1).GetMethod();
						}
						this.dimeReader.Close();
						this.dimeReader = null;
						this.endOfStream = true;
						result = DataType.Undetermined;
					}
					else
					{
						DataType dataTypeFromString = DataTypes.GetDataTypeFromString(this.dimeRecordForRead.Type);
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace3 = new StackTrace();
							stackTrace3.GetFrame(1).GetMethod();
						}
						if (dataTypeFromString == DataType.Unknown)
						{
							throw new AdomdUnknownResponseException(XmlaSR.Dime_DataTypeNotSupported(this.dimeRecordForRead.Type), "");
						}
						result = dataTypeFromString;
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
			catch (SocketException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			return result;
		}

		public override void Close()
		{
		}

		public override void Dispose()
		{
			try
			{
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
			if (TcpStream.TRACESWITCH.TraceVerbose)
			{
				StackTrace stackTrace = new StackTrace();
				stackTrace.GetFrame(1).GetMethod();
			}
			try
			{
				if (this.dimeWriter == null)
				{
					throw new InvalidOperationException();
				}
				this.dimeWriter.Close();
				this.dimeWriter = null;
				this.endOfStream = false;
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
			try
			{
				if (size > 0)
				{
					if (this.dimeWriter == null)
					{
						this.dimeWriter = new DimeWriter(this.bufferedStream);
						this.dimeWriter.DefaultChunkSize = this.dimeChunkSize;
						this.dimeWriter.Options = base.GetTransportCapabilities();
						this.dimeRecordForWrite = this.dimeWriter.CreateRecord(null, DataTypes.GetDataTypeFromEnum(this.GetRequestDataType()), TypeFormatEnum.MediaType, -1);
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace = new StackTrace();
							stackTrace.GetFrame(1).GetMethod();
						}
					}
					this.dimeRecordForWrite.WriteBody(buffer, offset, size);
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
			int result;
			try
			{
				if (size == 0 || this.endOfStream)
				{
					result = 0;
				}
				else
				{
					if (this.dimeReader == null)
					{
						this.dimeReader = new DimeReader(this.bufferedStream);
						this.dimeRecordForRead = this.dimeReader.ReadRecord();
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace = new StackTrace();
							stackTrace.GetFrame(1).GetMethod();
						}
					}
					if (this.dimeRecordForRead == null)
					{
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace2 = new StackTrace();
							stackTrace2.GetFrame(1).GetMethod();
						}
						this.dimeReader.Close();
						this.dimeReader = null;
						this.endOfStream = true;
						result = 0;
					}
					else
					{
						int num = this.dimeRecordForRead.ReadBody(buffer, offset, size);
						if (TcpStream.TRACESWITCH.TraceVerbose)
						{
							StackTrace stackTrace3 = new StackTrace();
							stackTrace3.GetFrame(1).GetMethod();
						}
						if (num == 0)
						{
							this.dimeRecordForRead.Close();
							this.dimeReader.Close();
							this.dimeRecordForRead = null;
							this.dimeReader = null;
							this.endOfStream = true;
						}
						result = num;
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
			catch (SocketException innerException2)
			{
				throw new XmlaStreamException(innerException2);
			}
			return result;
		}

		public override void Skip()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			if (this.dimeReader != null)
			{
				while (0 < this.Read(TcpStream.BufferForSkip, 0, 8192))
				{
				}
			}
		}

		public override void Flush()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(null);
			}
			try
			{
				this.bufferedStream.Flush();
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
		}

		protected override void DetermineNegotiatedOptions()
		{
			if (!base.NegotiatedOptions)
			{
				TransportCapabilities transportCapabilities = this.dimeReader.Options.Clone();
				transportCapabilities.ContentTypeNegotiated = true;
				base.SetTransportCapabilities(transportCapabilities);
			}
		}
	}
}
