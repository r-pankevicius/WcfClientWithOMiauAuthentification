namespace LazyCatWinForm
{
	partial class Form1
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
			this.buttonRunAllTests = new System.Windows.Forms.Button();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonRunAllTests
			// 
			this.buttonRunAllTests.Location = new System.Drawing.Point(26, 12);
			this.buttonRunAllTests.Name = "buttonRunAllTests";
			this.buttonRunAllTests.Size = new System.Drawing.Size(75, 23);
			this.buttonRunAllTests.TabIndex = 0;
			this.buttonRunAllTests.Text = "Run all tests";
			this.buttonRunAllTests.UseVisualStyleBackColor = true;
			this.buttonRunAllTests.Click += new System.EventHandler(this.buttonRunAllTests_Click);
			// 
			// textBoxLog
			// 
			this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxLog.Location = new System.Drawing.Point(26, 55);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBoxLog.Size = new System.Drawing.Size(762, 383);
			this.textBoxLog.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.buttonRunAllTests);
			this.Name = "Form1";
			this.Text = "Lazy Cat Windows Form Test App";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonRunAllTests;
		private System.Windows.Forms.TextBox textBoxLog;
	}
}

