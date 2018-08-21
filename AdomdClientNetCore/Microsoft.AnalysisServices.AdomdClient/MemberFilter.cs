using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public class MemberFilter
	{
		private string propertyName;

		private string propertyValue;

		private MemberFilterType filterType;

		public string PropertyName
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException(null, "value");
				}
				this.propertyName = value;
			}
		}

		public string PropertyValue
		{
			get
			{
				return this.propertyValue;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.propertyValue = value;
			}
		}

		public MemberFilterType FilterType
		{
			get
			{
				return this.filterType;
			}
			set
			{
				if (!MemberFilterTypeChecker.IsValidMemberFilterType(value))
				{
					throw new ArgumentException(null, "value");
				}
				this.filterType = value;
			}
		}

		public MemberFilter(string propertyName, string propertyValue) : this(propertyName, MemberFilterType.Equals, propertyValue)
		{
		}

		public MemberFilter(string propertyName, MemberFilterType filterType, string propertyValue)
		{
			if (propertyName == null)
			{
				throw new ArgumentNullException("propertyName");
			}
			if (propertyName.Length == 0)
			{
				throw new ArgumentException(null, "propertyName");
			}
			if (!MemberFilterTypeChecker.IsValidMemberFilterType(filterType))
			{
				throw new ArgumentException(null, "filterType");
			}
			if (propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue");
			}
			this.propertyName = propertyName;
			this.propertyValue = propertyValue;
			this.filterType = filterType;
		}
	}
}
