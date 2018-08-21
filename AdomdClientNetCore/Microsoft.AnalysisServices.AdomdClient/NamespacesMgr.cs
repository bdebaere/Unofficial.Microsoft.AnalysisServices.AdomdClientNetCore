using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class NamespacesMgr
	{
		internal const string NamespaceCompatibilityElementName = "NamespaceCompatibility";

		internal const string NamespaceCompatibilityNamespace = "http://schemas.microsoft.com/analysisservices/2003/engine";

		private const string NamespaceElementName = "Namespace";

		private const string CompatibilityElementName = "Compatibility";

		private const string ProductBranchElementName = "ProductBranch";

		private const string VersionTimeStampElementName = "VersionTimeStamp";

		private const string MsasBranch = "MicrosoftAnalysisServices";

		private const string XmlaBranch = "xmla";

		internal const string NamespaceCompatibilityRequest = "<NamespaceCompatibility xmlns=\"http://schemas.microsoft.com/analysisservices/2003/xmla\" mustUnderstand=\"0\"/>";

		private const string NamespaceCompatibilityXmlSchemaTemplate = "<xsd:schema targetNamespace=\"{0}\" xmlns = \"{1}\" xmlns:sql=\"{2}\" xmlns:xsd=\"{3}\" elementFormDefault=\"qualified\">\r\n\t\t\t\t<xsd:element name=\"{4}\">\r\n\t\t\t\t\t<xsd:complexType>\r\n\t\t\t\t\t\t<xsd:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n\t\t\t\t\t\t\t<xsd:element name=\"{5}\" type=\"{5}\"/>\r\n\t\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t\t</xsd:complexType>\r\n\t\t\t\t</xsd:element>\r\n\t\t\t\t<xsd:complexType name=\"{5}\">\r\n\t\t\t\t\t<xsd:sequence>\r\n\t\t\t\t\t\t<xsd:element sql:field=\"{6}\" name=\"{6}\" type=\"xsd:string\"/>\r\n\t\t\t\t\t\t<xsd:element sql:field=\"{7}\" name=\"{7}\" minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n\t\t\t\t\t\t\t<xsd:complexType>\r\n\t\t\t\t\t\t\t\t<xsd:sequence>\r\n\t\t\t\t\t\t\t\t\t<xsd:element sql:field=\"{8}\" name=\"{8}\" type=\"xsd:string\" minOccurs=\"0\"/>\r\n\t\t\t\t\t\t\t\t\t<xsd:element sql:field=\"{9}\" name=\"{9}\" type=\"xsd:dateTime\" minOccurs=\"0\"/>\r\n\t\t\t\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t\t\t\t</xsd:complexType>\r\n\t\t\t\t\t\t</xsd:element>\r\n\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t</xsd:complexType>\r\n\t\t\t</xsd:schema>";

		private static readonly DateTime AdomdVersionTimeStamp;

		private static readonly string NamespaceCompatibilityXmlSchema;

		private static readonly DataSet compatibilityDataSet;

		private static readonly string[] known;

		private Hashtable namespacesToIgnore;

		static NamespacesMgr()
		{
			NamespacesMgr.AdomdVersionTimeStamp = new DateTime(2003, 11, 1, CultureInfo.InvariantCulture.Calendar);
			NamespacesMgr.NamespaceCompatibilityXmlSchema = null;
			NamespacesMgr.compatibilityDataSet = null;
			NamespacesMgr.known = new string[]
			{
				"http://schemas.xmlsoap.org/soap/envelope/",
				"urn:schemas-microsoft-com:xml-analysis",
				"http://schemas.microsoft.com/analysisservices/2003/engine",
				"http://schemas.microsoft.com/analysisservices/2003/engine/2",
				"http://schemas.microsoft.com/analysisservices/2003/engine/2/2",
				"http://schemas.microsoft.com/analysisservices/2008/engine/100",
				"http://schemas.microsoft.com/analysisservices/2008/engine/100/100",
				"http://schemas.microsoft.com/analysisservices/2010/engine/200",
				"http://schemas.microsoft.com/analysisservices/2010/engine/200/200",
				"http://schemas.microsoft.com/analysisservices/2011/engine/300",
				"http://schemas.microsoft.com/analysisservices/2011/engine/300/300",
				"http://schemas.microsoft.com/analysisservices/2012/engine/400",
				"http://schemas.microsoft.com/analysisservices/2012/engine/400/400",
				"http://schemas.microsoft.com/analysisservices/2012/engine/410",
				"http://schemas.microsoft.com/analysisservices/2012/engine/410/410",
				"http://schemas.microsoft.com/analysisservices/2013/engine/500",
				"http://schemas.microsoft.com/analysisservices/2013/engine/500/500",
				"urn:schemas-microsoft-com:xml-analysis:rowset",
				"urn:schemas-microsoft-com:xml-analysis:mddataset",
				"urn:schemas-microsoft-com:xml-analysis:exception",
				"http://www.w3.org/2001/XMLSchema",
				"http://www.w3.org/2001/XMLSchema-instance",
				"urn:schemas-microsoft-com:xml-analysis:empty",
				"http://schemas.microsoft.com/analysisservices/2003/xmla",
				"http://schemas.microsoft.com/analysisservices/2003/ext",
				"http://schemas.microsoft.com/analysisservices/2003/xmla-multipleresults",
				"urn:schemas-microsoft-com:xml-analysis:fault",
				"",
				"urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset",
				"urn:schemas-microsoft-com:xml-sql"
			};
			NamespacesMgr.NamespaceCompatibilityXmlSchema = string.Format(CultureInfo.InvariantCulture, "<xsd:schema targetNamespace=\"{0}\" xmlns = \"{1}\" xmlns:sql=\"{2}\" xmlns:xsd=\"{3}\" elementFormDefault=\"qualified\">\r\n\t\t\t\t<xsd:element name=\"{4}\">\r\n\t\t\t\t\t<xsd:complexType>\r\n\t\t\t\t\t\t<xsd:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n\t\t\t\t\t\t\t<xsd:element name=\"{5}\" type=\"{5}\"/>\r\n\t\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t\t</xsd:complexType>\r\n\t\t\t\t</xsd:element>\r\n\t\t\t\t<xsd:complexType name=\"{5}\">\r\n\t\t\t\t\t<xsd:sequence>\r\n\t\t\t\t\t\t<xsd:element sql:field=\"{6}\" name=\"{6}\" type=\"xsd:string\"/>\r\n\t\t\t\t\t\t<xsd:element sql:field=\"{7}\" name=\"{7}\" minOccurs=\"0\" maxOccurs=\"unbounded\">\r\n\t\t\t\t\t\t\t<xsd:complexType>\r\n\t\t\t\t\t\t\t\t<xsd:sequence>\r\n\t\t\t\t\t\t\t\t\t<xsd:element sql:field=\"{8}\" name=\"{8}\" type=\"xsd:string\" minOccurs=\"0\"/>\r\n\t\t\t\t\t\t\t\t\t<xsd:element sql:field=\"{9}\" name=\"{9}\" type=\"xsd:dateTime\" minOccurs=\"0\"/>\r\n\t\t\t\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t\t\t\t</xsd:complexType>\r\n\t\t\t\t\t\t</xsd:element>\r\n\t\t\t\t\t</xsd:sequence>\r\n\t\t\t\t</xsd:complexType>\r\n\t\t\t</xsd:schema>", new object[]
			{
				"urn:schemas-microsoft-com:xml-analysis:rowset",
				"urn:schemas-microsoft-com:xml-analysis:rowset",
				"urn:schemas-microsoft-com:xml-sql",
				"http://www.w3.org/2001/XMLSchema",
				"root",
				"row",
				"Namespace",
				"Compatibility",
				"ProductBranch",
				"VersionTimeStamp"
			});
			XmlReader xmlReader = null;
			try
			{
				xmlReader = XmlReader.Create(new StringReader(NamespacesMgr.NamespaceCompatibilityXmlSchema));
				RowsetFormatter.DataSetCreator dataSetCreator = new RowsetFormatter.DataSetCreator(null, false, null);
				FormattersHelpers.LoadSchema(xmlReader, new FormattersHelpers.ColumnDefinitionDelegate(dataSetCreator.ConstructDataSetColumnDelegate));
				NamespacesMgr.compatibilityDataSet = dataSetCreator.DataSet;
			}
			finally
			{
				if (xmlReader != null)
				{
					xmlReader.Close();
				}
			}
		}

		internal NamespacesMgr()
		{
		}

		internal bool IsNamespaceSkippable(string theNamespace)
		{
			if (this.namespacesToIgnore == null)
			{
				for (int i = 0; i < NamespacesMgr.known.Length; i++)
				{
					if (NamespacesMgr.known[i] == theNamespace)
					{
						return false;
					}
				}
				return true;
			}
			return this.namespacesToIgnore.ContainsKey(theNamespace);
		}

		private bool IsKnown(string theNamespace)
		{
			for (int i = 0; i < NamespacesMgr.known.Length; i++)
			{
				if (NamespacesMgr.known[i] == theNamespace)
				{
					return true;
				}
			}
			return false;
		}

		internal void PopulateIgnorableNamespaces(XmlReader reader)
		{
			if (!reader.IsStartElement("NamespaceCompatibility", "http://schemas.microsoft.com/analysisservices/2003/engine"))
			{
				this.namespacesToIgnore = null;
				return;
			}
			this.namespacesToIgnore = new Hashtable();
			DataSet dataSet = NamespacesMgr.compatibilityDataSet.Clone();
			reader.ReadStartElement("NamespaceCompatibility", "http://schemas.microsoft.com/analysisservices/2003/engine");
			if (!reader.IsStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset"))
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, string.Format(CultureInfo.InvariantCulture, "Expected {0}:{1} element, got {2}", new object[]
				{
					"urn:schemas-microsoft-com:xml-analysis:rowset",
					"root",
					reader.Name
				}));
			}
			if (reader.IsEmptyElement)
			{
				reader.ReadStartElement();
			}
			else
			{
				reader.ReadStartElement("root", "urn:schemas-microsoft-com:xml-analysis:rowset");
				RowsetFormatter.PopulateDataset(reader, "row", "urn:schemas-microsoft-com:xml-analysis:rowset", InlineErrorHandlingType.StoreInCell, dataSet.Tables["rowsetTable"]);
				DataTable dataTable = dataSet.Tables["rowsetTable"];
				foreach (DataRow dataRow in dataTable.Rows)
				{
					string text = dataRow["Namespace"] as string;
					if (text != null)
					{
						DataRow[] childRows = dataRow.GetChildRows("rowsetTableCompatibility");
						if (childRows.Length > 0)
						{
							DataRow[] array = childRows;
							for (int i = 0; i < array.Length; i++)
							{
								DataRow dataRow2 = array[i];
								string a = dataRow2["ProductBranch"] as string;
								object obj = dataRow2["VersionTimeStamp"];
								DateTime t = (obj == null || obj is DBNull) ? DateTime.MinValue : ((DateTime)obj);
								if ((a == "MicrosoftAnalysisServices" || a == "xmla") && t <= NamespacesMgr.AdomdVersionTimeStamp && !this.IsKnown(text))
								{
									this.namespacesToIgnore[text] = 1;
								}
							}
						}
						else if (!this.IsKnown(text))
						{
							this.namespacesToIgnore[text] = 1;
						}
					}
				}
				reader.ReadEndElement();
			}
			reader.ReadEndElement();
		}
	}
}
