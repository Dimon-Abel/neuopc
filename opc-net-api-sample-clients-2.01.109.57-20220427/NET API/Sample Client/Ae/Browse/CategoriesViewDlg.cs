//============================================================================
// TITLE: CategoriesViewDlg.cs
//
// CONTENTS:
// 
// A dialog that displays the current status of an OPC server.
//
// (c) Copyright 2002-2003 The OPC Foundation
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

namespace Opc.Ae.SampleClient
{
	/// <summary>
	/// A dialog that displays the current status of an OPC server.
	/// </summary>
	public class CategoriesViewDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel ButtonsPN;
		private System.Windows.Forms.Button CancelBTN;
		private System.Windows.Forms.ListView CategoriesLV;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CategoriesViewDlg()
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
			this.CancelBTN = new System.Windows.Forms.Button();
			this.CategoriesLV = new System.Windows.Forms.ListView();
			this.ButtonsPN.SuspendLayout();
			this.SuspendLayout();
			// 
			// ButtonsPN
			// 
			this.ButtonsPN.Controls.Add(this.CancelBTN);
			this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPN.Location = new System.Drawing.Point(0, 202);
			this.ButtonsPN.Name = "ButtonsPN";
			this.ButtonsPN.Size = new System.Drawing.Size(292, 36);
			this.ButtonsPN.TabIndex = 0;
			// 
			// CancelBTN
			// 
			this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBTN.Location = new System.Drawing.Point(109, 8);
			this.CancelBTN.Name = "CancelBTN";
			this.CancelBTN.TabIndex = 0;
			this.CancelBTN.Text = "Close";
			// 
			// CategoriesLV
			// 
			this.CategoriesLV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CategoriesLV.Location = new System.Drawing.Point(0, 0);
			this.CategoriesLV.Name = "CategoriesLV";
			this.CategoriesLV.Size = new System.Drawing.Size(292, 202);
			this.CategoriesLV.TabIndex = 1;
			this.CategoriesLV.View = System.Windows.Forms.View.Details;
			// 
			// CategoriesViewDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.CancelBTN;
			this.ClientSize = new System.Drawing.Size(292, 238);
			this.Controls.Add(this.CategoriesLV);
			this.Controls.Add(this.ButtonsPN);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(600, 400);
			this.MinimumSize = new System.Drawing.Size(300, 180);
			this.Name = "CategoriesViewDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Available Event Categories";
			this.ButtonsPN.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		#region Private Members
		#endregion

		#region Public Interface
		/// <summary>
		/// Displays the event categories supported by the server.
		/// </summary>
		public void ShowDialog(Opc.Ae.Server server)
		{
			if (server == null) throw new ArgumentNullException("server");

			// clear list view.
			CategoriesLV.Clear();	

			// add columns.
			AddHeader("ID");
			AddHeader("Name");
			AddHeader("Event Type");

			// fetch and populate categories.
			try
			{
				FetchCategories(server, EventType.Simple);
				FetchCategories(server, EventType.Tracking);
				FetchCategories(server, EventType.Condition);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, this.Text);
			}

			// adjust column widths.
			AdjustColumns();

			// show dialog.
			ShowDialog();
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// Populates the list box with the categories.
		/// </summary>
		private void AddHeader(string name)
		{
			ColumnHeader header = new ColumnHeader();
			header.Text = name;
			CategoriesLV.Columns.Add(header);
		}

		/// <summary>
		/// Adjusts the columns shown in the list view.
		/// </summary>
		private void AdjustColumns()
		{
			// adjust column widths.
			for (int ii = 0; ii < CategoriesLV.Columns.Count; ii++)
			{
				CategoriesLV.Columns[ii].Width = -2;
			}
		}

		/// <summary>
		/// Populates the list box with the categories.
		/// </summary>
		private void FetchCategories(Opc.Ae.Server server, EventType eventType)
		{
			Category[] categories = server.QueryEventCategories((int)eventType);
            
			foreach (Category category in categories)
			{
				ListViewItem item = new ListViewItem(category.ID.ToString());

				item.SubItems.Add(category.Name);
				item.SubItems.Add(eventType.ToString());

				item.Tag = category;

				CategoriesLV.Items.Add(item);
			}
		}
		#endregion
	}
}
