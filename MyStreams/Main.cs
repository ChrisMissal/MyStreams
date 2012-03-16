using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SHDocVw;
using WatiN.Core;
using Form = System.Windows.Forms.Form;

namespace MyStreams
{
	internal partial class Main : Form
	{
		private const int NumberOfColumns = 12;
		private static readonly TimeSpan ColumnTimeValue = new TimeSpan(0, 30, 0);

		private IE _browser;
		private IntPtr _browserHWnd;
		private ChannelListing[] _listings;
		private DateTime _startTime;
		private DateTime _endTime;

		public Main()
		{
			InitializeComponent();

			MoveToScreen(Handle, Screen.PrimaryScreen);
			WindowState = FormWindowState.Maximized;
		}

		private void Main_Load(object sender, EventArgs e)
		{
			RetrieveAndDisplayListings();
		}

		private void Main_KeyDown(object sender, KeyEventArgs e)
		{
			var selectedRow = GetSelectedRow();

			if (e.KeyCode == Keys.Enter)
			{
				OpenSelectedChannel();

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.PageUp && selectedRow != null)
			{
				if (selectedRow.Index > 0)
					listingsGrid.Rows[selectedRow.Index - 1].Selected = true;

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.PageDown && selectedRow != null)
			{
				if (selectedRow.Index < listingsGrid.Rows.Count - 1)
					listingsGrid.Rows[selectedRow.Index + 1].Selected = true;

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Escape)
			{
				Close();

				e.Handled = true;
			}
			else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.R)
			{
				RetrieveAndDisplayListings();

				e.Handled = true;
			}
			else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Right && Screen.AllScreens.Length > 1)
			{
				var currentScreenIndex = Screen.AllScreens.IndexOf(Screen.FromControl(this));
				var newIndex = currentScreenIndex + 1;

				if (newIndex >= Screen.AllScreens.Length)
					newIndex = 0;

				WindowState = FormWindowState.Normal;
				MoveToScreen(Handle, Screen.AllScreens[newIndex]);
				WindowState = FormWindowState.Maximized;

				e.Handled = true;
			}
			else if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Left && Screen.AllScreens.Length > 1)
			{
				var currentScreenIndex = Screen.AllScreens.IndexOf(Screen.FromControl(this));
				var newIndex = currentScreenIndex - 1;

				if (newIndex < 0)
					newIndex = Screen.AllScreens.Length - 1;

				WindowState = FormWindowState.Normal;
				MoveToScreen(Handle, Screen.AllScreens[newIndex]);
				WindowState = FormWindowState.Maximized;

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Right)
			{
				_startTime = _startTime.Add(ColumnTimeValue);
				_endTime = _endTime.Add(ColumnTimeValue);

				DisplayListings();
			}
			else if (e.KeyCode == Keys.Left)
			{
				_startTime = _startTime.Add(-ColumnTimeValue);
				_endTime = _endTime.Add(-ColumnTimeValue);

				DisplayListings();
			}
		}

		private void Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_browser != null)
			{
				_browser.Dispose();
				_browser = null;
			}
		}

		private void listingsGrid_Paint(object sender, PaintEventArgs e)
		{
			foreach (DataGridViewRow row in listingsGrid.Rows)
			{
				var cellNumber = 1;

				while (cellNumber < row.Cells.Count)
				{
					var firstRectangle = listingsGrid.GetCellDisplayRectangle(cellNumber, row.Index, true);

					var numberOfAdjcentCellsWithSameValue = GetNumberOfAdjcentCellsWithSameValue(row, cellNumber);

					var lastRectangle = listingsGrid.GetCellDisplayRectangle(cellNumber + numberOfAdjcentCellsWithSameValue, row.Index, true);
					var mergedRectangle = new Rectangle(firstRectangle.X + 1, firstRectangle.Y, lastRectangle.Right - firstRectangle.Left - 2, firstRectangle.Height - 1);
					var cell = row.Cells[cellNumber];
					var stringFormat = new StringFormat(StringFormatFlags.NoWrap) {Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter};

					if (row.Selected)
					{
						using (var brush = new SolidBrush(cell.Style.SelectionBackColor))
							e.Graphics.FillRectangle(brush, mergedRectangle);

						if (cell.Tag != null)
							using (var brush = new SolidBrush(GetSelectionForeColor(cell)))
								e.Graphics.DrawString(cell.Tag.ToString(), GetFont(cell, cellNumber == 1 ? FontStyle.Bold : FontStyle.Regular), brush, mergedRectangle, stringFormat);
					}
					else
					{
						using (var brush = new SolidBrush(cell.Style.BackColor))
							e.Graphics.FillRectangle(brush, mergedRectangle);

						if (cell.Tag != null)
							using (var brush = new SolidBrush(GetForeColor(cell)))
								e.Graphics.DrawString(cell.Tag.ToString(), GetFont(cell), brush, mergedRectangle, stringFormat);
					}

					cellNumber += numberOfAdjcentCellsWithSameValue + 1;
				}
			}
		}

		private void listingsGrid_Scroll(object sender, ScrollEventArgs e)
		{
			listingsGrid.Invalidate();
		}

		private void listingsGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex != -1)
				OpenSelectedChannel();
		}

		private void OpenSelectedChannel()
		{
			var selectedRow = GetSelectedRow();

			if (selectedRow != null)
			{
				var screen = Screen.FromControl(this);

				if (_browser != null && !Win32.IsWindow(_browserHWnd))
				{
					_browser.Dispose();
					_browser = null;
				}

				if (_browser == null)
				{
					_browser = new IE(true);
					_browserHWnd = _browser.hWnd;

					MoveToScreen(_browserHWnd, screen);
					((IWebBrowser2) _browser.InternetExplorer).FullScreen = true;
				}

				Win32.SetForegroundWindow(_browserHWnd);

				var tld = "tv";

				var url = "http://mystreams." + tld + "/go/stream.php?hd=1&server=2&stream=" + selectedRow.Cells[0].Value;
				_browser.GoTo(url);

				_browser.DomContainer.Eval("$f(0).play(); document.body.scroll = 'no';");

				var screenRectangle = screen.Bounds;
				Cursor.Position = new Point(screenRectangle.Right, screenRectangle.Height/2);
			}
		}

		private static void MoveToScreen(IntPtr hWnd, Screen screen)
		{
			var screenRectangle = screen.Bounds;

			Win32.SetWindowPos(hWnd, Win32.HWND_TOP, screenRectangle.Left, screenRectangle.Top, screenRectangle.Width, screenRectangle.Height, Win32.SWP_SHOWWINDOW);
		}

		private DataGridViewRow GetSelectedRow()
		{
			return listingsGrid.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault();
		}

		private static Color GetSelectionForeColor(DataGridViewCell cell)
		{
			if (cell.Style.ForeColor != Color.Empty)
				return cell.Style.SelectionForeColor;
			else
				return cell.OwningRow.DataGridView.DefaultCellStyle.SelectionForeColor;
		}

		private static Color GetForeColor(DataGridViewCell cell)
		{
			if (cell.Style.ForeColor != Color.Empty)
				return cell.Style.ForeColor;
			else
				return cell.OwningRow.DataGridView.DefaultCellStyle.ForeColor;
		}

		private static Font GetFont(DataGridViewCell cell, FontStyle fontStyle = FontStyle.Regular)
		{
			Font font;

			if (cell.Style.Font != null)
				font = cell.Style.Font;
			else
				font = cell.OwningRow.DataGridView.Font;

			return new Font(font, fontStyle);
		}

		private static int GetNumberOfAdjcentCellsWithSameValue(DataGridViewRow row, int cell)
		{
			var value = row.Cells[cell].Tag;

			if (value == null)
				return 0;

			var num = 0;
			while (++cell < row.Cells.Count && row.Cells[cell].Tag == value)
				++num;

			return num;
		}

		private void RetrieveAndDisplayListings()
		{
			listingsGrid.Rows.Clear();
			listingsGrid.Columns.Clear();

			_startTime = GetHalfHour(DateTime.Now);
			_endTime = _startTime + ColumnTimeValue.MultiplyBy(NumberOfColumns);

			var cursor = Cursor;
			Cursor = Cursors.WaitCursor;

			PerformInSeparateThread(() =>
			{
				_listings = GoogleCalendarChannelListingsRetriever.GetChannelListings(_startTime - ColumnTimeValue.MultiplyBy(5), _endTime + ColumnTimeValue.MultiplyBy(10));

				Invoke(() =>
				{
					Cursor = cursor;

					DisplayListings();
				});
			});
		}

		private void DisplayListings()
		{
			var listings = _listings
				.Select(x => new ChannelListing {Channel = x.Channel, Events = x.Events.Where(y => IsInTimeSlot(y, _startTime, _endTime)).ToArray()})
				.ToArray();

			listingsGrid.Rows.Clear();
			listingsGrid.Columns.Clear();

			listingsGrid.Columns.Add(new DataGridViewTextBoxColumn {AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader, SortMode = DataGridViewColumnSortMode.NotSortable});

			var currentTime = _startTime;
			for (var i = 0; i < NumberOfColumns; ++i)
			{
				listingsGrid.Columns.Add(new DataGridViewTextBoxColumn {HeaderText = currentTime.ToString("h:mm"), SortMode = DataGridViewColumnSortMode.NotSortable});
				currentTime = currentTime.Add(ColumnTimeValue);
			}

			var rowHeight = (int) Math.Round(CreateGraphics().MeasureString("Ay", Font).Height*1.1);

			foreach (var channel in listings)
			{
			    var row = new DataGridViewRow {Height = rowHeight};
			    row.Cells.Add(new DataGridViewTextBoxCell
			    {
					Value = channel.Channel,
					Style = new DataGridViewCellStyle {SelectionBackColor = Color.FromArgb(183, 210, 255), Alignment = DataGridViewContentAlignment.MiddleCenter}
			    });

			    currentTime = _startTime;
			    for (var i = 0; i < NumberOfColumns; ++i)
			    {
					var nextTime = currentTime.Add(ColumnTimeValue);
					var events = channel.Events
						.Where(x => IsInTimeSlot(x, currentTime, nextTime)).OrderBy(x => x.EndTime - x.StartTime)
						.ToArray();

					var cell = new DataGridViewTextBoxCell();
					if (events.Length > 0)
					{
						cell.Tag = events[0].Title;
						cell.ToolTipText = string.Join("\n", events.Select(x => x.ToString()));
						cell.Style = new DataGridViewCellStyle {BackColor = events[0].Color.ChangeSaturation(0.25), SelectionBackColor = events[0].Color};
					}
					else
					{
						cell.Style = new DataGridViewCellStyle {SelectionBackColor = Color.FromArgb(183, 210, 255)};
					}

					row.Cells.Add(cell);
					currentTime = nextTime;
				}

				listingsGrid.Rows.Add(row);
			}
		}

		private static bool IsInTimeSlot(Event @event, DateTime startTime, DateTime endTime)
		{
			return @event.StartTime < endTime && @event.EndTime > startTime;
		}

		private static DateTime GetHalfHour(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute < 30 ? 0 : 30, 0);
		}

		private static void PerformInSeparateThread(Action action)
		{
			var thread = new Thread(() => action());
			thread.Start();
		}

		private void Invoke(Action action)
		{
			if (InvokeRequired)
				Invoke((Delegate) action);
			else
				action();
		}
	}
}
