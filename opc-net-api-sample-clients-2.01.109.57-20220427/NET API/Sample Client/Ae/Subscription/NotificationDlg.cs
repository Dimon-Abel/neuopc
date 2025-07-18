//============================================================================
// TITLE: NotificationDlg.cs
//
// CONTENTS:
// 
// A dialog that displays the state of a condition.
//
// (c) Copyright 2002-2004 The OPC Foundation
// ALL RIGHTS RESERVED.
//
// DISCLAIMER:
//  This code is provided by the OPC Foundation solely to assist in 
//  understanding and use of the appropriate OPC Specification(s) and may be 
//  used as set forth in the License Grant section of the OPC Specification.
//  This code is provided as-is and without warranty or support of any sort
//  and is subject to the Warranty and Liability Disclaimers which appear
//  in the printed OPC Specification.
//
// MODIFICATION LOG:
//
// Date       By    Notes
// ---------- ---   -----
// 2002/11/16 RSA   First release.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Opc.Da;
using Opc.Ae;
using Opc.SampleClient;

namespace Opc.Ae.SampleClient
{
	/// <summary>
	/// A dialog that displays the current status of an OPC server.
	/// </summary>
	public class NotificationDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel ButtonsPN;
		private System.Windows.Forms.Button CancelBTN;
		private System.Windows.Forms.Panel LeftPN;
		private Opc.Ae.SampleClient.NotificationCtrl NotificationCTRL;
		private System.Windows.Forms.Button AcknowledgeBTN;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NotificationDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ButtonsPN = new System.Windows.Forms.Panel();
			this.AcknowledgeBTN = new System.Windows.Forms.Button();
			this.CancelBTN = new System.Windows.Forms.Button();
			this.LeftPN = new System.Windows.Forms.Panel();
			this.NotificationCTRL = new Opc.Ae.SampleClient.NotificationCtrl();
			this.ButtonsPN.SuspendLayout();
			this.LeftPN.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonsPN
			// 
			this.ButtonsPN.Controls.Add(this.AcknowledgeBTN);
			this.ButtonsPN.Controls.Add(this.CancelBTN);
			this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPN.Location = new System.Drawing.Point(0, 474);
			this.ButtonsPN.Name = "ButtonsPN";
			this.ButtonsPN.Size = new System.Drawing.Size(552, 36);
			this.ButtonsPN.TabIndex = 0;
			// 
			// AcknowledgeBTN
			// 
			this.AcknowledgeBTN.Location = new System.Drawing.Point(4, 8);
			this.AcknowledgeBTN.Name = "AcknowledgeBTN";
			this.AcknowledgeBTN.Size = new System.Drawing.Size(92, 23);
			this.AcknowledgeBTN.TabIndex = 0;
			this.AcknowledgeBTN.Text = "Acknowledge";
			this.AcknowledgeBTN.Click += new System.EventHandler(this.AcknowledgeBTN_Click);
			// 
			// CancelBTN
			// 
			this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBTN.Location = new System.Drawing.Point(472, 8);
			this.CancelBTN.Name = "CancelBTN";
			this.CancelBTN.TabIndex = 1;
			this.CancelBTN.Text = "Close";
			this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
			// 
			// LeftPN
			// 
			this.LeftPN.Controls.Add(this.NotificationCTRL);
			this.LeftPN.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LeftPN.DockPadding.Left = 4;
			this.LeftPN.DockPadding.Right = 4;
			this.LeftPN.DockPadding.Top = 4;
			this.LeftPN.Location = new System.Drawing.Point(0, 0);
			this.LeftPN.Name = "LeftPN";
			this.LeftPN.Size = new System.Drawing.Size(552, 474);
			this.LeftPN.TabIndex = 1;
			// 
			// NotificationCTRL
			// 
			this.NotificationCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.NotificationCTRL.Location = new System.Drawing.Point(4, 4);
			this.NotificationCTRL.Name = "NotificationCTRL";
			this.NotificationCTRL.Size = new System.Drawing.Size(544, 470);
			this.NotificationCTRL.TabIndex = 0;
			// 
			// NotificationDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelBTN;
			this.ClientSize = new System.Drawing.Size(552, 510);
			this.Controls.Add(this.LeftPN);
			this.Controls.Add(this.ButtonsPN);
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size(0, 180);
			this.Name = "NotificationDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "View Condition State";
			this.ButtonsPN.ResumeLayout(false);
			this.LeftPN.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Private Members
		private Opc.Ae.Subscription m_subscription = null;
		private EventNotification m_notification = null;
		#endregion

		#region Public Interface
		/// <summary>
		/// Displays the event notifications.
		/// </summary>
		public void ShowDialog(Opc.Ae.Subscription subscription, EventNotification notification)
		{
			if (subscription == null) throw new ArgumentNullException("subscription");

			m_subscription = subscription;
			m_notification = notification;

			AcknowledgeBTN.Enabled = notification.AckRequired;

			NotificationCTRL.ShowNotification(subscription, notification);

			ShowDialog();
		}
		#endregion

		#region Private Methods
		#endregion

		#region Event Handlers
		/// <summary>
		/// Closes the window.
		/// </summary>
		private void CancelBTN_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		/// <summary>
		/// Acknowledges an event.
		/// </summary>
		private void AcknowledgeBTN_Click(object sender, System.EventArgs e)
		{
			try
			{
				bool result = new AcknowledgerEditDlg().ShowDialog(m_subscription.Server, new EventNotification[] { m_notification });

				if (result)
				{
					AcknowledgeBTN.Enabled = false;
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}		
		}
		#endregion


	}
}
