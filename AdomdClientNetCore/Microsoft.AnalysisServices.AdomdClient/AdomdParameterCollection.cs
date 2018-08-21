using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdParameterCollection : MarshalByRefObject, IDataParameterCollection, IList, ICollection, IEnumerable
	{
		private AdomdCommand parent;

		private ArrayList items;

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this.ValidateType(value);
				this[index] = (AdomdParameter)value;
			}
		}

		object IDataParameterCollection.this[string index]
		{
			get
			{
				if (index == null)
				{
					throw new ArgumentNullException("index");
				}
				int num = this.items.IndexOf(index);
				if (-1 == num)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(index), "key");
				}
				return this.items[num];
			}
			set
			{
				this.ValidateType(value);
				int index2 = this.items.IndexOf(index);
				this[index2] = (AdomdParameter)value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AdomdParameter this[int index]
		{
			get
			{
				this.RangeCheck(index);
				return (AdomdParameter)this.items[index];
			}
			set
			{
				this.RangeCheck(index);
				this.Replace(index, value);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public AdomdParameter this[string parameterName]
		{
			get
			{
				AdomdParameter adomdParameter = this.Find(parameterName);
				if (adomdParameter == null)
				{
					throw new ArgumentException(SR.Indexer_ObjectNotFound(parameterName), "parameterName");
				}
				return adomdParameter;
			}
			set
			{
				int index = this.RangeCheck(parameterName);
				this.Replace(index, value);
			}
		}

		private Type ItemType
		{
			get
			{
				return typeof(AdomdParameter);
			}
		}

		internal AdomdParameterCollection(AdomdCommand parent)
		{
			this.parent = parent;
			this.items = new ArrayList();
		}

		int IList.Add(object value)
		{
			this.Add((AdomdParameter)value);
			return this.Count - 1;
		}

		public void Clear()
		{
			int count = this.items.Count;
			for (int i = 0; i < count; i++)
			{
				((AdomdParameter)this.items[i]).Parent = null;
			}
			this.items.Clear();
		}

		bool IList.Contains(object value)
		{
			return this.Contains((AdomdParameter)value);
		}

		public bool Contains(string value)
		{
			return -1 != this.IndexOf(value);
		}

		public bool Contains(AdomdParameter value)
		{
			return -1 != this.IndexOf(value);
		}

		int IList.IndexOf(object value)
		{
			return this.IndexOf((AdomdParameter)value);
		}

		public int IndexOf(AdomdParameter value)
		{
			return this.items.IndexOf(value);
		}

		public void Insert(int index, AdomdParameter value)
		{
			this.Validate(-1, value);
			value.Parent = this;
			this.items.Insert(index, value);
		}

		void IList.Insert(int index, object value)
		{
			this.ValidateType(value);
			this.Insert(index, (AdomdParameter)value);
		}

		public void Remove(AdomdParameter value)
		{
			int num = this.IndexOf(value);
			if (-1 != num)
			{
				this.RemoveIndex(num);
				return;
			}
			throw new ArgumentException(SR.Property_DoesNotExist, "value");
		}

		void IList.Remove(object value)
		{
			this.ValidateType(value);
			this.Remove((AdomdParameter)value);
		}

		public void RemoveAt(int index)
		{
			this.RangeCheck(index);
			this.RemoveIndex(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		public int IndexOf(string parameterName)
		{
			int count = this.items.Count;
			for (int i = 0; i < count; i++)
			{
				if (parameterName == ((AdomdParameter)this.items[i]).ParameterName)
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveAt(string parameterName)
		{
			int index = this.RangeCheck(parameterName);
			this.RemoveIndex(index);
		}

		public void CopyTo(AdomdParameter[] array, int index)
		{
			this.items.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.items.CopyTo(array, index);
		}

		public AdomdParameter Add(AdomdParameter value)
		{
			this.Validate(-1, value);
			value.Parent = this;
			this.items.Add(value);
			return value;
		}

		public AdomdParameter Add(string parameterName, object value)
		{
			return this.Add(new AdomdParameter(parameterName, value));
		}

		public AdomdParameter Find(string parameterName)
		{
			if (parameterName == null)
			{
				throw new ArgumentNullException("parameterName");
			}
			int num = this.IndexOf(parameterName);
			if (num < 0)
			{
				return null;
			}
			return (AdomdParameter)this.items[num];
		}

		private void RangeCheck(int index)
		{
			if (index < 0 || this.Count <= index)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		private int RangeCheck(string parameterName)
		{
			int num = this.IndexOf(parameterName);
			if (num < 0)
			{
				throw new ArgumentOutOfRangeException("parameterName");
			}
			return num;
		}

		private void Replace(int index, AdomdParameter newValue)
		{
			this.Validate(index, newValue);
			((AdomdParameter)this.items[index]).Parent = null;
			newValue.Parent = this;
			this.items[index] = newValue;
		}

		internal void Validate(int index, AdomdParameter value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Parent != null)
			{
				if (this != value.Parent)
				{
					throw new ArgumentException(SR.Parameter_Parent_Mismatch, "value");
				}
				if (index != this.IndexOf(value.ParameterName))
				{
					throw new ArgumentException(SR.Parameter_Already_Exists(value.ParameterName), "value");
				}
			}
			string text = value.ParameterName;
			if (text.Length == 0)
			{
				index = 1;
				int num = 0;
				while (index < 2147483647 && num != -1)
				{
					text = "Parameter" + index.ToString(CultureInfo.InvariantCulture);
					num = this.IndexOf(text);
					index++;
				}
				if (-1 != num)
				{
					text = "Parameter" + Guid.NewGuid().ToString();
				}
				value.ParameterName = text;
			}
		}

		private void ValidateType(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this.ItemType.IsInstanceOfType(value))
			{
				throw new ArgumentException(SR.Parameter_Value_Wrong_Type, "value");
			}
		}

		private void RemoveIndex(int index)
		{
			((AdomdParameter)this.items[index]).Parent = null;
			this.items.RemoveAt(index);
		}
	}
}
