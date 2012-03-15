using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using Google.GData.Calendar;
using Google.GData.Extensions;

namespace MyStreams
{
	internal static class GoogleCalendarChannelListingsRetriever
	{
		private static readonly Regex ChannelRegex = new Regex(@"\d+", RegexOptions.Compiled);

		public static ChannelListing[] GetChannelListings(DateTime startTime, DateTime endTime)
		{
			var service = new CalendarService("MyStreams");

			var events = Enumerable.Empty<Event>();

			//src\={.#\@group\.calendar\.google\.com}\&color\=\%.^2{.^2}{.^2}{.^2}\&
			//\n\t\t\t\tnew Calendar {UserId = "\1", Color = Color.FromArgb(0x\2, 0x\3, 0x\4)},
			var calendars = new[]
			{
				new Calendar {UserId = "843f2k6ufc6h938tqpjlrl1f2g@group.calendar.google.com", Color = Color.FromArgb(0xAB, 0x8B, 0x00)},//American Football
				new Calendar {UserId = "6nt5993tqvsaptu2gsij8pp1ic@group.calendar.google.com", Color = Color.FromArgb(0x1B, 0x88, 0x7A)},//Baseball
				new Calendar {UserId = "k1navve7eipa71vs0bj8h714kc@group.calendar.google.com", Color = Color.FromArgb(0x29, 0x52, 0xA3)},//Basketball
				new Calendar {UserId = "qesea2ng9dp56gf7guh2ngd400@group.calendar.google.com", Color = Color.FromArgb(0xAB, 0x8B, 0x00)},//Boxing
				new Calendar {UserId = "8pspdk0mihhbl6lnig40an4atk@group.calendar.google.com", Color = Color.FromArgb(0x0D, 0x78, 0x13)},//Cricket
				new Calendar {UserId = "8ilqj10b4k8si068vko201stj8@group.calendar.google.com", Color = Color.FromArgb(0x52, 0x29, 0xA3)},//Golf
				new Calendar {UserId = "g7uh0g6jtevu6857anlnp5f7as@group.calendar.google.com", Color = Color.FromArgb(0xAB, 0x8B, 0x00)},//Ice Hockey
				new Calendar {UserId = "fmthdnd74gkghjb7f83vg7737c@group.calendar.google.com", Color = Color.FromArgb(0x52, 0x29, 0xA3)},//Motor Sports
				new Calendar {UserId = "32fv7n986l6p4b0r45e77s3bik@group.calendar.google.com", Color = Color.FromArgb(0x8D, 0x6F, 0x47)},//Olympics
				new Calendar {UserId = "ddm4hlm4fcrc2andeejhj5mp6c@group.calendar.google.com", Color = Color.FromArgb(0x4A, 0x71, 0x6C)},//Other Sports
				new Calendar {UserId = "bi0pf2acjvg8g4ohak7bpsnn90@group.calendar.google.com", Color = Color.FromArgb(0x28, 0x75, 0x4E)},//Rugby
				new Calendar {UserId = "c4qhj6878jilb8l49bmbqdg1vo@group.calendar.google.com", Color = Color.FromArgb(0xA3, 0x29, 0x29)},//Soccer
				new Calendar {UserId = "tqchra1eni6s0a5c4h3ksjth2o@group.calendar.google.com", Color = Color.FromArgb(0xA3, 0x29, 0x29)},//Tennis
				new Calendar {UserId = "cte17kh4rgknfokp60lsesg4dg@group.calendar.google.com", Color = Color.FromArgb(0x0D, 0x78, 0x13)},//TV Shows
				new Calendar {UserId = "6h5rqm6oub9gu6nm1aekd8fhu8@group.calendar.google.com", Color = Color.FromArgb(0x29, 0x52, 0xA3)},//Wrestling/MMA
			};

			foreach (var calendar in calendars)
			{
				var queryUri = string.Format("http://www.google.com/calendar/feeds/{0}/public/full", calendar.UserId);

				var eventQuery = new EventQuery(queryUri) {StartTime = startTime, EndTime = endTime};

				var eventFeed = service.Query(eventQuery);

				events = events.Concat(GetEvents(eventFeed.Entries.Cast<EventEntry>(), calendar));
			}

			return events
				.GroupBy(x => x.Channel)
				.Select(x => new ChannelListing {Channel = x.Key, Events = x.OrderBy(y => y.StartTime).ToArray()})
				.OrderBy(x => x.Channel)
				.ToArray();
		}

		private static IEnumerable<Event> GetEvents(IEnumerable<EventEntry> eventEntries, Calendar calendar)
		{
			foreach (var eventEntry in eventEntries)
			{
				var channels = GetChannels(eventEntry.Locations);
				var time = eventEntry.Times.FirstOrDefault();

				if (time != null)
				{
					foreach (var channel in channels)
					{
						yield return new Event
						{
						    Title = eventEntry.Title.Text,
						    Channel = channel,
						    StartTime = time.StartTime,
						    EndTime = time.EndTime,
							Color = calendar.Color
						};
					}
				}
			}
		}

		private static IEnumerable<int> GetChannels(IEnumerable<Where> locations)
		{
			var channels = Enumerable.Empty<int>();

			foreach (var location in locations)
			{
				var matches = ChannelRegex.Matches(location.ValueString);

				channels = channels.Concat(matches.Cast<Match>().Select(x => Int32.Parse(x.Value)));
			}

			return channels.OrderBy(x => x);
		}

		private class Calendar
		{
			public string UserId { get; set; }
			public Color Color { get; set; }
		}
	}

	internal class ChannelListing
	{
		public int Channel { get; set; }
		public Event[] Events { get; set; } 
	}

	internal class Event
	{
		public string Title { get; set; }
		public int Channel { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public Color Color { get; set; }

		public override string ToString()
		{
			return string.Format("{0} ({1}-{2})", Title, ToString(StartTime), ToString(EndTime));
		}

		private static string ToString(DateTime dateTime)
		{
			var formatString = "";

			if (dateTime.Date != DateTime.Now.Date)
				formatString += "M/d/y ";

			if (dateTime.Minute == 0)
				formatString += "ht";
			else
				formatString += "h:mmt";

			return dateTime.ToString(formatString);
		}
	}
}