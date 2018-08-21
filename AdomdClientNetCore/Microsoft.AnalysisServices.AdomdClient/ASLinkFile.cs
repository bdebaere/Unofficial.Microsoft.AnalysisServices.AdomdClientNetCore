using Microsoft.AnalysisServices.AdomdClient.Internal.SPClient.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class ASLinkFile
	{
		private sealed class NonClosingStreamWrapper : Stream
		{
			private Stream innerStream;

			public override bool CanRead
			{
				get
				{
					return this.innerStream.CanRead;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return this.innerStream.CanSeek;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return this.innerStream.CanWrite;
				}
			}

			public override long Length
			{
				get
				{
					return this.innerStream.Length;
				}
			}

			public override long Position
			{
				get
				{
					return this.innerStream.Position;
				}
				set
				{
					this.innerStream.Position = value;
				}
			}

			public NonClosingStreamWrapper(Stream innerStream)
			{
				this.innerStream = innerStream;
			}

			protected override void Dispose(bool disposing)
			{
			}

			public override void Flush()
			{
				this.innerStream.Flush();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return this.innerStream.Read(buffer, offset, count);
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				return this.innerStream.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				this.innerStream.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				this.innerStream.Write(buffer, offset, count);
			}
		}

		private static class Schema
		{
			public static class Elements
			{
				public static class Database
				{
					public const string Name = "Database";
				}

				public static class Description
				{
					public const string Name = "Description";
				}

				public static class Root
				{
					public static class Attributes
					{
						public static class AllowDelegation
						{
							public const string Name = "allowDelegation";
						}
					}

					public const string Name = "ASLinkFile";
				}

				public static class Server
				{
					public const string Name = "Server";
				}
			}

			public const string XmlVersion = "1.0";

			public const string Namespace = "http://schemas.microsoft.com/analysisservices/linkfile";

			public const string NamespacePrefix = "lnk";

			public const string FullSchema = "<?xml version='1.0' encoding='utf-8'?> \r\n<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://schemas.microsoft.com/analysisservices/linkfile' elementFormDefault='qualified'>\r\n  <xs:element name='ASLinkFile'>\r\n    <xs:complexType>\r\n      <xs:all>\r\n        <xs:element name='Server' type='xs:string'/> \r\n        <xs:element name='Database' type='xs:string' />\r\n        <xs:element name='Description' type='xs:string' minOccurs='0'/>\r\n      </xs:all>\r\n      <xs:attribute name='allowDelegation' type='xs:boolean' default='false'/>\r\n    </xs:complexType>\r\n  </xs:element>\r\n</xs:schema>";
		}

		public const int MaxLinkFileSize = 4096;

		private string database;

		private string description;

		private bool isDelegationAllowed;

		private bool isInFarm;

		private XmlSchemaException malformedFileException;

		private string server;

		public string Database
		{
			get
			{
				return this.database;
			}
			set
			{
				this.database = value;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public bool IsDelegationAllowed
		{
			get
			{
				return this.isDelegationAllowed;
			}
			set
			{
				this.isDelegationAllowed = value;
			}
		}

		public bool IsFileMalformed
		{
			get
			{
				return this.malformedFileException != null;
			}
		}

		public bool IsInFarm
		{
			get
			{
				return this.isInFarm;
			}
		}

		public static ASLinkFile NotInFarm
		{
			get
			{
				return new ASLinkFile(false, null)
				{
					Database = null,
					Description = null,
					IsDelegationAllowed = false,
					Server = null
				};
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		public ASLinkFile() : this(true, null)
		{
		}

		private ASLinkFile(bool isInFarm, XmlSchemaException malformedFileException)
		{
			this.isInFarm = isInFarm;
			this.malformedFileException = malformedFileException;
		}

		public ASLinkFile(ILinkFile linkFile)
		{
			this.database = linkFile.Database;
			this.description = linkFile.Description;
			this.isDelegationAllowed = linkFile.IsDelegationAllowed;
			this.isInFarm = linkFile.IsInFarm;
			this.malformedFileException = (linkFile.IsFileMalformed ? new XmlSchemaException() : null);
			this.server = linkFile.Server;
		}

		public static ASLinkFile LoadFromStream(Stream stream)
		{
			return ASLinkFile.LoadFromStream(stream, false);
		}

		public static ASLinkFile LoadFromStream(Stream stream, bool throwOnValidationError)
		{
			ASLinkFile result;
			try
			{
				using (ASLinkFile.NonClosingStreamWrapper nonClosingStreamWrapper = new ASLinkFile.NonClosingStreamWrapper(stream))
				{
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					xmlReaderSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
					xmlReaderSettings.DtdProcessing = DtdProcessing.Prohibit;
					xmlReaderSettings.MaxCharactersInDocument = 4096L;
					xmlReaderSettings.Schemas.Add("http://schemas.microsoft.com/analysisservices/linkfile", new XmlTextReader("<?xml version='1.0' encoding='utf-8'?> \r\n<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://schemas.microsoft.com/analysisservices/linkfile' elementFormDefault='qualified'>\r\n  <xs:element name='ASLinkFile'>\r\n    <xs:complexType>\r\n      <xs:all>\r\n        <xs:element name='Server' type='xs:string'/> \r\n        <xs:element name='Database' type='xs:string' />\r\n        <xs:element name='Description' type='xs:string' minOccurs='0'/>\r\n      </xs:all>\r\n      <xs:attribute name='allowDelegation' type='xs:boolean' default='false'/>\r\n    </xs:complexType>\r\n  </xs:element>\r\n</xs:schema>", XmlNodeType.Document, null));
					xmlReaderSettings.ValidationEventHandler += delegate(object sender, ValidationEventArgs args)
					{
						throw args.Exception;
					};
					using (XmlReader xmlReader = XmlReader.Create(nonClosingStreamWrapper, xmlReaderSettings))
					{
						result = ASLinkFile.LoadFromXmlReader(xmlReader);
					}
				}
			}
			catch (XmlSchemaException ex)
			{
				if (throwOnValidationError)
				{
					throw;
				}
				result = ASLinkFile.CreateMalformed(ex);
			}
			return result;
		}

		private static ASLinkFile LoadFromXmlReader(XmlReader linkFileReader)
		{
			ASLinkFile aSLinkFile = new ASLinkFile();
			while (linkFileReader.Read())
			{
				if (linkFileReader.NodeType == XmlNodeType.Element && linkFileReader.NamespaceURI.Equals("http://schemas.microsoft.com/analysisservices/linkfile"))
				{
					if (linkFileReader.LocalName.Equals("ASLinkFile"))
					{
						if (linkFileReader.MoveToAttribute("allowDelegation"))
						{
							aSLinkFile.IsDelegationAllowed = linkFileReader.ReadContentAsBoolean();
						}
					}
					else if (linkFileReader.LocalName.Equals("Server"))
					{
						aSLinkFile.Server = linkFileReader.ReadString();
					}
					else if (linkFileReader.LocalName.Equals("Database"))
					{
						aSLinkFile.Database = linkFileReader.ReadString();
					}
					else if (linkFileReader.LocalName.Equals("Description"))
					{
						aSLinkFile.Description = linkFileReader.ReadString();
					}
				}
			}
			return aSLinkFile;
		}

		internal static ASLinkFile CreateMalformed(XmlSchemaException malformedFileException)
		{
			return new ASLinkFile(true, malformedFileException)
			{
				Database = null,
				Description = null,
				IsDelegationAllowed = false,
				Server = null
			};
		}

		public void SaveToStream(Stream stream)
		{
			using (ASLinkFile.NonClosingStreamWrapper nonClosingStreamWrapper = new ASLinkFile.NonClosingStreamWrapper(stream))
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(nonClosingStreamWrapper, Encoding.UTF8))
				{
					XmlDocument xmlDocument = this.CreateXmlDocument();
					xmlDocument.Save(xmlTextWriter);
				}
			}
		}

		private XmlDocument CreateXmlDocument()
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", null, null);
			xmlDocument.AppendChild(newChild);
			XmlElement xmlElement = xmlDocument.CreateElement("ASLinkFile", "http://schemas.microsoft.com/analysisservices/linkfile");
			xmlDocument.AppendChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("Server", "http://schemas.microsoft.com/analysisservices/linkfile");
			xmlElement2.InnerText = this.server;
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xmlDocument.CreateElement("Database", "http://schemas.microsoft.com/analysisservices/linkfile");
			xmlElement3.InnerText = this.database;
			xmlElement.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlDocument.CreateElement("Description", "http://schemas.microsoft.com/analysisservices/linkfile");
			if (!string.IsNullOrEmpty(this.description))
			{
				xmlElement4.InnerText = this.description;
			}
			xmlElement.AppendChild(xmlElement4);
			xmlElement.SetAttribute("allowDelegation", null, this.IsDelegationAllowed.ToString().ToLowerInvariant());
			return xmlDocument;
		}
	}
}
