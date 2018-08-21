using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AdomdUtils
	{
		internal delegate string GetInvalidatedMessageDelegate();

		private const string treeOpRestrictionName = "TREE_OP";

		private const int MDTREEOP_FETCH_BLOB_PROPERTIES = 64;

		private const int MDTREEOP_SELF = 8;

		internal const string UnqualifiedPropertyName = "MemberPropertyUnqualifiedName";

		internal const string NamesHashtablePropertyName = "MemberPropertiesNamesHash";

		private const string propertyNameGroupName = "propertyName";

		private const string IdentifierRegExpr = "(((?<propertyName>(\\w+)))|(\\[(?<propertyName>((\\s*)(([^\\s\\]])|(\\]\\]))((([^\\]])|(\\]\\])))*))\\]))";

		private static string cubeSourceRestrictionName = "CUBE_SOURCE";

		private static int cubeSourceCube = 1;

		private static int cubeSourceDimension = 2;

		private static int cubeSourceAll = AdomdUtils.cubeSourceCube | AdomdUtils.cubeSourceDimension;

		private static string MetadataHashCodeTemplate = "{0}#{1}#{2}#{3}#{4}#{5}";

		private static string namePropertyFormat = "^((.*)(((?<propertyName>(\\w+)))|(\\[(?<propertyName>((\\s*)(([^\\s\\]])|(\\]\\]))((([^\\]])|(\\]\\])))*))\\])))$";

		private static Regex namePropertyRegex = new Regex(AdomdUtils.namePropertyFormat, RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.RightToLeft | RegexOptions.CultureInvariant);

		internal static readonly Version UnknownVersion = new Version("0.0.0.0");

		private AdomdUtils()
		{
		}

		internal static object GetProperty(DataRow dataRow, string propertyName)
		{
			if (dataRow == null)
			{
				throw new ArgumentNullException("dataRow");
			}
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			if (!dataRow.Table.Columns.Contains(propertyName))
			{
				throw new NotSupportedException(SR.Property_UnknownProperty(propertyName));
			}
			if (dataRow.Table.Columns[propertyName].AutoIncrement)
			{
				string relationName = dataRow.Table.TableName + propertyName;
				return dataRow.GetChildRows(relationName);
			}
			object obj = dataRow[propertyName];
			if (obj is XmlaError)
			{
				throw new AdomdErrorResponseException((XmlaError)obj);
			}
			return obj;
		}

		internal static object GetProperty(DataRow dataRow, int index)
		{
			if (dataRow == null)
			{
				throw new ArgumentNullException("dataRow");
			}
			object obj = dataRow[index];
			if (obj is XmlaError)
			{
				throw new AdomdErrorResponseException((XmlaError)obj);
			}
			return obj;
		}

		internal static DataRowCollection GetRows(AdomdConnection connection, string requestType, ListDictionary restrictions)
		{
			RowsetFormatter rowsetFormatter = connection.IDiscoverProvider.Discover(requestType, restrictions);
			return rowsetFormatter.MainRowsetTable.Rows;
		}

		internal static void PopulateSymetry(IAdomdBaseObject iBaseObject)
		{
			if (iBaseObject.Connection == null)
			{
				throw new NotSupportedException(SR.NotSupportedWhenConnectionMissing);
			}
			AdomdUtils.CheckConnectionOpened(iBaseObject.Connection);
			if (iBaseObject.CubeName == null)
			{
				throw new NotSupportedException(SR.NotSupportedByProvider);
			}
			IAdomdBaseObject adomdBaseObject = (IAdomdBaseObject)iBaseObject.Connection.GetObjectData(iBaseObject.SchemaObjectType, iBaseObject.CubeName, iBaseObject.InternalUniqueName);
			iBaseObject.MetadataData = adomdBaseObject.MetadataData;
			iBaseObject.ParentObject = adomdBaseObject.ParentObject;
			iBaseObject.IsMetadata = true;
		}

		internal static void CheckCopyToParameters(Array array, int index, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank > 1)
			{
				throw new ArgumentException(SR.ICollection_CannotCopyToMultidimensionalArray, "array");
			}
			if (index < 0 || index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.Length - index < count)
			{
				throw new ArgumentException(SR.ICollection_NotEnoughSpaceToCopyTo(array.Length - index, count), "index");
			}
		}

		internal static void AddCubeSourceRestrictionIfApplicable(AdomdConnection connection, ListDictionary restrictions)
		{
			if (connection.IsPostYukonProvider())
			{
				restrictions.Add(AdomdUtils.cubeSourceRestrictionName, AdomdUtils.cubeSourceAll);
			}
		}

		internal static void AddMemberBinaryRestrictionIfApplicable(AdomdConnection connection, ListDictionary restrictions)
		{
			if (connection.IsPostYukonProvider())
			{
				restrictions.Add("TREE_OP", 72);
			}
		}

		internal static void AddObjectVisibilityRestrictionIfApplicable(AdomdConnection connection, string requestType, ListDictionary restrictions)
		{
			if (AdomdUtils.ShouldAddObjectVisibilityRestriction(connection))
			{
				string text;
				if (requestType == DimensionCollectionInternal.schemaName)
				{
					text = "DIMENSION_VISIBILITY";
				}
				else if (requestType == HierarchyCollectionInternal.schemaName)
				{
					text = "HIERARCHY_VISIBILITY";
				}
				else if (requestType == LevelCollectionInternal.schemaName)
				{
					text = "LEVEL_VISIBILITY";
				}
				else if (requestType == MeasureCollectionInternal.schemaName)
				{
					text = "MEASURE_VISIBILITY";
				}
				else if (requestType == "MDSCHEMA_PROPERTIES")
				{
					text = "PROPERTY_VISIBILITY";
				}
				else
				{
					text = null;
				}
				if (text != null)
				{
					restrictions.Add(text, 3);
				}
			}
		}

		internal static bool ShouldAddObjectVisibilityRestriction(AdomdConnection connection)
		{
			return connection.ShowHiddenObjects && connection.IsPostYukonProvider();
		}

		internal static void CopyRestrictions(ListDictionary sourceRestrictions, ListDictionary destinationRestrictions)
		{
			foreach (object current in sourceRestrictions.Keys)
			{
				destinationRestrictions.Add(current, sourceRestrictions[current]);
			}
		}

		internal static void FillNamesHashTable(DataTable table, Hashtable hash)
		{
			AdomdUtils.FillNamesHashTable(table, hash, 0);
		}

		internal static void FillNamesHashTable(DataTable table, Hashtable hash, int startColumn)
		{
			if (table == null || hash == null)
			{
				return;
			}
			hash.Clear();
			for (int i = startColumn; i < table.Columns.Count; i++)
			{
				string caption = table.Columns[i].Caption;
				if (hash[caption] == null)
				{
					hash[caption] = i;
				}
			}
		}

		internal static void FillPropertiesNamesHashTable(DataTable table, Hashtable hash, int startColumn)
		{
			if (table == null || hash == null)
			{
				return;
			}
			hash.Clear();
			for (int i = startColumn; i < table.Columns.Count; i++)
			{
				DataColumn dataColumn = table.Columns[i];
				if (!dataColumn.ExtendedProperties.ContainsKey("MemberPropertyUnqualifiedName"))
				{
					dataColumn.ExtendedProperties["MemberPropertyUnqualifiedName"] = AdomdUtils.UnQualifyPropertyName(dataColumn.Caption);
				}
				string key = dataColumn.ExtendedProperties["MemberPropertyUnqualifiedName"] as string;
				if (hash[key] == null)
				{
					hash[key] = i;
				}
			}
		}

		internal static string UnQualifyPropertyName(string name)
		{
			Match match = AdomdUtils.namePropertyRegex.Match(name);
			if (match.Success)
			{
				return match.Groups["propertyName"].Value;
			}
			return name;
		}

		internal static bool Equals(IMetadataObject object1, IMetadataObject object2)
		{
			return object.ReferenceEquals(object1, object2) || (object1 != null && object2 != null && !(object1.UniqueName != object2.UniqueName) && !(object1.CubeName != object2.CubeName) && !(object1.Type != object2.Type) && object.ReferenceEquals(object1.Connection, object2.Connection) && (object1.Connection == null || (object1.SessionId == object2.SessionId && object1.Catalog == object2.Catalog)));
		}

		internal static bool Equals(ISubordinateObject object1, ISubordinateObject object2)
		{
			return object.ReferenceEquals(object1, object2) || (object1 != null && object2 != null && object1.Ordinal == object2.Ordinal && !(object1.Type != object2.Type) && object.Equals(object1.Parent, object2.Parent));
		}

		internal static int GetHashCode(IMetadataObject metadataObject)
		{
			string text = string.Format(CultureInfo.InvariantCulture, AdomdUtils.MetadataHashCodeTemplate, new object[]
			{
				metadataObject.Connection.GetHashCode().ToString(CultureInfo.InvariantCulture),
				metadataObject.SessionId,
				metadataObject.Catalog,
				metadataObject.CubeName,
				metadataObject.Type.GetHashCode(),
				metadataObject.UniqueName
			});
			return text.GetHashCode();
		}

		internal static int GetHashCode(ISubordinateObject propertyObject)
		{
			return propertyObject.Ordinal;
		}

		internal static string GetDataTableFilter(string columnName, string columnValue)
		{
			if (columnValue == null)
			{
				return string.Format(CultureInfo.InvariantCulture, "( {0} is NULL )", new object[]
				{
					columnName
				});
			}
			string text = AdomdUtils.Enquote(columnValue, "'", "'");
			return string.Format(CultureInfo.InvariantCulture, "( {0} = {1} )", new object[]
			{
				columnName,
				text
			});
		}

		internal static string Enquote(string stringValue, string openQuote, string closeQuote)
		{
			if (openQuote == null || closeQuote == null)
			{
				return stringValue;
			}
			string str = stringValue.Replace(closeQuote, closeQuote + closeQuote);
			return openQuote + str + closeQuote;
		}

		internal static void CheckConnectionOpened(AdomdConnection connection)
		{
			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}
			if (connection.State == ConnectionState.Open)
			{
				return;
			}
			if (connection.UserOpened)
			{
				throw new AdomdConnectionException(SR.Command_ConnectionIsNotOpened);
			}
			throw new InvalidOperationException(SR.Command_ConnectionIsNotOpened);
		}

		internal static void EnsureCacheNotAbandoned(MetadataCacheState state)
		{
			if (state == MetadataCacheState.Abandoned)
			{
				throw new InvalidOperationException(SR.MetadataCache_Abandoned);
			}
		}

		internal static void EnsureCacheNotInvalid(MetadataCacheState state, AdomdUtils.GetInvalidatedMessageDelegate msgDelegate)
		{
			if (state == MetadataCacheState.Invalid)
			{
				throw new AdomdCacheExpiredException(msgDelegate());
			}
		}

		internal static bool IsPostYukonVersion(Version serverVersion)
		{
			return serverVersion != null && serverVersion.Major >= 9;
		}

		internal static Version ConvertVersionStringToVersionObject(string versionString)
		{
			Version result = AdomdUtils.UnknownVersion;
			if (!string.IsNullOrEmpty(versionString))
			{
				try
				{
					result = new Version(versionString);
				}
				catch (ArgumentNullException)
				{
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				catch (ArgumentException)
				{
				}
				catch (FormatException)
				{
				}
				catch (OverflowException)
				{
				}
			}
			return result;
		}
	}
}
