using LazyCatConsole;
using System;
using System.ServiceModel;
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

			textBoxLog.Text += "\r\n@@@@@@ Checking an ordinary WCF client @@@@@.\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Anonymous_Sync\r\n";
			ClientUsageScenarios.SumTwoNumbers_Anonymous_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Anonymous_Async\r\n";
			await ClientUsageScenarios.SumTwoNumbers_Anonymous_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumWithOMiauAuth_Anonymous_Fails_Sync\r\n";
			bool exceptionThrown = false;
			try
			{
				ClientUsageScenarios.SumWithOMiauAuth_Anonymous_Fails_Sync();
			}
			catch (FaultException<ExceptionDetail> ex)
			{
				exceptionThrown = true;
				textBoxLog.Text += $"Got FaultException as expected: {ex.Message}\r\n";
			}

			if (!exceptionThrown)
			{
				textBoxLog.Text += "FAIL - OMiau protected method can be accessed with Anonymous auth.\r\n";
				return;
			}

			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\n@@@@@@ Diving into level 1 @@@@@.\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_OMiau_Sync\r\n";
			ClientUsageScenarios.SumTwoNumbers_OMiau_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_OMiau_Async\r\n";
			await ClientUsageScenarios.SumTwoNumbers_OMiau_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\n@@@@@@ Diving into level 2 @@@@@.\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Slim_Sync\r\n";
			ClientUsageScenarios.SumTwoNumbers_OMiau_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_Slim_Async\r\n";
			await ClientUsageScenarios.SumTwoNumbers_Slim_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_SlimOMiau_Sync\r\n";
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSumTwoNumbers_SlimOMiau_Async\r\n";
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSeems all fine, we haven't blocked UI so far...\r\n";
		}
	}
}
