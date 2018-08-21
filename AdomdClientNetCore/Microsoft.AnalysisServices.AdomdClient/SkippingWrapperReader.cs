using System;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class SkippingWrapperReader : XmlReader
	{
		private XmlReader reader;

		private NamespacesMgr namespacesManager;

		public override XmlNodeType NodeType
		{
			get
			{
				return this.reader.NodeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.reader.Name;
			}
		}

		public override string LocalName
		{
			get
			{
				return this.reader.LocalName;
			}
		}

		public override string NamespaceURI
		{
			get
			{
				return this.reader.NamespaceURI;
			}
		}

		public override string Prefix
		{
			get
			{
				return this.reader.Prefix;
			}
		}

		public override bool HasValue
		{
			get
			{
				return this.reader.HasValue;
			}
		}

		public override string Value
		{
			get
			{
				return this.reader.Value;
			}
		}

		public override int Depth
		{
			get
			{
				return this.reader.Depth;
			}
		}

		public override string BaseURI
		{
			get
			{
				return this.reader.BaseURI;
			}
		}

		public override bool IsEmptyElement
		{
			get
			{
				return this.reader.IsEmptyElement;
			}
		}

		public override bool IsDefault
		{
			get
			{
				return this.reader.IsDefault;
			}
		}

		public override char QuoteChar
		{
			get
			{
				return this.reader.QuoteChar;
			}
		}

		public override XmlSpace XmlSpace
		{
			get
			{
				return this.reader.XmlSpace;
			}
		}

		public override string XmlLang
		{
			get
			{
				return this.reader.XmlLang;
			}
		}

		public override int AttributeCount
		{
			get
			{
				return this.reader.AttributeCount;
			}
		}

		public override bool CanResolveEntity
		{
			get
			{
				return this.reader.CanResolveEntity;
			}
		}

		public override bool EOF
		{
			get
			{
				return this.reader.EOF;
			}
		}

		public override ReadState ReadState
		{
			get
			{
				return this.reader.ReadState;
			}
		}

		public override bool HasAttributes
		{
			get
			{
				return this.reader.HasAttributes;
			}
		}

		public override XmlNameTable NameTable
		{
			get
			{
				return this.reader.NameTable;
			}
		}

		public override string this[int i]
		{
			get
			{
				return this.reader[i];
			}
		}

		public override string this[string name]
		{
			get
			{
				return this.reader[name];
			}
		}

		public override string this[string name, string namespaceURI]
		{
			get
			{
				return this.reader[name, namespaceURI];
			}
		}

		internal SkippingWrapperReader(XmlReader reader)
		{
			this.reader = reader;
			this.namespacesManager = new NamespacesMgr();
		}

		public override string GetAttribute(string name)
		{
			return this.reader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return this.reader.GetAttribute(name, namespaceURI);
		}

		public override string GetAttribute(int i)
		{
			return this.reader.GetAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return this.reader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return this.reader.MoveToAttribute(name, ns);
		}

		public override void MoveToAttribute(int i)
		{
			this.reader.MoveToAttribute(i);
		}

		public override bool MoveToFirstAttribute()
		{
			return this.reader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return this.reader.MoveToNextAttribute();
		}

		public override bool MoveToElement()
		{
			return this.reader.MoveToElement();
		}

		public override bool Read()
		{
			return this.reader.Read();
		}

		public override void Close()
		{
		}

		public override void Skip()
		{
			this.reader.Skip();
		}

		public override string ReadString()
		{
			return this.reader.ReadString();
		}

		public override XmlNodeType MoveToContent()
		{
			while (this.reader.MoveToContent() == XmlNodeType.Element)
			{
				if (!this.namespacesManager.IsNamespaceSkippable(this.NamespaceURI))
				{
					return this.NodeType;
				}
				this.reader.Skip();
			}
			return this.NodeType;
		}

		public override void ReadStartElement()
		{
			this.MoveToContent();
			this.reader.ReadStartElement();
		}

		public override void ReadStartElement(string name)
		{
			this.MoveToContent();
			this.reader.ReadStartElement(name);
		}

		public override void ReadStartElement(string localname, string ns)
		{
			this.MoveToContent();
			this.reader.ReadStartElement(localname, ns);
		}

		public override string ReadElementString()
		{
			this.MoveToContent();
			return this.reader.ReadElementString();
		}

		public override string ReadElementString(string name)
		{
			this.MoveToContent();
			return this.reader.ReadElementString(name);
		}

		public override string ReadElementString(string localname, string ns)
		{
			this.MoveToContent();
			return this.reader.ReadElementString(localname, ns);
		}

		public override void ReadEndElement()
		{
			this.MoveToContent();
			this.reader.ReadEndElement();
		}

		public override bool IsStartElement()
		{
			this.MoveToContent();
			return this.reader.IsStartElement();
		}

		public override bool IsStartElement(string name)
		{
			this.MoveToContent();
			return this.reader.IsStartElement(name);
		}

		public override bool IsStartElement(string localname, string ns)
		{
			this.MoveToContent();
			return this.reader.IsStartElement(localname, ns);
		}

		public override string ReadInnerXml()
		{
			this.MoveToContent();
			return this.reader.ReadInnerXml();
		}

		public override string ReadOuterXml()
		{
			this.MoveToContent();
			return this.reader.ReadOuterXml();
		}

		public override string LookupNamespace(string prefix)
		{
			return this.reader.LookupNamespace(prefix);
		}

		public override void ResolveEntity()
		{
			this.reader.ResolveEntity();
		}

		public override bool ReadAttributeValue()
		{
			return this.reader.ReadAttributeValue();
		}
	}
}
