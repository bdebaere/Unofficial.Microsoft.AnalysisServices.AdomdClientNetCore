using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class FormattersHelpers
	{
		private delegate object GetValueDelegate(string s);

		private delegate string CovertToStringDelegate(object val);

		public delegate object ColumnDefinitionDelegate(int ordinal, string name, string theNamespace, string caption, Type type, bool isNested, object parent, string strColumnXsdType);

		private class EmptyRestorer : IDisposable
		{
			public void Dispose()
			{
			}
		}

		internal const string GuidColumnType = "uuid";

		internal const string XmlDocumentColumnType = "xmlDocument";

		internal const string XmlDocumentColumnDataset = "urn:schemas-microsoft-com:xml-analysis:xmlDocumentDataset";

		internal const string BinaryDataTypeName = "base64Binary";

		private const string typeKeyStr = "type";

		private const string namespaceKeyStr = "namespace";

		private const string xsdTypeNameKeyStr = "xsdTypeName";

		private const string convertTolocalTimeStr = "convertTolocalTime";

		private const string typeAttribute = "type";

		private const string occurenceUnbounded = "unbounded";

		private const string fieldAttributeName = "field";

		private const string nilAttributeName = "nil";

		private const string trueConstant = "true";

		public const int PreFirstColumn = -1;

		public const int PostLastColumn = -2;

		internal static string RowElement;

		internal static string RowElementNamespace;

		private static readonly FormattersHelpers.EmptyRestorer emptyRestorer;

		private static Hashtable typeConvertersHash;

		private static Hashtable stringConvertersHash;

		private static readonly Type dateTimeType;

		static FormattersHelpers()
		{
			FormattersHelpers.RowElement = "row";
			FormattersHelpers.RowElementNamespace = "urn:schemas-microsoft-com:xml-analysis:rowset";
			FormattersHelpers.emptyRestorer = new FormattersHelpers.EmptyRestorer();
			FormattersHelpers.typeConvertersHash = null;
			FormattersHelpers.stringConvertersHash = null;
			FormattersHelpers.dateTimeType = typeof(DateTime);
			FormattersHelpers.FillInTypeConvertersHash();
			FormattersHelpers.FillInStringConvertersHash();
		}

		private static object ConvertToBoolean(string s)
		{
			return XmlConvert.ToBoolean(s);
		}

		private static object ConvertToByte(string s)
		{
			return XmlConvert.ToByte(s);
		}

		private static object ConvertToChar(string s)
		{
			return XmlConvert.ToChar(s);
		}

		private static object ConvertToDateTime(string s)
		{
			return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
		}

		private static object ConvertToDecimal(string s)
		{
			return XmlConvert.ToDecimal(s);
		}

		private static object ConvertToDouble(string s)
		{
			return XmlConvert.ToDouble(s);
		}

		private static object ConvertToGuid(string s)
		{
			return XmlConvert.ToGuid(s);
		}

		private static object ConvertToInt16(string s)
		{
			return XmlConvert.ToInt16(s);
		}

		private static object ConvertToInt32(string s)
		{
			return XmlConvert.ToInt32(s);
		}

		private static object ConvertToInt64(string s)
		{
			return XmlConvert.ToInt64(s);
		}

		private static object ConvertToSByte(string s)
		{
			return XmlConvert.ToSByte(s);
		}

		private static object ConvertToSingle(string s)
		{
			return XmlConvert.ToSingle(s);
		}

		private static object ConvertToString(string s)
		{
			return s;
		}

		private static object ConvertToTimeSpan(string s)
		{
			return XmlConvert.ToTimeSpan(s);
		}

		private static object ConvertToUInt16(string s)
		{
			return XmlConvert.ToUInt16(s);
		}

		private static object ConvertToUInt32(string s)
		{
			return XmlConvert.ToUInt32(s);
		}

		private static object ConvertToUInt64(string s)
		{
			return XmlConvert.ToUInt64(s);
		}

		private static object ConvertToBase64(string s)
		{
			return Convert.FromBase64String(s);
		}

		private static object ConvertToObject(string s)
		{
			return s;
		}

		private static string ConvertFromBoolean(object val)
		{
			return XmlConvert.ToString((bool)val);
		}

		private static string ConvertFromByte(object val)
		{
			return XmlConvert.ToString((byte)val);
		}

		private static string ConvertFromChar(object val)
		{
			return XmlConvert.ToString((char)val);
		}

		private static string ConvertFromDateTime(object val)
		{
			return XmlConvert.ToString((DateTime)val, XmlDateTimeSerializationMode.RoundtripKind);
		}

		private static string ConvertFromDecimal(object val)
		{
			return XmlConvert.ToString((decimal)val);
		}

		private static string ConvertFromDouble(object val)
		{
			return XmlConvert.ToString((double)val);
		}

		private static string ConvertFromGuid(object val)
		{
			return XmlConvert.ToString((Guid)val);
		}

		private static string ConvertFromInt16(object val)
		{
			return XmlConvert.ToString((short)val);
		}

		private static string ConvertFromInt32(object val)
		{
			return XmlConvert.ToString((int)val);
		}

		private static string ConvertFromInt64(object val)
		{
			return XmlConvert.ToString((long)val);
		}

		private static string ConvertFromSByte(object val)
		{
			return XmlConvert.ToString((sbyte)val);
		}

		private static string ConvertFromSingle(object val)
		{
			return XmlConvert.ToString((float)val);
		}

		private static string ConvertFromTimeSpan(object val)
		{
			return XmlConvert.ToString((TimeSpan)val);
		}

		private static string ConvertFromUInt16(object val)
		{
			return XmlConvert.ToString((ushort)val);
		}

		private static string ConvertFromUInt32(object val)
		{
			return XmlConvert.ToString((uint)val);
		}

		private static string ConvertFromUInt64(object val)
		{
			return XmlConvert.ToString((ulong)val);
		}

		private static string ConvertFromBase64(object val)
		{
			return Convert.ToBase64String((byte[])val);
		}

		private FormattersHelpers()
		{
		}

		private static object ConvertFromXml(string xmlValue, Type targetType, bool convertLocalTime)
		{
			object result;
			try
			{
				if (!convertLocalTime || targetType != FormattersHelpers.dateTimeType)
				{
					result = ((FormattersHelpers.GetValueDelegate)FormattersHelpers.typeConvertersHash[targetType])(xmlValue);
				}
				else
				{
					result = ((DateTime)((FormattersHelpers.GetValueDelegate)FormattersHelpers.typeConvertersHash[targetType])(xmlValue)).ToLocalTime();
				}
			}
			catch (ArgumentException ex)
			{
				result = ex;
			}
			catch (ArithmeticException ex2)
			{
				result = ex2;
			}
			catch (FormatException ex3)
			{
				result = ex3;
			}
			return result;
		}

		public static string ConvertToXml(object objVal)
		{
			FormattersHelpers.CovertToStringDelegate covertToStringDelegate = FormattersHelpers.stringConvertersHash[objVal.GetType()] as FormattersHelpers.CovertToStringDelegate;
			if (covertToStringDelegate != null)
			{
				return covertToStringDelegate(objVal);
			}
			return objVal.ToString();
		}

		public static bool IsNullContentElement(XmlReader reader)
		{
			return reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true";
		}

		public static object ReadRowsetProperty(XmlReader reader, string elementName, string elementNamespace, Type type, bool throwOnInlineError, bool isArray, bool convertToLocalTime)
		{
			object result = null;
			if (isArray)
			{
				ArrayList arrayList = new ArrayList();
				Type elementType = type.GetElementType();
				while (reader.IsStartElement(elementName, elementNamespace))
				{
					object obj = FormattersHelpers.ReadRowsetProperty(reader, elementName, elementNamespace, elementType, throwOnInlineError, false, convertToLocalTime);
					if (obj is XmlaError)
					{
						if (throwOnInlineError)
						{
							throw XmlaResultCollection.ExceptionOnError(new XmlaResult
							{
								Messages = 
								{
									(XmlaError)obj
								}
							});
						}
						while (reader.IsStartElement(elementName, elementNamespace))
						{
							reader.Skip();
						}
						result = obj;
						break;
					}
					else
					{
						arrayList.Add(obj);
					}
				}
				Array array = Array.CreateInstance(elementType, arrayList.Count);
				arrayList.CopyTo(array);
				result = array;
			}
			else
			{
				if (!reader.IsEmptyElement)
				{
					using (FormattersHelpers.GetWhitespaceHandlingRestorer(reader, WhitespaceHandling.All))
					{
						reader.ReadStartElement(elementName, elementNamespace);
						if (reader.NodeType == XmlNodeType.Text)
						{
							result = FormattersHelpers.ConvertFromXml(reader.ReadString(), type, convertToLocalTime);
						}
						else
						{
							string text = FormattersHelpers.ReadWhiteSpace(reader);
							if (reader.NodeType == XmlNodeType.EndElement)
							{
								result = text;
							}
							else
							{
								bool flag = false;
								bool skipElements = false;
								if (type == typeof(object) || type == typeof(string))
								{
									flag = true;
									skipElements = ((XmlaReader)reader).SkipElements;
									((XmlaReader)reader).SkipElements = false;
								}
								try
								{
									if ((result = XmlaClient.CheckAndGetRowsetError(reader, throwOnInlineError)) == null)
									{
										FormattersHelpers.CheckException(reader);
										if (flag)
										{
											result = FormattersHelpers.ReadPropertyXml(reader);
										}
									}
								}
								finally
								{
									if (flag)
									{
										((XmlaReader)reader).SkipElements = skipElements;
									}
								}
							}
						}
						reader.ReadEndElement();
						return result;
					}
				}
				reader.Skip();
				result = FormattersHelpers.ConvertFromXml(string.Empty, type, convertToLocalTime);
			}
			return result;
		}

		public static void CheckException(XmlReader reader)
		{
			XmlaClient.CheckForException(reader, null, true);
		}

		public static object ReadDataSetProperty(XmlReader reader, Type type)
		{
			object result = null;
			if (!reader.IsEmptyElement)
			{
				using (FormattersHelpers.GetWhitespaceHandlingRestorer(reader, WhitespaceHandling.All))
				{
					reader.ReadStartElement();
					if (reader.NodeType == XmlNodeType.Text)
					{
						result = ((FormattersHelpers.GetValueDelegate)FormattersHelpers.typeConvertersHash[type])(reader.ReadString());
					}
					else
					{
						string text = FormattersHelpers.ReadWhiteSpace(reader);
						if (reader.NodeType == XmlNodeType.EndElement)
						{
							result = text;
						}
						else if ((result = XmlaClient.CheckAndGetDatasetError(reader)) == null)
						{
							FormattersHelpers.CheckException(reader);
							if (type == typeof(object))
							{
								result = FormattersHelpers.ReadPropertyXml(reader);
							}
						}
					}
					reader.ReadEndElement();
					return result;
				}
			}
			reader.Read();
			result = ((FormattersHelpers.GetValueDelegate)FormattersHelpers.typeConvertersHash[type])(string.Empty);
			return result;
		}

		public static Type GetElementType(XmlReader reader)
		{
			return FormattersHelpers.GetElementType(reader, null, typeof(object));
		}

		public static Type GetElementType(XmlReader reader, string theNamespace, Type typeIfNotSpecified)
		{
			Type type = FormattersHelpers.GetElementTypeInternal(reader, theNamespace);
			if (type == null)
			{
				type = typeIfNotSpecified;
			}
			return type;
		}

		public static Type GetElementType(XmlReader reader, string theNamespace, DataColumn column)
		{
			Type type = FormattersHelpers.GetElementTypeInternal(reader, theNamespace);
			if (type == null)
			{
				type = FormattersHelpers.GetColumnType(column);
			}
			return type;
		}

		public static Type GetColumnType(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			Type type = column.ExtendedProperties["type"] as Type;
			if (type == null)
			{
				type = column.DataType;
			}
			return type;
		}

		public static void SetColumnType(DataColumn column, Type type)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			if (type != null)
			{
				column.ExtendedProperties["type"] = type;
			}
		}

		public static string GetColumnNamespace(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			return column.ExtendedProperties["namespace"] as string;
		}

		public static void SetColumnNamespace(DataColumn column, string theNamespace)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			column.ExtendedProperties["namespace"] = theNamespace;
		}

		public static string GetColumnXsdTypeName(DataColumn column)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			return column.ExtendedProperties["xsdTypeName"] as string;
		}

		public static void SetColumnXsdTypeName(DataColumn column, string theXsdTypeName)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			column.ExtendedProperties["xsdTypeName"] = theXsdTypeName;
		}

		public static string GetColumnCaptionFromSchemaElement(XmlSchemaElement element)
		{
			XmlAttribute unhandledAttributeByName = FormattersHelpers.GetUnhandledAttributeByName(element, "field");
			if (unhandledAttributeByName != null)
			{
				return unhandledAttributeByName.Value;
			}
			return element.Name;
		}

		internal static bool GetConvertToLocalTime(DataColumn column)
		{
			return column != null && column.ExtendedProperties.ContainsKey("convertTolocalTime") && (bool)column.ExtendedProperties["convertTolocalTime"];
		}

		internal static void SetConvertToLocalTime(DataColumn column, bool convert)
		{
			if (column != null)
			{
				column.ExtendedProperties["convertTolocalTime"] = convert;
			}
		}

		public static void GetSchemaElementTypeAndName(XmlSchemaElement element, out Type valueType, out string typeName)
		{
			valueType = typeof(object);
			typeName = string.Empty;
			if (!element.SchemaTypeName.IsEmpty && element.MaxOccurs > 0m)
			{
				XmlSchemaDatatype xmlSchemaDatatype;
				if (element.SchemaTypeName.Namespace == "http://www.w3.org/2001/XMLSchema")
				{
					xmlSchemaDatatype = element.ElementSchemaType.Datatype;
				}
				else
				{
					xmlSchemaDatatype = null;
				}
				XmlSchemaType elementSchemaType = element.ElementSchemaType;
				if (xmlSchemaDatatype != null)
				{
					valueType = xmlSchemaDatatype.ValueType;
					typeName = element.SchemaTypeName.Name;
				}
				else if (elementSchemaType != null)
				{
					if (elementSchemaType is XmlSchemaSimpleType)
					{
						XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)elementSchemaType;
						if (xmlSchemaSimpleType.Name == "uuid")
						{
							valueType = typeof(Guid);
							typeName = xmlSchemaSimpleType.Name;
						}
					}
					else if (elementSchemaType is XmlSchemaComplexType)
					{
						XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)elementSchemaType;
						if (xmlSchemaComplexType.Name == "xmlDocument")
						{
							valueType = typeof(string);
							typeName = xmlSchemaComplexType.Name;
						}
					}
				}
				if (element.MaxOccursString == "unbounded")
				{
					valueType = Array.CreateInstance(valueType, 0).GetType();
					typeName = valueType.ToString();
				}
			}
		}

		public static bool IsNestedRowsetColumn(XmlSchemaElement xmlSchemaElement)
		{
			if (xmlSchemaElement != null && xmlSchemaElement.SchemaType is XmlSchemaComplexType && ((XmlSchemaComplexType)xmlSchemaElement.SchemaType).Particle is XmlSchemaSequence)
			{
				XmlSchemaSequence xmlSchemaSequence = ((XmlSchemaComplexType)xmlSchemaElement.SchemaType).Particle as XmlSchemaSequence;
				foreach (XmlSchemaObject current in xmlSchemaSequence.Items)
				{
					if (!(current is XmlSchemaElement))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public static void LoadSchema(XmlReader xmlReader, FormattersHelpers.ColumnDefinitionDelegate definitionDelegate)
		{
			FormattersHelpers.LoadSchema(xmlReader, definitionDelegate, false);
		}

		public static void LoadSchema(XmlReader xmlReader, FormattersHelpers.ColumnDefinitionDelegate definitionDelegate, bool requirePrePostInit)
		{
			try
			{
				xmlReader.MoveToContent();
				XmlSchema xmlSchema;
				if (xmlReader is XmlaReader)
				{
					xmlSchema = ((XmlaReader)xmlReader).ReadSchema();
				}
				else
				{
					xmlSchema = XmlSchema.Read(xmlReader, null);
					xmlReader.ReadEndElement();
				}
				xmlReader.MoveToContent();
				XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
				xmlSchemaSet.Add(xmlSchema);
				xmlSchemaSet.Compile();
				foreach (XmlSchemaObject current in xmlSchema.Items)
				{
					if (current is XmlSchemaComplexType)
					{
						XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)current;
						if (xmlSchemaComplexType.Name == FormattersHelpers.RowElement)
						{
							FormattersHelpers.LoadComplexType(xmlSchemaComplexType, definitionDelegate, null, 0, requirePrePostInit);
							break;
						}
					}
				}
			}
			catch (XmlSchemaException innerException)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException);
			}
		}

		private static void LoadComplexType(XmlSchemaComplexType complexType, FormattersHelpers.ColumnDefinitionDelegate definitionDelegate, object parent, int ordinal, bool requirePrePostInit)
		{
			int num = ordinal;
			XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)complexType.Particle;
			if (requirePrePostInit)
			{
				definitionDelegate(-1, null, null, null, null, false, xmlSchemaSequence.Items.Count, null);
			}
			foreach (XmlSchemaElement xmlSchemaElement in xmlSchemaSequence.Items)
			{
				string name = (xmlSchemaElement.Name != null) ? xmlSchemaElement.Name : xmlSchemaElement.QualifiedName.Name;
				string columnCaptionFromSchemaElement = FormattersHelpers.GetColumnCaptionFromSchemaElement(xmlSchemaElement);
				string @namespace = xmlSchemaElement.QualifiedName.Namespace;
				if (FormattersHelpers.IsNestedRowsetColumn(xmlSchemaElement))
				{
					object obj = definitionDelegate(num, name, @namespace, columnCaptionFromSchemaElement, null, true, parent, null);
					FormattersHelpers.ColumnDefinitionDelegate columnDefinitionDelegate = obj as FormattersHelpers.ColumnDefinitionDelegate;
					if (columnDefinitionDelegate == null)
					{
						columnDefinitionDelegate = definitionDelegate;
					}
					else
					{
						obj = parent;
					}
					FormattersHelpers.LoadComplexType((XmlSchemaComplexType)xmlSchemaElement.ElementSchemaType, columnDefinitionDelegate, obj, 0, requirePrePostInit);
				}
				else
				{
					Type type = null;
					string strColumnXsdType = null;
					FormattersHelpers.GetSchemaElementTypeAndName(xmlSchemaElement, out type, out strColumnXsdType);
					definitionDelegate(num, name, @namespace, columnCaptionFromSchemaElement, type, false, parent, strColumnXsdType);
				}
				num++;
			}
			if (requirePrePostInit)
			{
				definitionDelegate(-2, null, null, null, null, false, xmlSchemaSequence.Items.Count, null);
			}
		}

		private static Type GetElementTypeInternal(XmlReader reader, string theNamespace)
		{
			Type type = null;
			string attribute = reader.GetAttribute("type", theNamespace);
			if (attribute != null)
			{
				string[] array = attribute.Split(new char[]
				{
					':'
				});
				if (array.Length == 2)
				{
					if (!(reader.LookupNamespace(array[0]) == "http://www.w3.org/2001/XMLSchema"))
					{
						throw new AdomdUnknownResponseException(XmlaSR.UnexpectedXsiType(attribute), "");
					}
					type = NetTypeMapper.GetNetType(array[1]);
				}
				else
				{
					type = NetTypeMapper.GetNetType(attribute);
				}
				if (type == null)
				{
					throw new AdomdUnknownResponseException(XmlaSR.UnexpectedXsiType(attribute), "");
				}
			}
			return type;
		}

		private static XmlAttribute GetUnhandledAttributeByName(XmlSchemaElement element, string name)
		{
			for (int i = 0; i < element.UnhandledAttributes.Length; i++)
			{
				if (element.UnhandledAttributes[i].LocalName == name)
				{
					return element.UnhandledAttributes[i];
				}
			}
			return null;
		}

		private static void FillInTypeConvertersHash()
		{
			FormattersHelpers.typeConvertersHash = new Hashtable(19);
			FormattersHelpers.typeConvertersHash[typeof(bool)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToBoolean);
			FormattersHelpers.typeConvertersHash[typeof(byte)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToByte);
			FormattersHelpers.typeConvertersHash[typeof(char)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToChar);
			FormattersHelpers.typeConvertersHash[typeof(DateTime)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToDateTime);
			FormattersHelpers.typeConvertersHash[typeof(decimal)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToDecimal);
			FormattersHelpers.typeConvertersHash[typeof(double)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToDouble);
			FormattersHelpers.typeConvertersHash[typeof(Guid)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToGuid);
			FormattersHelpers.typeConvertersHash[typeof(short)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToInt16);
			FormattersHelpers.typeConvertersHash[typeof(int)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToInt32);
			FormattersHelpers.typeConvertersHash[typeof(long)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToInt64);
			FormattersHelpers.typeConvertersHash[typeof(sbyte)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToSByte);
			FormattersHelpers.typeConvertersHash[typeof(float)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToSingle);
			FormattersHelpers.typeConvertersHash[typeof(string)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToString);
			FormattersHelpers.typeConvertersHash[typeof(TimeSpan)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToTimeSpan);
			FormattersHelpers.typeConvertersHash[typeof(ushort)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToUInt16);
			FormattersHelpers.typeConvertersHash[typeof(uint)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToUInt32);
			FormattersHelpers.typeConvertersHash[typeof(ulong)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToUInt64);
			FormattersHelpers.typeConvertersHash[typeof(byte[])] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToBase64);
			FormattersHelpers.typeConvertersHash[typeof(object)] = new FormattersHelpers.GetValueDelegate(FormattersHelpers.ConvertToObject);
		}

		private static void FillInStringConvertersHash()
		{
			FormattersHelpers.stringConvertersHash = new Hashtable(17);
			FormattersHelpers.stringConvertersHash[typeof(bool)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromBoolean);
			FormattersHelpers.stringConvertersHash[typeof(byte)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromByte);
			FormattersHelpers.stringConvertersHash[typeof(char)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromChar);
			FormattersHelpers.stringConvertersHash[typeof(DateTime)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromDateTime);
			FormattersHelpers.stringConvertersHash[typeof(decimal)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromDecimal);
			FormattersHelpers.stringConvertersHash[typeof(double)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromDouble);
			FormattersHelpers.stringConvertersHash[typeof(Guid)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromGuid);
			FormattersHelpers.stringConvertersHash[typeof(short)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromInt16);
			FormattersHelpers.stringConvertersHash[typeof(int)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromInt32);
			FormattersHelpers.stringConvertersHash[typeof(long)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromInt64);
			FormattersHelpers.stringConvertersHash[typeof(sbyte)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromSByte);
			FormattersHelpers.stringConvertersHash[typeof(float)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromSingle);
			FormattersHelpers.stringConvertersHash[typeof(TimeSpan)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromTimeSpan);
			FormattersHelpers.stringConvertersHash[typeof(ushort)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromUInt16);
			FormattersHelpers.stringConvertersHash[typeof(uint)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromUInt32);
			FormattersHelpers.stringConvertersHash[typeof(ulong)] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromUInt64);
			FormattersHelpers.stringConvertersHash[typeof(byte[])] = new FormattersHelpers.CovertToStringDelegate(FormattersHelpers.ConvertFromBase64);
		}

		private static string ReadPropertyXml(XmlReader reader)
		{
			StringBuilder stringBuilder = new StringBuilder();
			XmlaReader xmlaReader = reader as XmlaReader;
			bool skipElements = true;
			try
			{
				if (xmlaReader != null)
				{
					skipElements = xmlaReader.SkipElements;
					xmlaReader.SkipElements = false;
				}
				while (reader.IsStartElement())
				{
					stringBuilder.Append(reader.ReadOuterXml());
				}
			}
			finally
			{
				if (xmlaReader != null)
				{
					xmlaReader.SkipElements = skipElements;
				}
			}
			return stringBuilder.ToString();
		}

		private static IDisposable GetWhitespaceHandlingRestorer(XmlReader reader, WhitespaceHandling handling)
		{
			if (reader is XmlaReader)
			{
				return ((XmlaReader)reader).GetWhitespaceHandlingRestorer(handling);
			}
			return FormattersHelpers.emptyRestorer;
		}

		private static string ReadWhiteSpace(XmlReader reader)
		{
			string text = string.Empty;
			while (reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace)
			{
				text += reader.Value;
				reader.Read();
			}
			return text;
		}
	}
}
