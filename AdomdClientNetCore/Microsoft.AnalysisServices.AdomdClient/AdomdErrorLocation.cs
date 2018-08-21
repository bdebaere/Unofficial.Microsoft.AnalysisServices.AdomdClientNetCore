using System;

namespace Microsoft.AnalysisServices.AdomdClient
{
	[Serializable]
	public class AdomdErrorLocation
	{
		private int startLine = -1;

		private int startColumn = -1;

		private int endLine = -1;

		private int endColumn = -1;

		private int lineOffset = -1;

		private int textLength = -1;

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

		internal AdomdErrorLocation(int startLine, int startColumn, int endLine, int endColumn, int lineOffset, int textLength)
		{
			this.startLine = startLine;
			this.startColumn = startColumn;
			this.endLine = endLine;
			this.endColumn = endColumn;
			this.lineOffset = lineOffset;
			this.textLength = textLength;
		}

		internal AdomdErrorLocation(XmlaMessageLocation location) : this(location.StartLine, location.StartColumn, location.EndLine, location.EndColumn, location.LineOffset, location.TextLength)
		{
		}
	}
}
