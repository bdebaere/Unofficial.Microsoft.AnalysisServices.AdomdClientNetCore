using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class XmlTypeMapper
	{
		private static readonly Hashtable xmlTypes;

		private XmlTypeMapper()
		{
		}

		static XmlTypeMapper()
		{
			XmlTypeMapper.xmlTypes = new Hashtable();
			XmlTypeMapper.IncludeStandardTypes();
		}

		private static void IncludeStandardTypes()
		{
			XmlTypeMapper.xmlTypes.Add(typeof(string), "xsd:string");
			XmlTypeMapper.xmlTypes.Add(typeof(bool), "xsd:boolean");
			XmlTypeMapper.xmlTypes.Add(typeof(decimal), "xsd:decimal");
			XmlTypeMapper.xmlTypes.Add(typeof(float), "xsd:float");
			XmlTypeMapper.xmlTypes.Add(typeof(double), "xsd:double");
			XmlTypeMapper.xmlTypes.Add(typeof(sbyte), "xsd:byte");
			XmlTypeMapper.xmlTypes.Add(typeof(byte), "xsd:unsignedByte");
			XmlTypeMapper.xmlTypes.Add(typeof(short), "xsd:short");
			XmlTypeMapper.xmlTypes.Add(typeof(ushort), "xsd:unsignedShort");
			XmlTypeMapper.xmlTypes.Add(typeof(int), "xsd:int");
			XmlTypeMapper.xmlTypes.Add(typeof(uint), "xsd:unsignedInt");
			XmlTypeMapper.xmlTypes.Add(typeof(long), "xsd:long");
			XmlTypeMapper.xmlTypes.Add(typeof(ulong), "xsd:unsignedLong");
			XmlTypeMapper.xmlTypes.Add(typeof(DateTime), "xsd:dateTime");
			XmlTypeMapper.xmlTypes.Add(typeof(Guid), "uuid");
			XmlTypeMapper.xmlTypes.Add(typeof(byte[]), "xsd:base64Binary");
			XmlTypeMapper.xmlTypes.Add(typeof(DBNull), string.Empty);
		}

		public static bool IsTypeSupported(Type type)
		{
			return XmlTypeMapper.xmlTypes.ContainsKey(type);
		}

		public static string GetXmlType(Type type)
		{
			return XmlTypeMapper.xmlTypes[type].ToString();
		}
	}
}
