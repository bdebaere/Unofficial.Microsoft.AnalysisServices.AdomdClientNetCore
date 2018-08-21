using System;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdParameter : MarshalByRefObject, IDbDataParameter, IDataParameter, ICloneable
	{
		private AdomdParameterCollection parent;

		private string parameterName;

		private object parameterValue;

		public DbType DbType
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public ParameterDirection Direction
		{
			get
			{
				return ParameterDirection.Input;
			}
			set
			{
				if (ParameterDirection.Input != value)
				{
					throw new NotSupportedException();
				}
			}
		}

		public bool IsNullable
		{
			get
			{
				return false;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public string ParameterName
		{
			get
			{
				if (this.parameterName == null)
				{
					return string.Empty;
				}
				return this.parameterName;
			}
			set
			{
				if (this.parameterName != value)
				{
					this.parameterName = value;
				}
			}
		}

		internal AdomdParameterCollection Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public string SourceColumn
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public DataRowVersion SourceVersion
		{
			get
			{
				return DataRowVersion.Current;
			}
			set
			{
				if (value != DataRowVersion.Current)
				{
					throw new NotSupportedException();
				}
			}
		}

		public object Value
		{
			get
			{
				return this.parameterValue;
			}
			set
			{
				this.CheckParameterValueType(value, "value");
				this.parameterValue = value;
			}
		}

		public byte Precision
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public byte Scale
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Size
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public AdomdParameter()
		{
		}

		public AdomdParameter(string parameterName, object value)
		{
			this.CheckParameterValueType(value, "value");
			this.parameterName = parameterName;
			this.parameterValue = value;
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		public AdomdParameter Clone()
		{
			return new AdomdParameter(this.parameterName, this.parameterValue);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		private void CheckParameterValueType(object value, string argumentName)
		{
			if (value == null)
			{
				throw new ArgumentNullException(argumentName);
			}
			Type type = value.GetType();
			if (!XmlaClient.IsTypeSupportedForParameters(type))
			{
				throw new ArgumentException(SR.ArgumentErrorUnsupportedParameterType(type.FullName), argumentName);
			}
		}
	}
}
