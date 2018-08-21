using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal abstract class XmlaPropertyCollectionBase : IList, IDictionary, ICollection, IEnumerable
	{
		private class HashHelper : IEqualityComparer, IComparer
		{
			internal HashHelper()
			{
			}

			bool IEqualityComparer.Equals(object x, object y)
			{
				return ((IComparer)this).Compare(x, y) == 0;
			}

			int IComparer.Compare(object x, object y)
			{
				IXmlaPropertyKey xmlaPropertyKey = (IXmlaPropertyKey)x;
				IXmlaPropertyKey xmlaPropertyKey2 = (IXmlaPropertyKey)y;
				int num = string.Compare(xmlaPropertyKey.Name, xmlaPropertyKey2.Name, StringComparison.Ordinal);
				if (num == 0)
				{
					num = string.Compare(xmlaPropertyKey.Name, xmlaPropertyKey2.Name, StringComparison.Ordinal);
				}
				return num;
			}

			int IEqualityComparer.GetHashCode(object obj)
			{
				IXmlaPropertyKey xmlaPropertyKey = (IXmlaPropertyKey)obj;
				string text = string.Format(CultureInfo.InvariantCulture, "{0}#{1}", new object[]
				{
					(xmlaPropertyKey.Name != null) ? xmlaPropertyKey.Name.GetHashCode().ToString(CultureInfo.InvariantCulture) : string.Empty,
					(xmlaPropertyKey.Namespace != null) ? xmlaPropertyKey.Namespace.GetHashCode().ToString(CultureInfo.InvariantCulture) : string.Empty
				});
				return text.GetHashCode();
			}
		}

		private class XmlaPropertyDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			private XmlaPropertyCollectionBase collection;

			private int current = -1;

			public DictionaryEntry Entry
			{
				get
				{
					return new DictionaryEntry(((IDictionaryEnumerator)this).Key, ((IDictionaryEnumerator)this).Value);
				}
			}

			public IXmlaPropertyKey Key
			{
				get
				{
					IXmlaPropertyKey result;
					try
					{
						result = this.collection[this.current];
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException();
					}
					return result;
				}
			}

			object IDictionaryEnumerator.Key
			{
				get
				{
					return this.Key;
				}
			}

			public object Value
			{
				get
				{
					object value;
					try
					{
						value = this.collection[this.current].Value;
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException();
					}
					return value;
				}
			}

			public DictionaryEntry Current
			{
				get
				{
					return ((IDictionaryEnumerator)this).Entry;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal XmlaPropertyDictionaryEnumerator(XmlaPropertyCollectionBase collection)
			{
				this.collection = collection;
				this.current = -1;
			}

			public void Reset()
			{
				this.current = -1;
			}

			public bool MoveNext()
			{
				return ++this.current < this.collection.Count;
			}
		}

		private Hashtable hashStore;

		private ArrayList items;

		protected abstract Type ItemType
		{
			get;
		}

		protected abstract object Parent
		{
			get;
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.items.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IXmlaProperty this[int index]
		{
			get
			{
				this.RangeCheck(index);
				return (IXmlaProperty)this.items[index];
			}
			set
			{
				this.RangeCheck(index);
				this.Replace(index, value);
			}
		}

		public object this[IXmlaPropertyKey key]
		{
			get
			{
				int num = this.IndexOf(key);
				if (num == -1)
				{
					throw new ArgumentException(SR.Property_DoesNotExist, "key");
				}
				return this[num];
			}
			set
			{
				int num = this.IndexOf(key);
				if (num == -1)
				{
					this.Add(this.CreateBasePropertyObject(key, value));
					return;
				}
				((IXmlaProperty)this.items[num]).Value = value;
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
				this[index] = (IXmlaProperty)value;
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.hashStore.Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				object[] array = new object[this.Count];
				for (int i = 0; i < this.Count; i++)
				{
					array[i] = ((IXmlaProperty)this.items[i]).Value;
				}
				return array;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				this.ValidateKeyType(key);
				return this[(IXmlaPropertyKey)key];
			}
			set
			{
				this.ValidateKeyType(key);
				this[(IXmlaPropertyKey)key] = value;
			}
		}

		internal XmlaPropertyCollectionBase()
		{
			XmlaPropertyCollectionBase.HashHelper equalityComparer = new XmlaPropertyCollectionBase.HashHelper();
			this.hashStore = new Hashtable(equalityComparer);
			this.items = new ArrayList();
		}

		protected abstract IXmlaProperty CreateBasePropertyObject(IXmlaPropertyKey key, object propertyValue);

		void ICollection.CopyTo(Array array, int index)
		{
			this.items.CopyTo(array, index);
		}

		public IXmlaProperty Add(IXmlaProperty value)
		{
			this.Validate(-1, value);
			value.Parent = this.Parent;
			int num = this.items.Add(value);
			this.hashStore[value] = num;
			return value;
		}

		int IList.Add(object value)
		{
			this.ValidateType(value);
			this.Add((IXmlaProperty)value);
			return this.Count - 1;
		}

		public void Clear()
		{
			int count = this.items.Count;
			for (int i = 0; i < count; i++)
			{
				((IXmlaProperty)this.items[i]).Parent = null;
			}
			this.items.Clear();
			this.hashStore.Clear();
		}

		public bool Contains(IXmlaProperty property)
		{
			return -1 != this.IndexOf(property);
		}

		public bool Contains(IXmlaPropertyKey key)
		{
			return -1 != this.IndexOf(key);
		}

		bool IList.Contains(object value)
		{
			this.ValidateType(value);
			return this.Contains((IXmlaProperty)value);
		}

		public int IndexOf(IXmlaProperty property)
		{
			return this.items.IndexOf(property);
		}

		public int IndexOf(IXmlaPropertyKey key)
		{
			int result = -1;
			if (this.hashStore.ContainsKey(key))
			{
				result = (int)this.hashStore[key];
			}
			return result;
		}

		int IList.IndexOf(object value)
		{
			this.ValidateType(value);
			return this.IndexOf((IXmlaProperty)value);
		}

		public void Insert(int index, IXmlaProperty value)
		{
			this.Validate(-1, value);
			value.Parent = this.Parent;
			this.items.Insert(index, value);
			this.hashStore[value] = index;
			for (int i = this.items.Count - 1; i > index; i--)
			{
				this.hashStore[(IXmlaProperty)this.items[i]] = i;
			}
		}

		void IList.Insert(int index, object value)
		{
			this.ValidateType(value);
			this.Insert(index, (IXmlaProperty)value);
		}

		public void Remove(IXmlaProperty value)
		{
			this.ValidateType(value);
			int num = this.IndexOf(value);
			if (-1 != num)
			{
				this.RemoveIndex(num);
				return;
			}
			throw new ArgumentException(SR.Property_DoesNotExist, "value");
		}

		public void Remove(IXmlaPropertyKey value)
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
			this.Remove((IXmlaProperty)value);
		}

		public void RemoveAt(int index)
		{
			this.RangeCheck(index);
			this.RemoveIndex(index);
		}

		void IDictionary.Add(object key, object value)
		{
			this.ValidateKeyType(key);
			this[(IXmlaPropertyKey)key] = value;
		}

		bool IDictionary.Contains(object key)
		{
			this.ValidateKeyType(key);
			return this.Contains((IXmlaPropertyKey)key);
		}

		void IDictionary.Remove(object key)
		{
			this.ValidateKeyType(key);
			this.Remove((IXmlaPropertyKey)key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new XmlaPropertyCollectionBase.XmlaPropertyDictionaryEnumerator(this);
		}

		public IEnumerator GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		internal void ChangeName(IXmlaProperty property, string newName)
		{
			XmlaPropertyKey key = new XmlaPropertyKey(newName, property.Namespace);
			int num = this.RemoveKey(key, property);
			property.Name = newName;
			this.hashStore[property] = num;
		}

		internal void ChangeNamespace(IXmlaProperty property, string newNamespace)
		{
			XmlaPropertyKey key = new XmlaPropertyKey(property.Name, newNamespace);
			int num = this.RemoveKey(key, property);
			property.Namespace = newNamespace;
			this.hashStore[property] = num;
		}

		internal void Validate(int index, IXmlaProperty value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Parent != null && this.Parent != value.Parent)
			{
				throw new ArgumentException(SR.Property_Parent_Mismatch, "value");
			}
			if (index != this.IndexOf(value))
			{
				throw new ArgumentException(SR.Property_Already_Exists, "value");
			}
			string name = value.Name;
			if (name.Length == 0)
			{
				throw new ArgumentException(SR.Connection_PropertyNameEmpty, "Name");
			}
		}

		private void RangeCheck(int index)
		{
			if (index < 0 || this.Count <= index)
			{
				throw new ArgumentOutOfRangeException("index");
			}
		}

		private void Replace(int index, IXmlaProperty newValue)
		{
			this.Validate(index, newValue);
			IXmlaProperty xmlaProperty = (IXmlaProperty)this.items[index];
			xmlaProperty.Parent = null;
			newValue.Parent = this.Parent;
			this.items[index] = newValue;
			this.hashStore.Remove(xmlaProperty);
			this.hashStore.Add(newValue, index);
		}

		private void ValidateType(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this.ItemType.IsInstanceOfType(value))
			{
				throw new ArgumentException(SR.Property_Value_Wrong_Type, "value");
			}
		}

		private void ValidateKeyType(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!typeof(IXmlaPropertyKey).IsInstanceOfType(value))
			{
				throw new ArgumentException(SR.Property_Key_Wrong_Type, "value");
			}
		}

		private void RemoveIndex(int index)
		{
			IXmlaProperty xmlaProperty = (IXmlaProperty)this.items[index];
			xmlaProperty.Parent = null;
			this.items.RemoveAt(index);
			this.hashStore.Remove(xmlaProperty);
			for (int i = index; i < this.items.Count; i++)
			{
				this.hashStore[(IXmlaProperty)this.items[i]] = i;
			}
		}

		private int RemoveKey(XmlaPropertyKey key, IXmlaProperty property)
		{
			if (this.hashStore.ContainsKey(key))
			{
				throw new ArgumentException(SR.Property_Already_Exists, "key");
			}
			int result = this.IndexOf(property);
			this.hashStore.Remove(property);
			return result;
		}
	}
}
