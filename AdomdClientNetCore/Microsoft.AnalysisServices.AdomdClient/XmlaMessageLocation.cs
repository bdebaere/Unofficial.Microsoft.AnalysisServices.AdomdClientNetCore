using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal sealed class XmlaMessageLocation
	{
		private int startLine = -1;

		private int startColumn = -1;

		private int endLine = -1;

		private int endColumn = -1;

		private int lineOffset = -1;

		private int textLength = -1;

		private XmlaLocationReference sourceObject;

		private XmlaLocationReference dependsOnObject;

		private long rowNumber = -1L;

		public int StartLine
		{
			get
			{
				return this.startLine;
			}
		}

		public int StartColumn
		{
			get
			{
				return this.startColumn;
			}
		}

		public int EndLine
		{
			get
			{
				return this.endLine;
			}
		}

		public int EndColumn
		{
			get
			{
				return this.endColumn;
			}
		}

		public int LineOffset
		{
			get
			{
				return this.lineOffset;
			}
		}

		public int TextLength
		{
			get
			{
				return this.textLength;
			}
		}

		public XmlaLocationReference SourceObject
		{
			get
			{
				return this.sourceObject;
			}
		}

		public XmlaLocationReference DependsOnObject
		{
			get
			{
				return this.dependsOnObject;
			}
		}

		public long RowNumber
		{
			get
			{
				return this.rowNumber;
			}
		}

		internal XmlaMessageLocation(int startLine, int startColumn, int endLine, int endColumn, int lineOffset, int textLength, XmlaLocationReference sourceObject, XmlaLocationReference dependsOnObject, long rowNumber)
		{
			this.startLine = startLine;
			this.startColumn = startColumn;
			this.endLine = endLine;
			this.endColumn = endColumn;
			this.lineOffset = lineOffset;
			this.textLength = textLength;
			this.sourceObject = sourceObject;
			this.dependsOnObject = dependsOnObject;
			this.rowNumber = rowNumber;
		}
	}
}
