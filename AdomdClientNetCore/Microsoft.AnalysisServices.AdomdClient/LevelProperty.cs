using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class LevelProperty : ISubordinateObject
	{
		private DataRow levelPropRow;

		private Level parentLevel;

		private AdomdConnection connection;

		private int propertyOrdinal = -1;

		private int hashCode;

		private bool hashCodeCalculated;

		private static string descriptionColumn = "DESCRIPTION";

		internal static string levelPropNameColumn = "PROPERTY_NAME";

		private static string levelPropCaptionColumn = "PROPERTY_CAPTION";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.levelPropRow, LevelProperty.levelPropNameColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return this.parentLevel.UniqueName + "." + AdomdUtils.Enquote(this.Name, "[", "]");
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.levelPropRow, LevelProperty.levelPropCaptionColumn).ToString();
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.levelPropRow, LevelProperty.descriptionColumn).ToString();
			}
		}

		public Level ParentLevel
		{
			get
			{
				return this.parentLevel;
			}
		}

		object ISubordinateObject.Parent
		{
			get
			{
				return this.parentLevel;
			}
		}

		int ISubordinateObject.Ordinal
		{
			get
			{
				return this.propertyOrdinal;
			}
		}

		Type ISubordinateObject.Type
		{
			get
			{
				return typeof(LevelProperty);
			}
		}

		internal LevelProperty(AdomdConnection connection, DataRow levelPropRow, Level level, int propertyOrdinal)
		{
			this.connection = connection;
			this.levelPropRow = levelPropRow;
			this.parentLevel = level;
			this.propertyOrdinal = propertyOrdinal;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public override int GetHashCode()
		{
			if (!this.hashCodeCalculated)
			{
				this.hashCode = AdomdUtils.GetHashCode(this);
				this.hashCodeCalculated = true;
			}
			return this.hashCode;
		}

		public override bool Equals(object obj)
		{
			return AdomdUtils.Equals(this, obj as ISubordinateObject);
		}

		public static bool operator ==(LevelProperty o1, LevelProperty o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(LevelProperty o1, LevelProperty o2)
		{
			return !(o1 == o2);
		}
	}
}
