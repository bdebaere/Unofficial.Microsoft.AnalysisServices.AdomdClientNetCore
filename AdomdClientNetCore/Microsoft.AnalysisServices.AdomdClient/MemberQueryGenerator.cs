using System;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class MemberQueryGenerator
	{
		internal const int InternallyAddedDimensionPropertyCount = 2;

		private const string mdxListSeperator = ", ";

		private const string mdxStringOpenQuote = "\"";

		private const string mdxStringCloseQuote = "\"";

		private const string mdxIdentifyOpenQuote = "[";

		private const string mdxIdentifyCloseQuote = "]";

		private const string mdxSpace = " ";

		private MemberQueryGenerator()
		{
		}

		public static string GetMemberQuery(string memberSet, string dimensionPropertiesClause, CubeDef cube)
		{
			string text = AdomdUtils.Enquote(cube.Name, "[", "]");
			return string.Format(CultureInfo.InvariantCulture, "SELECT {0} {1} ON 0, {{}} ON 1 FROM {2}", new object[]
			{
				memberSet,
				dimensionPropertiesClause,
				text
			});
		}

		public static string GetDimensionPropertiesClause(string[] userSuppliedProperties)
		{
			string text = "DIMENSION PROPERTIES MEMBER_NAME, MEMBER_TYPE";
			if (userSuppliedProperties.Length > 0)
			{
				text = text + ",  " + string.Join(", ", userSuppliedProperties);
			}
			return text;
		}

		public static string GetBaseSetMemberChilden(Member parentMember)
		{
			return string.Format(CultureInfo.InvariantCulture, "ADDCALCULATEDMEMBERS( {0}.CHILDREN )", new object[]
			{
				parentMember.UniqueName
			});
		}

		public static string GetBaseSetLevelMembers(Level level)
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}.ALLMEMBERS", new object[]
			{
				level.UniqueName
			});
		}

		public static string GetFilteredAndRangedMemberSet(string baseSet, string hierarchyUniqueName, long start, long count, MemberFilter[] filters)
		{
			string text;
			if (filters.Length > 0)
			{
				string filterExpression = MemberQueryGenerator.GetFilterExpression(hierarchyUniqueName, filters);
				text = MemberQueryGenerator.GetSetWithFilter(baseSet, filterExpression);
			}
			else
			{
				text = baseSet;
			}
			string result;
			if (count > -1L)
			{
				result = MemberQueryGenerator.GetSetWithRange(text, start, count);
			}
			else
			{
				result = text;
			}
			return result;
		}

		private static string GetSetWithRange(string filteredSet, long start, long count)
		{
			return string.Format(CultureInfo.InvariantCulture, "SUBSET( {0}, {1}, {2} )", new object[]
			{
				filteredSet,
				start,
				count
			});
		}

		private static string GetSetWithFilter(string baseSet, string filterExpression)
		{
			return string.Format(CultureInfo.InvariantCulture, "FILTER( {0}, {1} )", new object[]
			{
				baseSet,
				filterExpression
			});
		}

		private static string GetFilterExpression(string hierarchyUniqueName, MemberFilter[] filters)
		{
			string[] array = new string[filters.Length];
			for (int i = 0; i < filters.Length; i++)
			{
				array[i] = "( " + MemberQueryGenerator.GetIndividualFilter(hierarchyUniqueName, filters[i]) + " )";
			}
			return string.Join(" AND ", array);
		}

		private static string GetIndividualFilter(string hierarchyUniqueName, MemberFilter filter)
		{
			string propertyReference = MemberQueryGenerator.GetPropertyReference(hierarchyUniqueName, filter.PropertyName);
			string result;
			switch (filter.FilterType)
			{
			case MemberFilterType.Equals:
				result = MemberQueryGenerator.GetEqualFilter(propertyReference, filter.PropertyValue);
				break;
			case MemberFilterType.BeginsWith:
				result = MemberQueryGenerator.GetBeginsWithFilter(propertyReference, filter.PropertyValue);
				break;
			case MemberFilterType.EndsWith:
				result = MemberQueryGenerator.GetEndsWithFilter(propertyReference, filter.PropertyValue);
				break;
			case MemberFilterType.Contains:
				result = MemberQueryGenerator.GetContainsFilter(propertyReference, filter.PropertyValue);
				break;
			default:
				result = MemberQueryGenerator.GetEqualFilter(propertyReference, filter.PropertyValue);
				break;
			}
			return result;
		}

		private static string GetPropertyReference(string hierarchyUniqueName, string propertyName)
		{
			string text;
			if (propertyName == "Name" || propertyName == "UniqueName")
			{
				text = propertyName;
			}
			else
			{
				propertyName = MemberQueryGenerator.EnquoteMdxString(propertyName);
				text = string.Format(CultureInfo.InvariantCulture, "Properties({0})", new object[]
				{
					propertyName
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.CurrentMember.{1}", new object[]
			{
				hierarchyUniqueName,
				text
			});
		}

		private static string GetEqualFilter(string propertyReference, string propertyValue)
		{
			propertyValue = MemberQueryGenerator.EnquoteMdxString(propertyValue);
			return string.Format(CultureInfo.InvariantCulture, "{0} = {1}", new object[]
			{
				propertyReference,
				propertyValue
			});
		}

		private static string GetBeginsWithFilter(string propertyReference, string propertyValue)
		{
			int length = propertyValue.Length;
			propertyValue = MemberQueryGenerator.EnquoteMdxString(propertyValue);
			return string.Format(CultureInfo.InvariantCulture, "LEFT( {0}, {1} ) = {2}", new object[]
			{
				propertyReference,
				length,
				propertyValue
			});
		}

		private static string GetEndsWithFilter(string propertyReference, string propertyValue)
		{
			int length = propertyValue.Length;
			propertyValue = MemberQueryGenerator.EnquoteMdxString(propertyValue);
			return string.Format(CultureInfo.InvariantCulture, "RIGHT( {0}, {1} ) = {2}", new object[]
			{
				propertyReference,
				length,
				propertyValue
			});
		}

		private static string GetContainsFilter(string propertyReference, string propertyValue)
		{
			propertyValue = MemberQueryGenerator.EnquoteMdxString(propertyValue);
			return string.Format(CultureInfo.InvariantCulture, "INSTR( {0}, {1} ) > 0", new object[]
			{
				propertyReference,
				propertyValue
			});
		}

		private static string EnquoteMdxString(string stringValue)
		{
			return AdomdUtils.Enquote(stringValue, "\"", "\"");
		}
	}
}
