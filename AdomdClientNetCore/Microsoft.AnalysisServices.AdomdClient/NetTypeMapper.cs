using System;
using System.Collections;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class NetTypeMapper
	{
		private static readonly Hashtable netTypes;

		private NetTypeMapper()
		{
		}

		static NetTypeMapper()
		{
			NetTypeMapper.netTypes = new Hashtable();
			NetTypeMapper.IncludeStandardTypes();
		}

		private static void IncludeStandardTypes()
		{
			NetTypeMapper.netTypes.Add("string", typeof(string));
			NetTypeMapper.netTypes.Add("boolean", typeof(bool));
			NetTypeMapper.netTypes.Add("decimal", typeof(decimal));
			NetTypeMapper.netTypes.Add("float", typeof(float));
			NetTypeMapper.netTypes.Add("double", typeof(double));
			NetTypeMapper.netTypes.Add("byte", typeof(sbyte));
			NetTypeMapper.netTypes.Add("unsignedByte", typeof(byte));
			NetTypeMapper.netTypes.Add("short", typeof(short));
			NetTypeMapper.netTypes.Add("unsignedShort", typeof(ushort));
			NetTypeMapper.netTypes.Add("int", typeof(int));
			NetTypeMapper.netTypes.Add("unsignedInt", typeof(uint));
			NetTypeMapper.netTypes.Add("long", typeof(long));
			NetTypeMapper.netTypes.Add("unsignedLong", typeof(ulong));
			NetTypeMapper.netTypes.Add("dateTime", typeof(DateTime));
			NetTypeMapper.netTypes.Add("uuid", typeof(Guid));
			NetTypeMapper.netTypes.Add("base64Binary", typeof(byte[]));
		}

		public static Type GetNetType(string xmlType)
		{
			return (Type)NetTypeMapper.netTypes[xmlType];
		}

		public static Type GetNetTypeWithPrefix(string xmlTypeWithPrefix)
		{
			int num = xmlTypeWithPrefix.IndexOf(':');
			string xmlType;
			if (num != -1)
			{
				xmlType = xmlTypeWithPrefix.Substring(num + 1);
			}
			else
			{
				xmlType = xmlTypeWithPrefix;
			}
			return NetTypeMapper.GetNetType(xmlType);
		}
	}
}
