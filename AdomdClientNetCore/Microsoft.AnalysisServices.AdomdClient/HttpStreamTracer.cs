using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class HttpStreamTracer : IDisposable
	{
		private FileStream requestFileStream;

		private FileStream responseFileStream;

		private bool requestHeaderSetOnce;

		private bool responseHeaderSetOnce;

		private bool compressedResponse;

		private int serial;

		private string baseFolder;

		private HttpStream baseHttpStream;

		private CompressedStream compressedStream;

		private bool disposed;

		internal HttpStreamTracer(HttpStream baseHttpStream)
		{
			this.baseHttpStream = baseHttpStream;
			this.requestFileStream = null;
			this.responseFileStream = null;
			this.PrepareHttpTraceFolder();
		}

		~HttpStreamTracer()
		{
			this.Dispose(false);
		}

		internal void CreateHttpTraceRequestFile()
		{
			this.CreateHttpTraceFile(true);
		}

		internal void CreateHttpTraceResponseFile()
		{
			this.CreateHttpTraceFile(false);
		}

		internal void WriteHttpTraceResponse(byte[] buffer, int offset, int size)
		{
			try
			{
				if (size != 0 && this.baseHttpStream.HttpStreamResponse != null)
				{
					if (this.responseFileStream == null)
					{
						this.CreateHttpTraceFile(false);
					}
					if (!this.responseHeaderSetOnce)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Status Code : {0}", new object[]
						{
							this.baseHttpStream.HttpStreamResponse.StatusCode.ToString()
						}));
						foreach (string text in this.baseHttpStream.HttpStreamResponse.Headers.Keys)
						{
							string text2 = this.baseHttpStream.HttpStreamResponse.Headers[text];
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
							{
								text,
								text2
							}));
							if (!this.compressedResponse && !string.IsNullOrEmpty(text2) && text.Contains("Content-Type") && text2.IndexOf("xpress", StringComparison.OrdinalIgnoreCase) != -1)
							{
								this.compressedResponse = true;
							}
						}
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
						this.responseFileStream.Write(bytes, 0, bytes.Length);
						this.responseHeaderSetOnce = true;
					}
					if (this.responseFileStream != null & size > 0)
					{
						if (this.compressedResponse)
						{
							this.compressedStream.BaseXmlaStream.Write(buffer, offset, size);
						}
						else
						{
							this.responseFileStream.Write(buffer, offset, size);
						}
						this.responseFileStream.Flush();
					}
				}
			}
			catch
			{
			}
		}

		internal void WriteHttpTraceError(WebException webException)
		{
			try
			{
				if (this.responseFileStream == null)
				{
					this.CreateHttpTraceFile(false);
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (webException.Response == null)
				{
					stringBuilder.AppendLine("Error occurred: " + webException.Message);
				}
				else
				{
					HttpWebResponse httpWebResponse = webException.Response as HttpWebResponse;
					if (httpWebResponse != null)
					{
						stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "Status Code : {0}", new object[]
						{
							httpWebResponse.StatusCode
						}));
						foreach (string text in httpWebResponse.Headers.Keys)
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
							{
								text,
								httpWebResponse.Headers[text]
							}));
						}
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						stringBuilder.AppendLine("Response Uri:" + httpWebResponse.ResponseUri.ToString());
						stringBuilder.AppendLine("Status Description:" + httpWebResponse.StatusDescription);
						stringBuilder.AppendLine("Message:" + webException.Message);
						if (webException.InnerException != null)
						{
							stringBuilder.AppendLine("Inner Exception:" + webException.InnerException.Message);
						}
					}
				}
				if (this.responseFileStream != null)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
					this.responseFileStream.Write(bytes, 0, bytes.Length);
					this.responseFileStream.Flush();
				}
			}
			catch
			{
			}
		}

		internal void WriteHttpTraceRequest(byte[] buffer, int offset, int size)
		{
			try
			{
				if (size != 0 && this.baseHttpStream.HttpStreamRequest != null)
				{
					if (this.requestFileStream == null)
					{
						this.CreateHttpTraceFile(true);
					}
					if (!this.requestHeaderSetOnce)
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (string text in this.baseHttpStream.HttpStreamRequest.Headers.Keys)
						{
							stringBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0} : {1}", new object[]
							{
								text,
								this.baseHttpStream.HttpStreamRequest.Headers[text]
							}));
						}
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
						this.requestFileStream.Write(bytes, 0, bytes.Length);
						this.requestHeaderSetOnce = true;
					}
					if (this.requestFileStream != null && size > 0)
					{
						this.requestFileStream.Write(buffer, offset, size);
						this.requestFileStream.Flush();
					}
				}
			}
			catch
			{
			}
		}

		internal void CloseHttpTraceFiles()
		{
			this.CloseHttpTraceRequestFile();
			this.CloseHttpTraceResponseFile();
		}

		internal void CloseHttpTraceRequestFile()
		{
			this.CloseHttpTraceFile(true);
		}

		internal void CloseHttpTraceResponseFile()
		{
			this.CloseHttpTraceFile(false);
		}

		internal void PrepareHttpTraceFolder()
		{
			try
			{
				this.baseFolder = Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "XMLA_HTTP_Debug");
				string path = string.Format(CultureInfo.InvariantCulture, "{0:yyyy_MM_dd_HH_mm_ss_fff}-{1}", new object[]
				{
					DateTime.Now,
					Guid.NewGuid()
				});
				this.baseFolder = Path.Combine(this.baseFolder, path);
				if (!Directory.Exists(this.baseFolder))
				{
					Directory.CreateDirectory(this.baseFolder);
				}
			}
			catch
			{
			}
		}

		private void DecompressResponseAndWrite()
		{
			if (!this.compressedResponse)
			{
				return;
			}
			try
			{
				int num = Convert.ToInt32(this.compressedStream.BaseXmlaStream.Length);
				byte[] buffer = new byte[num];
				this.compressedStream.BaseXmlaStream.Seek(0L, SeekOrigin.Begin);
				int num2;
				do
				{
					num2 = this.compressedStream.Read(buffer, 0, num);
					if (num2 != 0)
					{
						this.responseFileStream.Write(buffer, 0, num2);
					}
				}
				while (num2 == num);
			}
			catch
			{
			}
		}

		private void CreateHttpTraceFile(bool request)
		{
			this.CloseHttpTraceFile(request);
			if (request)
			{
				this.requestHeaderSetOnce = false;
			}
			else
			{
				this.responseHeaderSetOnce = false;
				this.compressedResponse = false;
			}
			try
			{
				if (request)
				{
					string path = Path.Combine(this.baseFolder, string.Format(CultureInfo.InvariantCulture, "req_{0}.txt", new object[]
					{
						++this.serial
					}));
					this.requestFileStream = File.Open(path, FileMode.CreateNew, FileAccess.Write);
				}
				else
				{
					string path2 = Path.Combine(this.baseFolder, string.Format(CultureInfo.InvariantCulture, "res_{0}.txt", new object[]
					{
						this.serial
					}));
					this.responseFileStream = File.Open(path2, FileMode.Append, FileAccess.Write);
					if (this.compressedStream != null)
					{
						this.compressedStream.Dispose();
					}
					this.compressedStream = new CompressedStream(new MemoryXmlaStream(DataType.CompressedXml), 1);
				}
			}
			catch
			{
			}
		}

		private void CloseHttpTraceFile(bool request)
		{
			try
			{
				if (request)
				{
					if (this.requestFileStream != null)
					{
						this.requestFileStream.Close();
						this.requestFileStream.Dispose();
						this.requestFileStream = null;
					}
				}
				else if (this.responseFileStream != null)
				{
					this.DecompressResponseAndWrite();
					this.responseFileStream.Close();
					this.responseFileStream.Dispose();
					this.responseFileStream = null;
				}
			}
			catch
			{
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				try
				{
					if (this.compressedStream != null)
					{
						this.compressedStream.Dispose();
					}
					if (this.requestFileStream != null)
					{
						this.requestFileStream.Dispose();
					}
					if (this.responseFileStream != null)
					{
						this.responseFileStream.Dispose();
					}
				}
				catch
				{
				}
			}
			this.disposed = true;
		}
	}
}
