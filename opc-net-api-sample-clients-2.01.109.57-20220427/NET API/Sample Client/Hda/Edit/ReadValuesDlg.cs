using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Opc.Hda.SampleClient
{
	/// <summary>
	/// Summary description for ItemAddDlg.
	/// </summary>
	public class ReadValuesDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Splitter SplitterV;
		private System.Windows.Forms.Panel RightPN;
		private System.Windows.Forms.Panel LeftPN;
		private Opc.Hda.SampleClient.BrowseTreeCtrl BrowseCTRL;
		private System.Windows.Forms.Panel ButtonsPN;
		private System.Windows.Forms.Button BackBTN;
		private System.Windows.Forms.Button NextBTN;
		private System.Windows.Forms.Button CancelBTN;
		private System.Windows.Forms.Button DoneBTN;
		private Opc.Hda.SampleClient.ItemValuesCtrl ResultsCTRL;
		private Opc.Hda.SampleClient.ResultViewCtrl ItemsCTRL;
		private Opc.Hda.SampleClient.TrendItemsCtrl TrendItemsCTRL;
		private Opc.Hda.SampleClient.ResultListCtrl AsyncResultsCTRL;
		private Opc.Hda.SampleClient.TrendEditCtrl TrendCTRL;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ReadValuesDlg()
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
			this.SplitterV = new System.Windows.Forms.Splitter();
			this.RightPN = new System.Windows.Forms.Panel();
			this.AsyncResultsCTRL = new Opc.Hda.SampleClient.ResultListCtrl();
			this.ResultsCTRL = new Opc.Hda.SampleClient.ItemValuesCtrl();
			this.TrendItemsCTRL = new Opc.Hda.SampleClient.TrendItemsCtrl();
			this.BrowseCTRL = new Opc.Hda.SampleClient.BrowseTreeCtrl();
			this.LeftPN = new System.Windows.Forms.Panel();
			this.ItemsCTRL = new Opc.Hda.SampleClient.ResultViewCtrl();
			this.ButtonsPN = new System.Windows.Forms.Panel();
			this.BackBTN = new System.Windows.Forms.Button();
			this.NextBTN = new System.Windows.Forms.Button();
			this.DoneBTN = new System.Windows.Forms.Button();
			this.CancelBTN = new System.Windows.Forms.Button();
			this.TrendCTRL = new Opc.Hda.SampleClient.TrendEditCtrl();
			this.RightPN.SuspendLayout();
			this.LeftPN.SuspendLayout();
			this.ButtonsPN.SuspendLayout();
			this.SuspendLayout();
			// 
			// SplitterV
			// 
			this.SplitterV.Location = new System.Drawing.Point(328, 0);
			this.SplitterV.Name = "SplitterV";
			this.SplitterV.Size = new System.Drawing.Size(3, 386);
			this.SplitterV.TabIndex = 12;
			this.SplitterV.TabStop = false;
			// 
			// RightPN
			// 
			this.RightPN.Controls.Add(this.AsyncResultsCTRL);
			this.RightPN.Controls.Add(this.ResultsCTRL);
			this.RightPN.Controls.Add(this.TrendItemsCTRL);
			this.RightPN.Controls.Add(this.BrowseCTRL);
			this.RightPN.Dock = System.Windows.Forms.DockStyle.Fill;
			this.RightPN.DockPadding.Right = 4;
			this.RightPN.DockPadding.Top = 4;
			this.RightPN.Location = new System.Drawing.Point(331, 0);
			this.RightPN.Name = "RightPN";
			this.RightPN.Size = new System.Drawing.Size(605, 386);
			this.RightPN.TabIndex = 13;
			// 
			// AsyncResultsCTRL
			// 
			this.AsyncResultsCTRL.AllowDrop = true;
			this.AsyncResultsCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.AsyncResultsCTRL.Location = new System.Drawing.Point(0, 4);
			this.AsyncResultsCTRL.Name = "AsyncResultsCTRL";
			this.AsyncResultsCTRL.Size = new System.Drawing.Size(601, 382);
			this.AsyncResultsCTRL.TabIndex = 4;
			// 
			// ResultsCTRL
			// 
			this.ResultsCTRL.DisplayGraph = false;
			this.ResultsCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ResultsCTRL.Location = new System.Drawing.Point(0, 4);
			this.ResultsCTRL.Name = "ResultsCTRL";
			this.ResultsCTRL.ReadOnly = true;
			this.ResultsCTRL.Size = new System.Drawing.Size(601, 382);
			this.ResultsCTRL.TabIndex = 2;
			// 
			// TrendItemsCTRL
			// 
			this.TrendItemsCTRL.AllowDrop = true;
			this.TrendItemsCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TrendItemsCTRL.Location = new System.Drawing.Point(0, 4);
			this.TrendItemsCTRL.Name = "TrendItemsCTRL";
			this.TrendItemsCTRL.Size = new System.Drawing.Size(601, 382);
			this.TrendItemsCTRL.TabIndex = 3;
			// 
			// BrowseCTRL
			// 
			this.BrowseCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BrowseCTRL.Location = new System.Drawing.Point(0, 4);
			this.BrowseCTRL.Name = "BrowseCTRL";
			this.BrowseCTRL.Size = new System.Drawing.Size(601, 382);
			this.BrowseCTRL.TabIndex = 1;
			this.BrowseCTRL.ItemSelected += new Opc.Hda.SampleClient.BrowseTreeCtrl.ItemSelectedEventHandler(this.BrowseCTRL_ItemSelected);
			this.BrowseCTRL.ItemPicked += new Opc.Hda.SampleClient.BrowseTreeCtrl.ItemPickedEventHandler(this.BrowseCTRL_ItemPicked);
			// 
			// LeftPN
			// 
			this.LeftPN.Controls.Add(this.TrendCTRL);
			this.LeftPN.Controls.Add(this.ItemsCTRL);
			this.LeftPN.Dock = System.Windows.Forms.DockStyle.Left;
			this.LeftPN.DockPadding.Left = 4;
			this.LeftPN.DockPadding.Top = 4;
			this.LeftPN.Location = new System.Drawing.Point(0, 0);
			this.LeftPN.Name = "LeftPN";
			this.LeftPN.Size = new System.Drawing.Size(328, 386);
			this.LeftPN.TabIndex = 14;
			// 
			// ItemsCTRL
			// 
			this.ItemsCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ItemsCTRL.Location = new System.Drawing.Point(4, 4);
			this.ItemsCTRL.Name = "ItemsCTRL";
			this.ItemsCTRL.Size = new System.Drawing.Size(324, 382);
			this.ItemsCTRL.TabIndex = 4;
			this.ItemsCTRL.ResultSelected += new Opc.Hda.SampleClient.ResultViewCtrl.ResultSelectedEventHandler(this.ItemsCTRL_ResultSelected);
			// 
			// ButtonsPN
			// 
			this.ButtonsPN.Controls.Add(this.BackBTN);
			this.ButtonsPN.Controls.Add(this.NextBTN);
			this.ButtonsPN.Controls.Add(this.DoneBTN);
			this.ButtonsPN.Controls.Add(this.CancelBTN);
			this.ButtonsPN.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.ButtonsPN.Location = new System.Drawing.Point(0, 386);
			this.ButtonsPN.Name = "ButtonsPN";
			this.ButtonsPN.Size = new System.Drawing.Size(936, 36);
			this.ButtonsPN.TabIndex = 15;
			// 
			// BackBTN
			// 
			this.BackBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BackBTN.Location = new System.Drawing.Point(696, 8);
			this.BackBTN.Name = "BackBTN";
			this.BackBTN.TabIndex = 3;
			this.BackBTN.Text = "< Back";
			this.BackBTN.Click += new System.EventHandler(this.BackBTN_Click);
			// 
			// NextBTN
			// 
			this.NextBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.NextBTN.Location = new System.Drawing.Point(776, 8);
			this.NextBTN.Name = "NextBTN";
			this.NextBTN.TabIndex = 2;
			this.NextBTN.Text = "Next >";
			this.NextBTN.Click += new System.EventHandler(this.NextBTN_Click);
			// 
			// DoneBTN
			// 
			this.DoneBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.DoneBTN.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DoneBTN.Location = new System.Drawing.Point(856, 8);
			this.DoneBTN.Name = "DoneBTN";
			this.DoneBTN.TabIndex = 0;
			this.DoneBTN.Text = "Done";
			// 
			// CancelBTN
			// 
			this.CancelBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.CancelBTN.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CancelBTN.Location = new System.Drawing.Point(856, 8);
			this.CancelBTN.Name = "CancelBTN";
			this.CancelBTN.TabIndex = 5;
			this.CancelBTN.Text = "Cancel";
			// 
			// TrendCTRL
			// 
			this.TrendCTRL.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TrendCTRL.Location = new System.Drawing.Point(4, 4);
			this.TrendCTRL.Name = "TrendCTRL";
			this.TrendCTRL.Size = new System.Drawing.Size(324, 382);
			this.TrendCTRL.TabIndex = 5;
			// 
			// ReadValuesDlg
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(936, 422);
			this.Controls.Add(this.RightPN);
			this.Controls.Add(this.SplitterV);
			this.Controls.Add(this.LeftPN);
			this.Controls.Add(this.ButtonsPN);
			this.Name = "ReadValuesDlg";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Read Values";
			this.RightPN.ResumeLayout(false);
			this.LeftPN.ResumeLayout(false);
			this.ButtonsPN.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		#region Public Interface	
		/// <summary>
		/// Prompts the user to select an item and specify trend properties.
		/// </summary>
		public ItemValueCollection ShowDialog(Opc.Hda.Server server, RequestType type, bool synchronous)
		{
			if (server == null) throw new ArgumentNullException("server");

			m_server      = server;
			m_type        = type;
			m_synchronous = synchronous;
			m_singleItem  = true;
			m_results     = null;

			// create new trend.
			m_trend = new Trend(m_server);

			// set reasonable defaults.
            m_trend.StartTime = new Time("NOW-18MO");
            m_trend.EndTime = new Time("NOW");

			BrowseCTRL.Browse(m_server, null);
			TrendItemsCTRL.Initialize(m_trend, false, null);
			TrendCTRL.Initialize(m_trend, type);
			ItemsCTRL.Initialize(null);
			ResultsCTRL.Initialize(m_server, null);
			AsyncResultsCTRL.Initialize(m_server, null);

			// update dialog state.
			SetState();

			// show dialog.
			bool result = (ShowDialog() == DialogResult.OK);

			// release item handles.
			m_trend.ClearItems();

			// return item values.
			return (result)?m_results[0]:null;
		}

		/// <summary>
		/// Prompts the user to select trend items and specify trend properties.
		/// </summary>
		public ItemValueCollection[] ShowDialog(Opc.Hda.Trend trend, RequestType type, bool synchronous)
		{
			if (trend == null) throw new ArgumentNullException("trend");

			m_server      = trend.Server;
			m_trend       = trend;
			m_type        = type;
			m_synchronous = synchronous;
			m_singleItem  = false;
			m_results     = null;

			BrowseCTRL.Browse(m_server, null);
			TrendItemsCTRL.Initialize(trend, false, null);
			TrendCTRL.Initialize(m_trend, type);
			ItemsCTRL.Initialize(null);
			ResultsCTRL.Initialize(m_server, null);

			// update dialog state.
			SetState();

			// show dialog.
			bool result = (ShowDialog() == DialogResult.OK);

			// return item values.
			return m_results;
		}
		#endregion

		#region Private Members
		/// <summary>
		/// The historian database server.
		/// </summary>
		private Opc.Hda.Server m_server = null;

		/// <summary>
		/// The trend used to read the data.
		/// </summary>
		private Trend m_trend = null;

		/// <summary>
		/// The item to read.
		/// </summary>
		private ItemIdentifier m_item = null;
		
		/// <summary>
		/// The requst object for an asynchronous read.
		/// </summary>
		private IRequest m_request = null;

		/// <summary>
		/// Whether an asynchronous request was sent (and possibly failed).
		/// </summary>
		private bool m_asyncSent = false;

		/// <summary>
		/// The set of new items which will be added to the trend.
		/// </summary>
		private ItemValueCollection[] m_results = null;

		/// <summary>
		/// Type of read operation.
		/// </summary>
		private RequestType m_type = RequestType.ReadRaw;

		/// <summary>
		/// Whether to use synchronous or asynchronous reads.
		/// </summary>
		private bool m_synchronous = true;

		/// <summary>
		/// Whether only a single item is being read.
		/// </summary>
		private bool m_singleItem = false;

		/// <summary>
		/// Synchronously eads the values from the server.
		/// </summary>
		private ItemValueCollection[] SyncRead(Item[] items)
		{
			switch (m_type)
			{
				// synchronous read raw or read processed.
				case RequestType.Read:
				{
					return m_trend.Read(items);
				}

				// synchronous read raw.
				case RequestType.ReadRaw:
				{
					return m_trend.ReadRaw(items);
				}
					
				// synchronous read processed.
				case RequestType.ReadProcessed:
				{
					return m_trend.ReadProcessed(items);
				}
					
				// synchronous read modified.
				case RequestType.ReadModified:
				{
					return m_trend.ReadModified(items);
				}
						
				// synchronous read at time.
				case RequestType.ReadAtTime:
				{
					return m_trend.ReadAtTime(items);
				}
			}

			return null;
		}

		/// <summary>
		/// Asynchronously eads the values from the server.
		/// </summary>
		private IdentifiedResult[] AsyncRead(Item[] items)
		{
			IdentifiedResult[] results = null;

			switch (m_type)
			{
				// begin asynchronous read raw or read processed.
				case RequestType.Read:
				{
					results = m_trend.Read(
						items,
						null,
						new ReadValuesEventHandler(OnReadComplete),
						out m_request);

					break;
				}

				// begin asynchronous read raw.
				case RequestType.ReadRaw:
				{
					results = m_trend.ReadRaw(
						items,
						null,
						new ReadValuesEventHandler(OnReadComplete),
						out m_request);

					break;
				}
					
				// begin asynchronous read processed.
				case RequestType.ReadProcessed:
				{
					results = m_trend.ReadProcessed(
						items,
						null,
						new ReadValuesEventHandler(OnReadComplete),
						out m_request);

					break;
				}

				// begin asynchronous read modified.
				case RequestType.ReadModified:
				{	
					results = m_trend.ReadModified(
						items,
						null,
						new ReadValuesEventHandler(OnReadComplete),
						out m_request);

					break;
				}
						
				// begin asynchronous read at time.
				case RequestType.ReadAtTime:
				{		
					results = m_trend.ReadAtTime(
						items,
						null,
						new ReadValuesEventHandler(OnReadComplete),
						out m_request);

					break;
				}
			}

			return results;
		}

		/// <summary>
		/// Creates a server handle for the selected item and reads the data.
		/// </summary>
		private void DoItemRead()
		{
			// create item (if necessary).
			Item item = m_trend.Items[m_item];

			if (item == null)
			{
				item = m_trend.AddItem(m_item);
			}

			// get request parameters from controls.
			TrendCTRL.Update(m_trend);

			if (m_synchronous)
			{
				// read data.
				ItemValueCollection[] results = SyncRead(new Item[] { item });
	
				if (results == null || results.Length != 1)
				{
					throw new InvalidResponseException();
				}

				// display results.
				ItemsCTRL.Initialize(results);

				// save results.
				m_results = results;
			}
			else
			{
				// check if already waiting for results.
				if (m_asyncSent)
				{
					return;
				}

				// begin read data.
				IdentifiedResult[] results = AsyncRead(new Item[] { item });
	
				if (results == null || results.Length != 1)
				{
					throw new InvalidResponseException();
				}

				// display initial results.
				AsyncResultsCTRL.Initialize(m_server, results);
				m_asyncSent = true;
			}		
		}

		/// <summary>
		/// Creates reads the data for the selected items.
		/// </summary>
		private void DoTrendRead()
		{
			Item[] items = TrendItemsCTRL.GetItems(false);

			if (items == null)
			{
				return;
			}

			// get request parameters from controls.
			TrendCTRL.Update(m_trend);

			if (m_synchronous)
			{
				// read data.
				ItemValueCollection[] results = SyncRead(items);
	
				if (results == null || results.Length != items.Length)
				{
					throw new InvalidResponseException();
				}

				// display results.
				ItemsCTRL.Initialize(results);

				// save results.
				m_results = results;
			}
			else
			{
				// check if already waiting for results.
				if (m_asyncSent)
				{
					return;
				}

				// begin read data.
				IdentifiedResult[] results = AsyncRead(items);
	
				if (results == null || results.Length != items.Length)
				{
					throw new InvalidResponseException();
				}

				// display initial results.
				AsyncResultsCTRL.Initialize(m_server, results);			
				m_asyncSent = true;
			}		
		}

		/// <summary>
		/// Remove server handles for new items.
		/// </summary>
		private void UndoRead()
		{
			m_results   = null;
			m_asyncSent = false;

			if (m_request != null)
			{
				m_server.CancelRequest(m_request, new CancelCompleteEventHandler(OnCancelComplete));
				m_request = null;
			}
		}

		/// <summary>
		/// Called when a read request completes.
		/// </summary>
		public void OnReadComplete(IRequest request, ItemValueCollection[] results)
		{
			// check if dialog has closed.
			if (IsDisposed)
			{
				return;
			}

			// check if invoke is required.
			if (InvokeRequired)
			{
				BeginInvoke(new ReadValuesEventHandler(OnReadComplete), new object[] { request, results });
				return;
			}
						
			try
			{				
				// enable next button since first batch of results have arrived.
				if (m_results == null)
				{
					ItemsCTRL.Initialize(results);
					m_results = results;
					NextBTN.Enabled = true;
				}

				// merge results with any existing results.
				else
				{
					m_results = ItemsCTRL.AppendValues(results);
				}
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		/// <summary>
		/// Called when a cancel request completes.
		/// </summary>
		public void OnCancelComplete(IRequest request)
		{
			// check if invoke is required.
			if (InvokeRequired)
			{
				BeginInvoke(new CancelCompleteEventHandler(OnCancelComplete), new object[] { request });
				return;
			}
			
			// check if dialog has closed.
			if (IsDisposed)
			{
				return;
			}

			MessageBox.Show("Asynchronous read successfully cancelled.");
		}

		/// <summary>
		/// Toggle control visibility based on the dialog state.
		/// </summary>
		private void SetState()
		{
			if (m_results != null)
			{
				NextBTN.Enabled          = false;
				BackBTN.Enabled          = true;
				DoneBTN.Visible          = true;
				CancelBTN.Visible        = false;
				TrendCTRL.Visible        = false;
				BrowseCTRL.Visible       = false;
				TrendItemsCTRL.Visible   = false;
				ItemsCTRL.Visible        = true;
				ResultsCTRL.Visible      = true;
				AsyncResultsCTRL.Visible = false;
			}
			else if (m_asyncSent)
			{
				NextBTN.Enabled          = m_results != null;
				BackBTN.Enabled          = true;
				DoneBTN.Visible          = false;
				CancelBTN.Visible        = true;
				TrendCTRL.Visible        = true;
				BrowseCTRL.Visible       = false;
				TrendItemsCTRL.Visible   = false;
				ItemsCTRL.Visible        = false;
				ResultsCTRL.Visible      = false;
				AsyncResultsCTRL.Visible = true;
			}
			else
			{
				NextBTN.Enabled          = !m_singleItem || m_type == RequestType.ReadRaw;
				BackBTN.Enabled          = false;
				DoneBTN.Visible          = false;
				CancelBTN.Visible        = true;
				TrendCTRL.Visible        = true;
				BrowseCTRL.Visible       = m_singleItem;
				TrendItemsCTRL.Visible   = !m_singleItem;
				ItemsCTRL.Visible        = false;
				ResultsCTRL.Visible      = false;
				AsyncResultsCTRL.Visible = false;

				BrowseCTRL.ClearSelection();
			}
		}
		#endregion

		/// <summary>
		/// Adds the current set of items to server.
		/// </summary>
		private void NextBTN_Click(object sender, System.EventArgs e)
		{
			try
			{
				// do single item read.
				if (m_singleItem)
				{
					if (m_item == null)
					{
						return;
					}

					// read values.
					DoItemRead();
				}

				// do multiple item read.
				else
				{
					// read values.
					DoTrendRead();
				}

				// update dialog state.
				SetState();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		/// <summary>
		/// Removes the items and goes back to the select items view.
		/// </summary>
		private void BackBTN_Click(object sender, System.EventArgs e)
		{
			try
			{
				// releases handles for items.
				UndoRead();

				// display results.
				ItemsCTRL.Initialize(null);
				ResultsCTRL.Initialize(m_server, null);

				// update dialog state.
				SetState();
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}

		/// <summary>
		/// Toggles the state of the next button based on the current selection.
		/// </summary>
		private void BrowseCTRL_ItemSelected(Opc.ItemIdentifier item)
		{
			m_item = item;
			NextBTN.Enabled = item != null;
		}

		/// <summary>
		/// Activates the next button when an item is picked.
		/// </summary>
		private void BrowseCTRL_ItemPicked(Opc.ItemIdentifier[] items)
		{
			if (items != null && items.Length == 1)
			{
				m_item = items[0];
				NextBTN.Enabled = true;
				NextBTN_Click(BrowseCTRL, null);
			}	
		}

		/// <summary>
		/// Display the current selection in the result control.
		/// </summary>
		private void ItemsCTRL_ResultSelected(Opc.ItemIdentifier result)
		{
			if (typeof(ItemValueCollection).IsInstanceOfType(result))
			{
				ResultsCTRL.Initialize(m_server, (ItemValueCollection)result);
			}
		}
	}
}
