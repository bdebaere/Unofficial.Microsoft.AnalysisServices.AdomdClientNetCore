using System;
using System.Collections;
using System.Xml;

namespace Microsoft.AnalysisServices.AdomdClient
{
	public sealed class CellSet
	{
		internal const int FilterAxisOrdinal = -1;

		private MDDatasetFormatter datasetFormatter;

		private AxisCollection axes;

		private CellCollection cells;

		private AdomdConnection connection;

		private Axis filterAxis;

		private string cubeName;

		private OlapInfo olapInfo;

		internal MDDatasetFormatter Formatter
		{
			get
			{
				return this.datasetFormatter;
			}
		}

		public AxisCollection Axes
		{
			get
			{
				if (this.axes == null)
				{
					this.axes = new AxisCollection(this.connection, this, this.cubeName);
				}
				return this.axes;
			}
		}

		public Axis FilterAxis
		{
			get
			{
				if (null == this.filterAxis && this.datasetFormatter.FilterAxis != null)
				{
					this.filterAxis = new Axis(this.connection, this.datasetFormatter.FilterAxis, this.cubeName, this, -1);
				}
				return this.filterAxis;
			}
		}

		public CellCollection Cells
		{
			get
			{
				if (this.cells == null)
				{
					this.cells = new CellCollection(this);
				}
				return this.cells;
			}
		}

		public Cell this[int index]
		{
			get
			{
				return this.Cells[index];
			}
		}

		public Cell this[int index1, int index2]
		{
			get
			{
				return this.Cells[index1, index2];
			}
		}

		public Cell this[params int[] indexes]
		{
			get
			{
				return this.Cells[indexes];
			}
		}

		public Cell this[ICollection indexes]
		{
			get
			{
				return this.Cells[indexes];
			}
		}

		public OlapInfo OlapInfo
		{
			get
			{
				if (this.olapInfo == null)
				{
					this.olapInfo = new OlapInfo(this.Formatter);
				}
				return this.olapInfo;
			}
		}

		internal CellSet(MDDatasetFormatter formatter) : this(null, formatter)
		{
		}

		internal CellSet(AdomdConnection connection, MDDatasetFormatter formatter)
		{
			this.datasetFormatter = formatter;
			this.connection = connection;
			this.cubeName = formatter.CubeName;
		}

		public static CellSet LoadXml(XmlReader xmlTextReader)
		{
			if (xmlTextReader == null)
			{
				throw new ArgumentNullException("xmlTextReader");
			}
			if (xmlTextReader.ReadState != ReadState.Initial && xmlTextReader.ReadState != ReadState.Interactive)
			{
				throw new ArgumentException(SR.CellSet_InvalidStateOfReader(xmlTextReader.ReadState.ToString()), "xmlTextReader");
			}
			CellSet result;
			try
			{
				XmlReader reader;
				if (xmlTextReader is XmlaReader)
				{
					((XmlaReader)xmlTextReader).SkipElements = true;
					reader = xmlTextReader;
				}
				else
				{
					reader = new SkippingWrapperReader(xmlTextReader);
				}
				MDDatasetFormatter formatter = SoapFormatter.ReadDataSetResponse(reader);
				CellSet cellSet = new CellSet(formatter);
				result = cellSet;
			}
			catch (XmlaException innerException)
			{
				throw new AdomdErrorResponseException(innerException);
			}
			catch (XmlException innerException2)
			{
				throw new AdomdUnknownResponseException(XmlaSR.UnknownServerResponseFormat, innerException2);
			}
			return result;
		}
	}
}
