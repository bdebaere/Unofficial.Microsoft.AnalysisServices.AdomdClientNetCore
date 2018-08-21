using System;
using System.Collections;
using System.Data;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class PropertyCollection : ICollection, IEnumerable
	{
		public struct Enumerator : IEnumerator
		{
			private int currentIndex;

			private PropertyCollection properties;

			public Property Current
			{
				get
				{
					Property result;
					try
					{
						result = this.properties[this.currentIndex];
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException();
					}
					return result;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			internal Enumerator(PropertyCollection properties)
			{
				this.properties = properties;
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				return ++this.currentIndex < this.properties.Count;
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
		}

		private const int standardMetadataOffset = 1;

		private const string propertiesColumnsHash = "PropertiesColumnsHash";

		private DataRow propertyRow;

		private Property[] propInternal;

		private object parentObject;

		private int propertiesOffset;

		private Hashtable namesHash;

		public Property this[int index]
		{
			get
			{
				if (index < 0 || index >= this.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return this.GetProperty(index);
			}
		}

		public Property this[string name]
		{
			get
			{
				Property property = this.Find(name);
				if (null == property)
				{
					throw new ArgumentException(SR.Property_PropertyNotFound(name), "name");
				}
				return property;
			}
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
				return this.propInternal.SyncRoot;
			}
		}

		public int Count
		{
			get
			{
				return this.propInternal.Length;
			}
		}

		internal PropertyCollection(DataRow propertyRow, object parent) : this(propertyRow, parent, 1)
		{
		}

		internal PropertyCollection(DataRow propertyRow, object parent, int propertiesOffset)
		{
			this.propertyRow = propertyRow;
			this.propertiesOffset = propertiesOffset;
			int num;
			if (propertyRow == null || propertyRow.Table == null)
			{
				num = 0;
				this.namesHash = new Hashtable();
			}
			else
			{
				num = Math.Max(propertyRow.Table.Columns.Count - this.propertiesOffset, 0);
				if (propertyRow.Table.ExtendedProperties["PropertiesColumnsHash"] is Hashtable)
				{
					this.namesHash = (propertyRow.Table.ExtendedProperties["PropertiesColumnsHash"] as Hashtable);
				}
				else
				{
					this.namesHash = PropertyCollection.GetNamesHash(propertyRow.Table);
					propertyRow.Table.ExtendedProperties["PropertiesColumnsHash"] = this.namesHash;
				}
			}
			this.propInternal = new Property[num];
			this.parentObject = parent;
		}

		public Property Find(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.propertyRow == null || this.propertyRow.Table == null)
			{
				return null;
			}
			int propertyColumnIndex = this.GetPropertyColumnIndex(name);
			if (propertyColumnIndex == -1)
			{
				return null;
			}
			return this.GetProperty(propertyColumnIndex - this.propertiesOffset);
		}

		public void CopyTo(Property[] array, int index)
		{
			((ICollection)this).CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.propInternal.CopyTo(array, index);
		}

		public PropertyCollection.Enumerator GetEnumerator()
		{
			return new PropertyCollection.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private Property GetProperty(int index)
		{
			if (this.propInternal[index] == null)
			{
				this.propInternal[index] = new Property(this.propertyRow, index + this.propertiesOffset, this.parentObject);
			}
			return this.propInternal[index];
		}

		private static Hashtable GetNamesHash(DataTable table)
		{
			Hashtable hashtable = new Hashtable();
			if (table == null)
			{
				return hashtable;
			}
			AdomdUtils.FillNamesHashTable(table, hashtable);
			return hashtable;
		}

		private int GetPropertyColumnIndex(string propName)
		{
			object obj = this.namesHash[propName];
			if (obj is int)
			{
				int num = (int)obj;
				if (num < this.propertiesOffset)
				{
					num = -1;
				}
				return num;
			}
			return -1;
		}
	}
}
