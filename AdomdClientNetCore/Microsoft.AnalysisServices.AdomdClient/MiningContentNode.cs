using System;
using System.Data;
using System.Globalization;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class MiningContentNode : IMetadataObject
	{
		private DataRow miningContentNodeRow;

		private MiningModel parentMiningModel;

		private AdomdConnection connection;

		private PropertyCollection propertiesCollection;

		private MiningContentNodeCollection ancestors;

		private MiningContentNodeCollection children;

		private MiningContentNodeCollection siblings;

		private MiningContentNodeCollection descendants;

		private MiningContentNode parent;

		private MiningDistributionCollection distributions;

		private MiningAttribute attribute;

		private string catalog;

		private string sessionId;

		private int hashCode;

		private bool hashCodeCalculated;

		internal static string miningContentNodeNameColumn = "NODE_NAME";

		internal static string miningContentNodeUniqueNameColumn = "NODE_UNIQUE_NAME";

		internal static string miningContentNodeTypeColumn = "NODE_TYPE";

		internal static string miningContentNodeProbabilityColumn = "NODE_PROBABILITY";

		internal static string miningContentNodeMargProbabilityColumn = "MARGINAL_PROBABILITY";

		internal static string miningContentNodeScoreColumn = "MSOLAP_NODE_SCORE";

		internal static string miningContentNodeSupportColumn = "NODE_SUPPORT";

		internal static string miningContentNodeDescriptionColumn = "NODE_DESCRIPTION";

		internal static string miningContentNodeRuleColumn = "NODE_RULE";

		internal static string miningContentNodeMargRuleColumn = "MARGINAL_RULE";

		internal static string miningContentNodeParentUniqueNameColumn = "PARENT_UNIQUE_NAME";

		internal static string miningContentNodeCaptionColumn = "NODE_CAPTION";

		internal static string miningContentNodeShortCaptionColumn = "MSOLAP_NODE_SHORT_CAPTION";

		internal static string miningContentNodeDistributionColumn = "NODE_DISTRIBUTION";

		internal static string miningContentNodeAttributeColumn = "ATTRIBUTE_NAME";

		public string Name
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeNameColumn).ToString();
			}
		}

		public string UniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeUniqueNameColumn).ToString();
			}
		}

		public MiningModel ParentMiningModel
		{
			get
			{
				return this.parentMiningModel;
			}
		}

		public MiningAttribute Attribute
		{
			get
			{
				if (this.attribute == null)
				{
					string attributeDisplayName = AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeAttributeColumn).ToString();
					this.attribute = new MiningAttribute(this.parentMiningModel, attributeDisplayName);
				}
				return this.attribute;
			}
		}

		public MiningNodeType Type
		{
			get
			{
				return (MiningNodeType)Convert.ToInt32(AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeTypeColumn), CultureInfo.InvariantCulture);
			}
		}

		public double Probability
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeProbabilityColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public double MarginalProbability
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeMargProbabilityColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public double Support
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeSupportColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public double Score
		{
			get
			{
				object property = AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeScoreColumn);
				return Convert.ToDouble(property, CultureInfo.InvariantCulture);
			}
		}

		public string Description
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeDescriptionColumn).ToString();
			}
		}

		public string NodeRule
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeRuleColumn).ToString();
			}
		}

		public string MarginalRule
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeMargRuleColumn).ToString();
			}
		}

		public MiningContentNode ParentNode
		{
			get
			{
				if (null == this.parent)
				{
					MiningContentNodeCollection miningContentNodeCollection = new MiningContentNodeCollection(this.connection, this, MiningNodeTreeOpType.TreeopParent);
					if (miningContentNodeCollection.Count > 0)
					{
						this.parent = miningContentNodeCollection[0];
					}
				}
				return this.parent;
			}
		}

		public string ParentUniqueName
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeParentUniqueNameColumn).ToString();
			}
		}

		public string Caption
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeCaptionColumn).ToString();
			}
		}

		public string ShortCaption
		{
			get
			{
				return AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeShortCaptionColumn).ToString();
			}
		}

		public MiningContentNodeCollection Ancestors
		{
			get
			{
				if (this.ancestors == null)
				{
					this.ancestors = new MiningContentNodeCollection(this.connection, this, MiningNodeTreeOpType.TreeopAncestors);
				}
				else
				{
					this.ancestors.CollectionInternal.CheckCache();
				}
				return this.ancestors;
			}
		}

		public MiningContentNodeCollection Children
		{
			get
			{
				if (this.children == null)
				{
					this.children = new MiningContentNodeCollection(this.connection, this, MiningNodeTreeOpType.TreeopChildren);
				}
				else
				{
					this.children.CollectionInternal.CheckCache();
				}
				return this.children;
			}
		}

		public MiningContentNodeCollection Siblings
		{
			get
			{
				if (this.siblings == null)
				{
					this.siblings = new MiningContentNodeCollection(this.connection, this, MiningNodeTreeOpType.TreeopSiblings);
				}
				else
				{
					this.siblings.CollectionInternal.CheckCache();
				}
				return this.siblings;
			}
		}

		public MiningContentNodeCollection Descendants
		{
			get
			{
				if (this.descendants == null)
				{
					this.descendants = new MiningContentNodeCollection(this.connection, this, MiningNodeTreeOpType.TreeopDescendants);
				}
				else
				{
					this.descendants.CollectionInternal.CheckCache();
				}
				return this.siblings;
			}
		}

		public MiningDistributionCollection Distribution
		{
			get
			{
				if (this.distributions == null)
				{
					DataRow[] rows = (DataRow[])AdomdUtils.GetProperty(this.miningContentNodeRow, MiningContentNode.miningContentNodeDistributionColumn);
					this.distributions = new MiningDistributionCollection(this.connection, this, rows);
				}
				return this.distributions;
			}
		}

		public PropertyCollection Properties
		{
			get
			{
				if (this.propertiesCollection == null)
				{
					this.propertiesCollection = new PropertyCollection(this.miningContentNodeRow, this);
				}
				return this.propertiesCollection;
			}
		}

		AdomdConnection IMetadataObject.Connection
		{
			get
			{
				return this.connection;
			}
		}

		string IMetadataObject.Catalog
		{
			get
			{
				return this.catalog;
			}
		}

		string IMetadataObject.SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		string IMetadataObject.CubeName
		{
			get
			{
				return this.ParentMiningModel.Name;
			}
		}

		string IMetadataObject.UniqueName
		{
			get
			{
				return this.Name;
			}
		}

		Type IMetadataObject.Type
		{
			get
			{
				return typeof(MiningContentNode);
			}
		}

		internal MiningContentNode(AdomdConnection connection, DataRow miningContentNodeRow, MiningModel parentMiningModel, string catalog, string sessionId)
		{
			this.connection = connection;
			this.miningContentNodeRow = miningContentNodeRow;
			this.parentMiningModel = parentMiningModel;
			this.propertiesCollection = null;
			this.catalog = catalog;
			this.sessionId = sessionId;
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal void SetParentNode(MiningContentNode node)
		{
			this.parent = node;
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
			return AdomdUtils.Equals(this, obj as IMetadataObject);
		}

		public static bool operator ==(MiningContentNode o1, MiningContentNode o2)
		{
			return object.Equals(o1, o2);
		}

		public static bool operator !=(MiningContentNode o1, MiningContentNode o2)
		{
			return !(o1 == o2);
		}
	}
}
