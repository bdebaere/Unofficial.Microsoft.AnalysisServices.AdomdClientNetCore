using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class ColumnXmlReader : XmlReader
	{
		private XmlReader srcReader;

		private string strXml;

		private string strColumnName;

		private string strColumnNamespace;

		private bool isDelegate;

		private int startDepth;

		private bool isClosed;

		private bool isDataSet;

		private bool originalSkipElements = true;

		internal bool IsDataSet
		{
			get
			{
				return this.isDataSet;
			}
		}

		internal DataSet Dataset
		{
			get
			{
				DataSet dataSet = null;
				if (this.IsDataSet)
				{
					dataSet = new DataSet();
					dataSet.Locale = CultureInfo.InvariantCulture;
					dataSet.ReadXml(this, XmlReadMode.Auto);
				}
				return dataSet;
			}
		}

		public override int AttributeCount
		{
			get
			{
				if (this.EOF)
				{
					return 0;
				}
				return this.srcReader.AttributeCount;
			}
		}

		public override string BaseURI
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.BaseURI;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return !this.EOF && this.srcReader.CanResolveEntity;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return !this.EOF && this.srcReader.HasAttributes;
			}
		}

		public override bool HasValue
		{
			get
			{
				return !this.EOF && this.srcReader.HasValue;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return !this.EOF && this.srcReader.IsDefault;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return !this.EOF && this.srcReader.IsEmptyElement;
			}
		}

		public override string LocalName
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.LocalName;
			}
		}

		public override string Name
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.Name;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.NamespaceURI;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				if (this.EOF)
				{
					return null;
				}
				return this.srcReader.NameTable;
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				if (this.EOF)
				{
					return XmlNodeType.None;
				}
				return this.srcReader.NodeType;
			}
		}

		public override string Prefix
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.Prefix;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.srcReader.QuoteChar;
			}
		}

		public override string this[int i]
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader[i];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader[name, namespaceURI];
			}
		}

		public override string this[string name]
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader[name];
			}
		}

		public override string Value
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.Value;
			}
		}

		public override string XmlLang
		{
			get
			{
				if (this.EOF)
				{
					return "";
				}
				return this.srcReader.XmlLang;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				if (this.EOF)
				{
					return XmlSpace.None;
				}
				return this.srcReader.XmlSpace;
			}
		}

		public override int Depth
		{
			get
			{
				return this.srcReader.Depth - this.startDepth;
			}
		}

		public override bool EOF
		{
			get
			{
				if (this.srcReader.EOF)
				{
					this.startDepth = this.srcReader.Depth;
				}
				bool flag = this.Depth < 0;
				if (flag)
				{
					this.Close();
				}
				return flag;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				if (!this.EOF)
				{
					return this.srcReader.ReadState;
				}
				if (this.isClosed)
				{
					return ReadState.Closed;
				}
				return ReadState.EndOfFile;
			}
		}

		internal ColumnXmlReader(XmlReader xmlReader, string columnName, string strNamespace)
		{
			this.strColumnName = columnName;
			this.strColumnNamespace = strNamespace;
			this.srcReader = xmlReader;
			this.strXml = string.Empty;
			this.isDelegate = true;
			this.isClosed = false;
			this.startDepth = this.srcReader.Depth;
			XmlaReader xmlaReader = this.srcReader as XmlaReader;
			if (xmlaReader != null)
			{
				this.originalSkipElements = xmlaReader.SkipElements;
				xmlaReader.SkipElements = false;
			}
			do
			{
				xmlReader.Read();
			}
			while (!xmlReader.IsStartElement() && xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF);
			this.isDataSet = (xmlReader.NamespaceURI == "urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset");
		}

		internal ColumnXmlReader(string strIn)
		{
			this.strXml = strIn;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			this.srcReader = XmlReader.Create(new StringReader(this.strXml), xmlReaderSettings, new XmlParserContext(null, null, null, XmlSpace.Default));
			this.isDelegate = false;
			this.isClosed = false;
			do
			{
				this.srcReader.Read();
			}
			while (!this.srcReader.IsStartElement() && !this.srcReader.EOF);
			this.isDataSet = (this.srcReader.NamespaceURI == "urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset");
		}

		public override string ToString()
		{
			if (this.strXml.Length == 0)
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.WriteNode(this, true);
				if (this.isDelegate)
				{
					this.Close();
				}
				this.strXml = stringWriter.GetStringBuilder().ToString();
			}
			return this.strXml;
		}

		public override string GetAttribute(int i)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.GetAttribute(name, namespaceURI);
		}

		public override bool IsStartElement()
		{
			return !this.EOF && this.srcReader.IsStartElement();
		}

		public override bool IsStartElement(string localname, string ns)
		{
			return !this.EOF && this.srcReader.IsStartElement(localname, ns);
		}

		public override bool IsStartElement(string name)
		{
			return !this.EOF && this.srcReader.IsStartElement(name);
		}

		public override string LookupNamespace(string prefix)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return !this.EOF && this.srcReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return !this.EOF && this.srcReader.MoveToAttribute(name, ns);
		}

		public override XmlNodeType MoveToContent()
		{
			if (this.EOF)
			{
				return XmlNodeType.None;
			}
			return this.srcReader.MoveToContent();
		}

		public override bool MoveToElement()
		{
			return !this.EOF && this.srcReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return !this.EOF && this.srcReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return !this.EOF && this.srcReader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return !this.EOF && this.srcReader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return !this.EOF && this.srcReader.ReadAttributeValue();
		}

		public override string ReadElementString()
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadElementString();
		}

		public override string ReadElementString(string localname, string ns)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadElementString(localname, ns);
		}

		public override string ReadElementString(string name)
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadElementString(name);
		}

		public override void ReadEndElement()
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.ReadEndElement();
		}

		public override string ReadInnerXml()
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadOuterXml();
		}

		public override void ReadStartElement()
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.ReadStartElement();
		}

		public override void ReadStartElement(string localname, string ns)
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.ReadStartElement(localname, ns);
		}

		public override void ReadStartElement(string name)
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.ReadStartElement(name);
		}

		public override string ReadString()
		{
			if (this.EOF)
			{
				return "";
			}
			return this.srcReader.ReadString();
		}

		public override void ResolveEntity()
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.ResolveEntity();
		}

		public override void Skip()
		{
			if (this.EOF)
			{
				return;
			}
			this.srcReader.Skip();
		}

		public override void Close()
		{
			if (!this.isClosed)
			{
				this.isClosed = true;
				if (!this.isDelegate)
				{
					this.srcReader.Close();
					return;
				}
				while (this.Depth >= 0 && (this.srcReader.NodeType != XmlNodeType.EndElement || this.strColumnName != this.srcReader.Name || this.strColumnNamespace != this.srcReader.NamespaceURI))
				{
					this.srcReader.Read();
				}
				this.srcReader.ReadEndElement();
				XmlaReader xmlaReader = this.srcReader as XmlaReader;
				if (xmlaReader != null)
				{
					xmlaReader.SkipElements = this.originalSkipElements;
				}
			}
		}
	}
}
