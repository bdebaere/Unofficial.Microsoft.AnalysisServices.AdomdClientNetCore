using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class AdomdRestriction : IXmlaProperty, IXmlaPropertyKey
	{
		private object parent;

		private string name;

		private object restrictionValue;

		private string restrictionNamespace;

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
					AdomdRestrictionCollectionInternal internalCollection = ((AdomdRestrictionCollection)this.Parent).InternalCollection;
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
				return this.restrictionValue;
			}
			set
			{
				this.restrictionValue = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.restrictionNamespace;
			}
			set
			{
				if (this.restrictionNamespace != value)
				{
					if (this.Parent == null)
					{
						this.restrictionNamespace = value;
						return;
					}
					AdomdRestrictionCollectionInternal internalCollection = ((AdomdRestrictionCollection)this.Parent).InternalCollection;
					internalCollection.ChangeNamespace(this, value);
				}
			}
		}

		string IXmlaPropertyKey.Namespace
		{
			get
			{
				return this.restrictionNamespace;
			}
			set
			{
				this.restrictionNamespace = value;
			}
		}

		public AdomdRestriction(string name, object restrictionValue) : this(name, null, restrictionValue)
		{
		}

		public AdomdRestriction(string name, string restrictionNamespace, object restrictionValue)
		{
			this.name = name;
			this.restrictionValue = restrictionValue;
			this.restrictionNamespace = restrictionNamespace;
		}
	}
}
