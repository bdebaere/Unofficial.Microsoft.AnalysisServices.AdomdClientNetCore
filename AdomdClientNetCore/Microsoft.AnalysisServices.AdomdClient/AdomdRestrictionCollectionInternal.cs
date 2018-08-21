using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AdomdRestrictionCollectionInternal : XmlaPropertyCollectionBase
	{
		private AdomdRestrictionCollection parentCollection;

		protected override Type ItemType
		{
			get
			{
				return typeof(AdomdRestriction);
			}
		}

		protected override object Parent
		{
			get
			{
				return this.parentCollection;
			}
		}

		internal AdomdRestrictionCollectionInternal(AdomdRestrictionCollection parentCollection)
		{
			this.parentCollection = parentCollection;
		}

		protected override IXmlaProperty CreateBasePropertyObject(IXmlaPropertyKey key, object propertyValue)
		{
			return new AdomdRestriction(key.Name, key.Namespace, propertyValue);
		}
	}
}
