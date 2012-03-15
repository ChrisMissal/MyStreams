namespace MyStreams
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listingsGrid = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.listingsGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// listingsGrid
			// 
			this.listingsGrid.AllowUserToAddRows = false;
			this.listingsGrid.AllowUserToDeleteRows = false;
			this.listingsGrid.AllowUserToResizeColumns = false;
			this.listingsGrid.AllowUserToResizeRows = false;
			this.listingsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.listingsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.listingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listingsGrid.Location = new System.Drawing.Point(0, 0);
			this.listingsGrid.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
			this.listingsGrid.MultiSelect = false;
			this.listingsGrid.Name = "listingsGrid";
			this.listingsGrid.ReadOnly = true;
			this.listingsGrid.RowHeadersVisible = false;
			this.listingsGrid.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.listingsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.listingsGrid.Size = new System.Drawing.Size(800, 600);
			this.listingsGrid.TabIndex = 0;
			this.listingsGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.listingsGrid_CellMouseDoubleClick);
			this.listingsGrid.Scroll += new System.Windows.Forms.ScrollEventHandler(this.listingsGrid_Scroll);
			this.listingsGrid.Paint += new System.Windows.Forms.PaintEventHandler(this.listingsGrid_Paint);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 37F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this.listingsGrid);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
			this.Name = "Main";
			this.Text = "MyStreams";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.Load += new System.EventHandler(this.Main_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Main_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.listingsGrid)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView listingsGrid;

	}
}
