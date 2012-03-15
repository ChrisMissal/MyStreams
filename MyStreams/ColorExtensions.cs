using System;
using System.Drawing;

namespace MyStreams
{
	internal static class ColorExtensions
	{
		public static Color ChangeSaturation(this Color color, double factor)
		{
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			var hue = color.GetHue();
			var saturation = (max == 0) ? 0 : (1 - (1d*min/max))*factor;
			var value = max/255d;

			if (saturation > 1)
				saturation = 1;

			var hi = Convert.ToInt32(Math.Floor(hue/60))%6;
			var f = hue/60 - Math.Floor(hue/60);

			value = value * 255;
			var v = Convert.ToInt32(value);
			var p = Convert.ToInt32(value*(1 - saturation));
			var q = Convert.ToInt32(value*(1 - f*saturation));
			var t = Convert.ToInt32(value*(1 - (1 - f)*saturation));

			switch (hi)
			{
				case 0:
					return Color.FromArgb(255, v, t, p);
				case 1:
					return Color.FromArgb(255, q, v, p);
				case 2:
					return Color.FromArgb(255, p, v, t);
				case 3:
					return Color.FromArgb(255, p, q, v);
				case 4:
					return Color.FromArgb(255, t, p, v);
				default:
					return Color.FromArgb(255, v, p, q);
			}
		}
	}
}