using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdProperty : IXmlaProperty, IXmlaPropertyKey
	{
		private object parent;

		private string name;

		private object propertyValue;

		private string propertyNamespace;

		public object Parent
		{
			get
			{
				return this.parent;
			}
		}

		object IXmlaProperty.Parent
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

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					if (this.Parent == null)
					{
						this.name = value;
						return;
					}
					AdomdPropertyCollectionInternal internalCollection = ((AdomdPropertyCollection)this.Parent).InternalCollection;
					internalCollection.ChangeName(this, value);
				}
			}
		}

		string IXmlaPropertyKey.Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public object Value
		{
			get
			{
				return this.propertyValue;
			}
			set
			{
				this.propertyValue = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.propertyNamespace;
			}
			set
			{
				if (this.propertyNamespace != value)
				{
					if (this.Parent == null)
					{
						this.propertyNamespace = value;
						return;
					}
					AdomdPropertyCollectionInternal internalCollection = ((AdomdPropertyCollection)this.Parent).InternalCollection;
					internalCollection.ChangeNamespace(this, value);
				}
			}
		}

		string IXmlaPropertyKey.Namespace
		{
			get
			{
				return this.propertyNamespace;
			}
			set
			{
				this.propertyNamespace = value;
			}
		}

		public AdomdProperty(string name, object propertyValue) : this(name, null, propertyValue)
		{
		}

		public AdomdProperty(string name, string propertyNamespace, object propertyValue)
		{
			this.name = name;
			this.propertyValue = propertyValue;
			this.propertyNamespace = propertyNamespace;
		}
	}
}
