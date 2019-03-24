using LazyCatConsole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LazyCatWinForm
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private async void buttonRunAllTests_Click(object sender, EventArgs e)
		{
			textBoxLog.Text = "";

			textBoxLog.Text += $"Using endpoint {ClientUsageScenarios.EndpointUrl}\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Anonymous_Sync\r\n";
			ClientUsageScenarios.SumTwoNumbers_Anonymous_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Anonymous_Async\r\n";
			await ClientUsageScenarios.SumTwoNumbers_Anonymous_Async();
			textBoxLog.Text += "OK\r\n";


			textBoxLog.Text += "\r\nSeems all fine, we haven't blocked UI...\r\n";
		}
	}
}
