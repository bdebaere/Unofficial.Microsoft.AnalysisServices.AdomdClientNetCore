using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class AdomdPropertyCollectionInternal : XmlaPropertyCollectionBase
	{
		private AdomdPropertyCollection parentCollection;

		protected override Type ItemType
		{
			get
			{
				return typeof(AdomdProperty);
			}
		}

		protected override object Parent
		{
			get
			{
				return this.parentCollection;
			}
		}

		internal AdomdPropertyCollectionInternal(AdomdPropertyCollection parentCollection)
		{
			this.parentCollection = parentCollection;
		}

		protected override IXmlaProperty CreateBasePropertyObject(IXmlaPropertyKey key, object propertyValue)
		{
			return new AdomdProperty(key.Name, key.Namespace, propertyValue);
		}
	}
}
