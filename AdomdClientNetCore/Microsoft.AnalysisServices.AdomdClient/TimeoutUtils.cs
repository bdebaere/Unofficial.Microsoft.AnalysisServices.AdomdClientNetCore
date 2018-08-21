using System;
using System.Diagnostics;

namespace Microsoft.AnalysisServices.AdomdClient
{
	internal class TimeoutUtils
	{
		internal delegate bool OnTimoutAction();

		internal class TimeLeft
		{
			private int timeMs;

			internal bool Infinite
			{
				get;
				set;
			}

			internal int TimeMs
			{
				get
				{
					return this.timeMs;
				}
				set
				{
					if (!this.Infinite)
					{
						this.timeMs = value;
					}
				}
			}

			internal int TimeSec
			{
				get
				{
					return this.timeMs / 1000;
				}
			}

			private TimeLeft(int timeMs)
			{
				if (timeMs == 0)
				{
					this.Infinite = true;
				}
				this.TimeMs = timeMs;
			}

			internal static TimeoutUtils.TimeLeft FromMs(int timeMs)
			{
				return new TimeoutUtils.TimeLeft(timeMs);
			}

			internal static TimeoutUtils.TimeLeft FromSeconds(int timeSec)
			{
				return new TimeoutUtils.TimeLeft(timeSec * 1000);
			}
		}

		internal class TimeRestrictedMonitor : IDisposable
		{
			private TimeoutUtils.TimeLeft timeLeft;

			private Stopwatch watch;

			private TimeoutUtils.OnTimoutAction timoutAction;

			internal TimeRestrictedMonitor(TimeoutUtils.TimeLeft timeLeft, TimeoutUtils.OnTimoutAction timoutAction)
			{
				this.timeLeft = timeLeft;
				this.timoutAction = timoutAction;
				if (!timeLeft.Infinite && timeLeft.TimeMs < 0)
				{
					timoutAction();
				}
				this.watch = Stopwatch.StartNew();
			}

			public void Dispose()
			{
				this.watch.Stop();
				this.timeLeft.TimeMs -= (int)this.watch.ElapsedMilliseconds;
				if (!this.timeLeft.Infinite && this.timeLeft.TimeMs < 0)
				{
					this.timoutAction();
				}
			}
		}
	}
}
