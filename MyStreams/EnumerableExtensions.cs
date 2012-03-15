using System.Collections.Generic;

namespace MyStreams
{
	internal static class EnumerableExtensions
	{
		public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
		{
			var i = 0;

			foreach (var obj in enumerable)
			{
				if (obj.Equals(item))
					return i;

				++i;
			}

			return -1;
		}
	}
}