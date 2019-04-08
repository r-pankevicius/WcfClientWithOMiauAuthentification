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

			// --------------------------------------------------------------------------
			textBoxLog.Text += "\r\n@@@@@@ Checking an ordinary WCF client @@@@@.\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_StandardClient_Anonymous_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\\n{nameof(ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync)}\r\n";
			bool exceptionThrown = false;
			try
			{
				ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Sync();
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

			textBoxLog.Text += $"\r\\n{nameof(ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async)}\r\n";
			exceptionThrown = false;
			try
			{
				await ClientUsageScenarios.SumWithOMiauAuth_StandardClient_Anonymous_Fails_Async();
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

			// --------------------------------------------------------------------------
			textBoxLog.Text += "\r\n@@@@@@ Diving into level 1 @@@@@.\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_Anonymous_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_ManualClient_OMiau_HandlesExpiredToken_Async();
			textBoxLog.Text += "OK\r\n";

			// --------------------------------------------------------------------------
			textBoxLog.Text += "\r\n@@@@@@ Diving into level 2 @@@@@.\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_Slim_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_Slim_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_Slim_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_Slim_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync)}\r\n";
			ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Sync();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += $"\r\n{nameof(ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async)}\r\n";
			await ClientUsageScenarios.SumTwoNumbers_SlimOMiau_HandlesExpiredToken_Async();
			textBoxLog.Text += "OK\r\n";

			textBoxLog.Text += "\r\nSeems all fine, we haven't blocked UI so far...\r\n";
		}
	}
}
