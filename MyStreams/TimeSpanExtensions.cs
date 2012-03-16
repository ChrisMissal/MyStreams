using System;

namespace MyStreams
{
	internal static class TimeSpanExtensions
	{
		public static TimeSpan MultiplyBy(this TimeSpan timeSpan, int factor)
		{
			return new TimeSpan(timeSpan.Ticks*factor);
		}
	}
}
