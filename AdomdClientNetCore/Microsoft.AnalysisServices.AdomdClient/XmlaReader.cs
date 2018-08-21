using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class XmlaReader : XmlReader
	{
		private interface IWhiteSpaceHandlingRestorer : IDisposable
		{
			void Initialize(WhitespaceHandling handling);
		}

		private sealed class WhiteSpaceHandlingRestorer : XmlaReader.IWhiteSpaceHandlingRestorer, IDisposable
		{
			private WhitespaceHandling handling = WhitespaceHandling.None;

			private XmlTextReader baseTextReader;

			internal WhiteSpaceHandlingRestorer(XmlTextReader reader)
			{
				this.baseTextReader = reader;
			}

			public void Initialize(WhitespaceHandling handling)
			{
				this.handling = this.baseTextReader.WhitespaceHandling;
				if (this.baseTextReader != null && this.baseTextReader.ReadState != ReadState.Closed)
				{
					this.baseTextReader.WhitespaceHandling = handling;
				}
			}

			public void Dispose()
			{
				if (this.baseTextReader != null && this.baseTextReader.ReadState != ReadState.Closed)
				{
					this.baseTextReader.WhitespaceHandling = this.handling;
				}
				this.handling = WhitespaceHandling.None;
			}
		}

		private sealed class WhiteSpaceHandlingRestorerEmpty : XmlaReader.IWhiteSpaceHandlingRestorer, IDisposable
		{
			internal WhiteSpaceHandlingRestorerEmpty(XmlReader reader)
			{
			}

			public void Initialize(WhitespaceHandling handling)
			{
			}

			public void Dispose()
			{
			}
		}

		internal class ClientLocaleHelper : IDisposable
		{
			private readonly CultureInfo prevUICulture;

			internal static IDisposable SetupThreadUICultureToMatchXmlaRequest(XmlaClient xmlaClient)
			{
				return new XmlaReader.ClientLocaleHelper(xmlaClient);
			}

			private ClientLocaleHelper(XmlaClient client)
			{
				if (client == null || client.ConnectionInfo == null)
				{
					return;
				}
				object obj = client.ConnectionInfo.ExtendedProperties["LocaleIdentifier"];
				if (obj == null)
				{
					return;
				}
				string s = (string)obj;
				int num = XmlConvert.ToInt32(s);
				if (num == Thread.CurrentThread.CurrentUICulture.LCID)
				{
					return;
				}
				this.prevUICulture = Thread.CurrentThread.CurrentUICulture;
				Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(num);
			}

			public void Dispose()
			{
				if (this.prevUICulture == null)
				{
					return;
				}
				Thread.CurrentThread.CurrentUICulture = this.prevUICulture;
			}
		}

		private static string ReturnElement = "return";

		private static string ReturnElementNamespace = "urn:schemas-microsoft-com:xml-analysis";

		private static string SchemaElement = "schema";

		private XmlaClient client;

		private NamespacesMgr namespacesManager;

		private bool skipUnknownElements;

		private XmlReader xmlReader;

		private bool detached;

		private bool maskEndOfStream;

		private int topNodeDepth;

		private XmlaReader.IWhiteSpaceHandlingRestorer whiteSpaceHandlingRestorer;

		private bool hasExtendedErrorInfoBeenRead;

		private bool ShouldSkipUnknownElements
		{
			get
			{
				return this.skipUnknownElements && this.namespacesManager != null;
			}
		}

		internal bool MaskEndOfStream
		{
			get
			{
				return this.maskEndOfStream;
			}
			set
			{
				this.maskEndOfStream = value;
				if (this.maskEndOfStream)
				{
					this.topNodeDepth = this.Depth;
					return;
				}
				this.topNodeDepth = 0;
			}
		}

		internal bool SkipElements
		{
			get
			{
				return this.skipUnknownElements;
			}
			set
			{
				this.skipUnknownElements = value;
			}
		}

		internal bool HasExtendedErrorInfoBeenRead
		{
			get
			{
				return this.hasExtendedErrorInfoBeenRead;
			}
		}

		public override XmlReaderSettings Settings
		{
			get
			{
				XmlReaderSettings settings;
				try
				{
					settings = this.xmlReader.Settings;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return settings;
			}
		}

		public override IXmlSchemaInfo SchemaInfo
		{
			get
			{
				IXmlSchemaInfo schemaInfo;
				try
				{
					schemaInfo = this.xmlReader.SchemaInfo;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return schemaInfo;
			}
		}

		public override Type ValueType
		{
			get
			{
				Type valueType;
				try
				{
					valueType = this.xmlReader.ValueType;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return valueType;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				XmlNodeType nodeType;
				try
				{
					nodeType = this.xmlReader.NodeType;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return nodeType;
			}
		}

		public override string Name
		{
			get
			{
				string name;
				try
				{
					name = this.xmlReader.Name;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return name;
			}
		}

		public override string LocalName
		{
			get
			{
				string localName;
				try
				{
					localName = this.xmlReader.LocalName;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return localName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				string namespaceURI;
				try
				{
					namespaceURI = this.xmlReader.NamespaceURI;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return namespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				string prefix;
				try
				{
					prefix = this.xmlReader.Prefix;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				bool hasValue;
				try
				{
					hasValue = this.xmlReader.HasValue;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return hasValue;
			}
		}

		public override string Value
		{
			get
			{
				string value;
				try
				{
					value = this.xmlReader.Value;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return value;
			}
		}

		public override int Depth
		{
			get
			{
				int result;
				try
				{
					result = this.xmlReader.Depth - this.topNodeDepth;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return result;
			}
		}

		public override string BaseURI
		{
			get
			{
				string baseURI;
				try
				{
					baseURI = this.xmlReader.BaseURI;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return baseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				bool isEmptyElement;
				try
				{
					isEmptyElement = this.xmlReader.IsEmptyElement;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return isEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				bool isDefault;
				try
				{
					isDefault = this.xmlReader.IsDefault;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return isDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				char quoteChar;
				try
				{
					quoteChar = this.xmlReader.QuoteChar;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return quoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				XmlSpace xmlSpace;
				try
				{
					xmlSpace = this.xmlReader.XmlSpace;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return xmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				string xmlLang;
				try
				{
					xmlLang = this.xmlReader.XmlLang;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return xmlLang;
			}
		}

		public override int AttributeCount
		{
			get
			{
				int attributeCount;
				try
				{
					attributeCount = this.xmlReader.AttributeCount;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return attributeCount;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				bool canResolveEntity;
				try
				{
					canResolveEntity = this.xmlReader.CanResolveEntity;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return canResolveEntity;
			}
		}

		public override bool EOF
		{
			get
			{
				bool result;
				try
				{
					if (this.MaskEndOfStream)
					{
						result = (this.xmlReader.EOF || this.ReachedClosingReturn());
					}
					else
					{
						result = this.xmlReader.EOF;
					}
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return result;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				ReadState readState;
				try
				{
					if (this.MaskEndOfStream && this.ReachedClosingReturn())
					{
						this.SkipToTheEnd();
					}
					readState = this.xmlReader.ReadState;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return readState;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				bool hasAttributes;
				try
				{
					hasAttributes = this.xmlReader.HasAttributes;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return hasAttributes;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				XmlNameTable nameTable;
				try
				{
					nameTable = this.xmlReader.NameTable;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return nameTable;
			}
		}

		public override bool CanReadBinaryContent
		{
			get
			{
				bool canReadBinaryContent;
				try
				{
					canReadBinaryContent = this.xmlReader.CanReadBinaryContent;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return canReadBinaryContent;
			}
		}

		public override bool CanReadValueChunk
		{
			get
			{
				bool canReadValueChunk;
				try
				{
					canReadValueChunk = this.xmlReader.CanReadValueChunk;
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return canReadValueChunk;
			}
		}

		public override string this[int i]
		{
			get
			{
				string result;
				try
				{
					result = this.xmlReader[i];
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return result;
			}
		}

		public override string this[string name]
		{
			get
			{
				string result;
				try
				{
					result = this.xmlReader[name];
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return result;
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				string result;
				try
				{
					result = this.xmlReader[name, namespaceURI];
				}
				catch (Exception ex)
				{
					if ((ex = this.HandleException(ex)) != null)
					{
						throw ex;
					}
					throw;
				}
				return result;
			}
		}

		internal bool IsReaderDetached
		{
			get
			{
				return this.detached;
			}
		}

		internal XmlaReader(XmlReader baseReader, XmlaClient client, NamespacesMgr namespacesManager)
		{
			this.client = client;
			this.namespacesManager = namespacesManager;
			this.skipUnknownElements = true;
			this.xmlReader = baseReader;
			if (this.xmlReader is XmlTextReader)
			{
				this.whiteSpaceHandlingRestorer = new XmlaReader.WhiteSpaceHandlingRestorer((XmlTextReader)this.xmlReader);
				return;
			}
			this.whiteSpaceHandlingRestorer = new XmlaReader.WhiteSpaceHandlingRestorerEmpty(this.xmlReader);
		}

		internal XmlSchema ReadSchema()
		{
			XmlSchema result;
			try
			{
				XmlSchema xmlSchema = null;
				if (!this.ShouldSkipUnknownElements)
				{
					xmlSchema = XmlSchema.Read(this.xmlReader, null);
					this.xmlReader.ReadEndElement();
				}
				else
				{
					XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(this.NameTable);
					IXmlNamespaceResolver xmlNamespaceResolver = this.xmlReader as IXmlNamespaceResolver;
					if (xmlNamespaceResolver != null)
					{
						IDictionary<string, string> namespacesInScope = xmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml);
						IEnumerator<KeyValuePair<string, string>> enumerator = namespacesInScope.GetEnumerator();
						while (enumerator.MoveNext())
						{
							XmlNamespaceManager arg_75_0 = xmlNamespaceManager;
							KeyValuePair<string, string> current = enumerator.Current;
							string arg_75_1 = current.Key;
							KeyValuePair<string, string> current2 = enumerator.Current;
							arg_75_0.AddNamespace(arg_75_1, current2.Value);
						}
					}
					XmlParserContext inputContext = new XmlParserContext(this.NameTable, xmlNamespaceManager, this.XmlLang, this.XmlSpace);
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
					XmlReader xmlReader = XmlReader.Create(new StringReader(this.GetCleanedUpSchemaDefinition()), xmlReaderSettings, inputContext);
					try
					{
						xmlSchema = XmlSchema.Read(xmlReader, null);
					}
					finally
					{
						xmlReader.Close();
					}
				}
				result = xmlSchema;
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		internal string GetExtendedErrorInfo()
		{
			this.hasExtendedErrorInfoBeenRead = true;
			using (XmlaReader.ClientLocaleHelper.SetupThreadUICultureToMatchXmlaRequest(this.client))
			{
				if (this.client != null && this.client.xmlaStream != null)
				{
					return this.client.xmlaStream.GetExtendedErrorInfo();
				}
			}
			return string.Empty;
		}

		internal IDisposable GetWhitespaceHandlingRestorer(WhitespaceHandling handling)
		{
			this.whiteSpaceHandlingRestorer.Initialize(handling);
			return this.whiteSpaceHandlingRestorer;
		}

		public override string GetAttribute(string name)
		{
			string attribute;
			try
			{
				attribute = this.xmlReader.GetAttribute(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return attribute;
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			string attribute;
			try
			{
				attribute = this.xmlReader.GetAttribute(name, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return attribute;
		}

		public override string GetAttribute(int i)
		{
			string attribute;
			try
			{
				attribute = this.xmlReader.GetAttribute(i);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return attribute;
		}

		public override bool MoveToAttribute(string name)
		{
			bool result;
			try
			{
				result = this.xmlReader.MoveToAttribute(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			bool result;
			try
			{
				result = this.xmlReader.MoveToAttribute(name, ns);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override void MoveToAttribute(int i)
		{
			try
			{
				this.xmlReader.MoveToAttribute(i);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override bool MoveToFirstAttribute()
		{
			bool result;
			try
			{
				result = this.xmlReader.MoveToFirstAttribute();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool MoveToNextAttribute()
		{
			bool result;
			try
			{
				result = this.xmlReader.MoveToNextAttribute();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool MoveToElement()
		{
			bool result;
			try
			{
				result = this.xmlReader.MoveToElement();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool Read()
		{
			bool result;
			try
			{
				if (this.MaskEndOfStream)
				{
					bool flag = this.xmlReader.Read();
					if (this.ReachedClosingReturn())
					{
						this.SkipToTheEnd();
						flag = false;
					}
					result = flag;
				}
				else
				{
					result = this.xmlReader.Read();
				}
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override void Close()
		{
			if (this.xmlReader == null)
			{
				return;
			}
			if (this.IsReaderDetached)
			{
				try
				{
					try
					{
						this.ReturnReader(true);
					}
					catch (Exception ex)
					{
						if ((ex = this.HandleException(ex)) != null)
						{
							throw ex;
						}
						throw;
					}
					return;
				}
				finally
				{
					if (this.xmlReader != null)
					{
						this.xmlReader.Close();
					}
				}
			}
			this.xmlReader.Close();
		}

		internal void CloseWithoutEndReceival()
		{
			XmlReader xmlReader = this.xmlReader;
			if (xmlReader != null)
			{
				xmlReader.Close();
			}
		}

		public override void Skip()
		{
			try
			{
				this.xmlReader.Skip();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override string ReadString()
		{
			string result;
			try
			{
				result = this.xmlReader.ReadString();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override XmlNodeType MoveToContent()
		{
			XmlNodeType result;
			try
			{
				if (this.ShouldSkipUnknownElements)
				{
					while (this.xmlReader.MoveToContent() == XmlNodeType.Element)
					{
						if (!this.namespacesManager.IsNamespaceSkippable(this.NamespaceURI))
						{
							result = this.NodeType;
							return result;
						}
						this.xmlReader.Skip();
					}
					result = this.NodeType;
				}
				else
				{
					result = this.xmlReader.MoveToContent();
				}
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override void ReadStartElement()
		{
			try
			{
				this.xmlReader.ReadStartElement();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override void ReadStartElement(string name)
		{
			try
			{
				this.xmlReader.ReadStartElement(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override void ReadStartElement(string localname, string ns)
		{
			try
			{
				this.xmlReader.ReadStartElement(localname, ns);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override string ReadElementString()
		{
			string result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.ReadElementString();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadElementString(string name)
		{
			string result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.ReadElementString(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadElementString(string localname, string ns)
		{
			string result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.ReadElementString(localname, ns);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override void ReadEndElement()
		{
			try
			{
				this.MoveToContent();
				this.xmlReader.ReadEndElement();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override bool IsStartElement()
		{
			bool result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.IsStartElement();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool IsStartElement(string name)
		{
			bool result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.IsStartElement(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool IsStartElement(string localname, string ns)
		{
			bool result;
			try
			{
				this.MoveToContent();
				result = this.xmlReader.IsStartElement(localname, ns);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadInnerXml()
		{
			string result;
			try
			{
				result = this.xmlReader.ReadInnerXml();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadOuterXml()
		{
			string result;
			try
			{
				result = this.xmlReader.ReadOuterXml();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string LookupNamespace(string prefix)
		{
			string result;
			try
			{
				result = this.xmlReader.LookupNamespace(prefix);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override void ResolveEntity()
		{
			try
			{
				this.xmlReader.ResolveEntity();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
		}

		public override bool ReadAttributeValue()
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadAttributeValue();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override DateTime ReadContentAsDateTime()
		{
			DateTime result;
			try
			{
				result = this.xmlReader.ReadContentAsDateTime();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override double ReadContentAsDouble()
		{
			double result;
			try
			{
				result = this.xmlReader.ReadContentAsDouble();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadContentAsInt()
		{
			int result;
			try
			{
				result = this.xmlReader.ReadContentAsInt();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override long ReadContentAsLong()
		{
			long result;
			try
			{
				result = this.xmlReader.ReadContentAsLong();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadContentAsObject()
		{
			object result;
			try
			{
				result = this.xmlReader.ReadContentAsObject();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadContentAs(Type type, IXmlNamespaceResolver resolver)
		{
			object result;
			try
			{
				result = this.xmlReader.ReadContentAs(type, resolver);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadContentAsBase64(buffer, index, count);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadContentAsBinHex(buffer, index, count);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
		{
			object result;
			try
			{
				result = this.xmlReader.ReadElementContentAs(returnType, namespaceResolver);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
		{
			object result;
			try
			{
				result = this.xmlReader.ReadElementContentAs(returnType, namespaceResolver, localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadElementContentAsBase64(byte[] buffer, int index, int count)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadElementContentAsBase64(buffer, index, count);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadElementContentAsBinHex(buffer, index, count);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadElementContentAsBoolean()
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadElementContentAsBoolean();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadElementContentAsBoolean(string localName, string namespaceURI)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadElementContentAsBoolean(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override DateTime ReadElementContentAsDateTime()
		{
			DateTime result;
			try
			{
				result = this.xmlReader.ReadElementContentAsDateTime();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
		{
			DateTime result;
			try
			{
				result = this.xmlReader.ReadElementContentAsDateTime(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override double ReadElementContentAsDouble()
		{
			double result;
			try
			{
				result = this.xmlReader.ReadElementContentAsDouble();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override double ReadElementContentAsDouble(string localName, string namespaceURI)
		{
			double result;
			try
			{
				result = this.xmlReader.ReadElementContentAsDouble(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadElementContentAsInt()
		{
			int result;
			try
			{
				result = this.xmlReader.ReadElementContentAsInt();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadElementContentAsInt(string localName, string namespaceURI)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadElementContentAsInt(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override long ReadElementContentAsLong()
		{
			long result;
			try
			{
				result = this.xmlReader.ReadElementContentAsLong();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override long ReadElementContentAsLong(string localName, string namespaceURI)
		{
			long result;
			try
			{
				result = this.xmlReader.ReadElementContentAsLong(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadElementContentAsObject()
		{
			object result;
			try
			{
				result = this.xmlReader.ReadElementContentAsObject();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override object ReadElementContentAsObject(string localName, string namespaceURI)
		{
			object result;
			try
			{
				result = this.xmlReader.ReadElementContentAsObject(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadElementContentAsString()
		{
			string result;
			try
			{
				result = this.xmlReader.ReadElementContentAsString();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override string ReadElementContentAsString(string localName, string namespaceURI)
		{
			string result;
			try
			{
				result = this.xmlReader.ReadElementContentAsString(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToFollowing(string name)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToFollowing(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToFollowing(string localName, string namespaceURI)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToFollowing(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToNextSibling(string name)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToNextSibling(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToNextSibling(string localName, string namespaceURI)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToNextSibling(localName, namespaceURI);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToDescendant(string name)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToDescendant(name);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override bool ReadToDescendant(string localName, string ns)
		{
			bool result;
			try
			{
				result = this.xmlReader.ReadToDescendant(localName, ns);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override XmlReader ReadSubtree()
		{
			XmlReader result;
			try
			{
				result = this.xmlReader.ReadSubtree();
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			int result;
			try
			{
				result = this.xmlReader.ReadValueChunk(buffer, index, count);
			}
			catch (Exception ex)
			{
				if ((ex = this.HandleException(ex)) != null)
				{
					throw ex;
				}
				throw;
			}
			return result;
		}

		internal void DetachReader()
		{
			if (!this.IsReaderDetached)
			{
				this.detached = true;
			}
		}

		private void ReturnReader(bool callEndReceival)
		{
			if (this.IsReaderDetached)
			{
				this.detached = false;
				if (this.ReadState != ReadState.Closed && callEndReceival && this.client != null)
				{
					if (this.client.IsConnected)
					{
						this.client.EndReceival(false);
					}
					this.client = null;
				}
			}
		}

		private Exception HandleException(Exception ex)
		{
			if (!this.IsReaderDetached || ex is XmlException || ex is XmlSchemaException || ex is AdomdUnknownResponseException)
			{
				return null;
			}
			try
			{
				if (this.client != null)
				{
					this.ReturnReader(false);
					if (this.xmlReader != null)
					{
						this.xmlReader.Close();
					}
					this.client.Disconnect(false);
					this.client = null;
				}
			}
			finally
			{
				if (this.xmlReader != null)
				{
					this.xmlReader.Close();
				}
				this.client = null;
			}
			if (!(ex is IOException))
			{
				return null;
			}
			return new AdomdConnectionException(XmlaSR.ConnectionBroken, ex);
		}

		private bool ReachedClosingReturn()
		{
			return !this.xmlReader.EOF && this.NodeType == XmlNodeType.EndElement && this.LocalName == XmlaReader.ReturnElement && this.LookupNamespace(this.Prefix) == XmlaReader.ReturnElementNamespace;
		}

		private void SkipToTheEnd()
		{
			while (!this.xmlReader.EOF)
			{
				this.xmlReader.Read();
			}
		}

		private string GetCleanedUpSchemaDefinition()
		{
			if (this.IsStartElement(XmlaReader.SchemaElement, "http://www.w3.org/2001/XMLSchema"))
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				try
				{
					xmlTextWriter.QuoteChar = this.QuoteChar;
					int depth = this.Depth;
					xmlTextWriter.WriteStartElement(this.xmlReader.Prefix, this.xmlReader.LocalName, this.xmlReader.NamespaceURI);
					xmlTextWriter.WriteAttributes(this, true);
					if (this.xmlReader.IsEmptyElement)
					{
						xmlTextWriter.WriteEndElement();
					}
					else
					{
						bool flag = !this.xmlReader.Read();
						while (!flag && this.xmlReader.Depth > depth)
						{
							XmlNodeType nodeType = this.xmlReader.NodeType;
							switch (nodeType)
							{
							case XmlNodeType.Element:
								if (this.namespacesManager.IsNamespaceSkippable(this.xmlReader.NamespaceURI))
								{
									this.xmlReader.Skip();
									flag = this.EOF;
									continue;
								}
								xmlTextWriter.WriteStartElement(this.xmlReader.Prefix, this.xmlReader.LocalName, this.xmlReader.NamespaceURI);
								xmlTextWriter.WriteAttributes(this, true);
								if (this.xmlReader.IsEmptyElement)
								{
									xmlTextWriter.WriteEndElement();
								}
								break;
							case XmlNodeType.Attribute:
							case XmlNodeType.CDATA:
								break;
							case XmlNodeType.Text:
								xmlTextWriter.WriteString(this.xmlReader.Value);
								break;
							case XmlNodeType.EntityReference:
								xmlTextWriter.WriteEntityRef(this.xmlReader.Name);
								break;
							default:
								if (nodeType == XmlNodeType.EndElement)
								{
									xmlTextWriter.WriteFullEndElement();
								}
								break;
							}
							flag = !this.xmlReader.Read();
						}
						if (depth == this.xmlReader.Depth && this.xmlReader.NodeType == XmlNodeType.EndElement)
						{
							xmlTextWriter.WriteFullEndElement();
							this.xmlReader.Read();
						}
					}
				}
				finally
				{
					if (xmlTextWriter != null)
					{
						xmlTextWriter.Close();
					}
				}
				return stringWriter.ToString();
			}
			throw new AdomdUnknownResponseException(XmlaSR.MissingElement(XmlaReader.SchemaElement, "http://www.w3.org/2001/XMLSchema"), "");
		}
	}
}
