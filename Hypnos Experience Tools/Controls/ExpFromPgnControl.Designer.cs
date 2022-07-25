
namespace Hypnos.Experience.Tools
{
	partial class ExpFromPgnControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnStartStop = new System.Windows.Forms.Button();
			this.listBoxPgnFiles = new System.Windows.Forms.ListBox();
			this.btnAddPgnFiles = new System.Windows.Forms.Button();
			this.btnRemoveSelectedPgn = new System.Windows.Forms.Button();
			this.btnAddPgnFolder = new System.Windows.Forms.Button();
			this.txtExpFilePath = new System.Windows.Forms.TextBox();
			this.btnBrowseExpFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.lblPgnInfo = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnStartStop
			// 
			this.btnStartStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnStartStop.Location = new System.Drawing.Point(177, 176);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(130, 22);
			this.btnStartStop.TabIndex = 4;
			this.btnStartStop.Text = "Merge";
			this.btnStartStop.UseVisualStyleBackColor = true;
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// listBoxPgnFiles
			// 
			this.listBoxPgnFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxPgnFiles.HorizontalScrollbar = true;
			this.listBoxPgnFiles.ItemHeight = 20;
			this.listBoxPgnFiles.Location = new System.Drawing.Point(13, 26);
			this.listBoxPgnFiles.Name = "listBoxPgnFiles";
			this.listBoxPgnFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxPgnFiles.Size = new System.Drawing.Size(368, 104);
			this.listBoxPgnFiles.TabIndex = 5;
			// 
			// btnAddPgnFiles
			// 
			this.btnAddPgnFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddPgnFiles.Location = new System.Drawing.Point(387, 26);
			this.btnAddPgnFiles.Name = "btnAddPgnFiles";
			this.btnAddPgnFiles.Size = new System.Drawing.Size(94, 22);
			this.btnAddPgnFiles.TabIndex = 6;
			this.btnAddPgnFiles.Text = "Add files";
			this.btnAddPgnFiles.UseVisualStyleBackColor = true;
			this.btnAddPgnFiles.Click += new System.EventHandler(this.btnAddPgnFiles_Click);
			// 
			// btnRemoveSelectedPgn
			// 
			this.btnRemoveSelectedPgn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemoveSelectedPgn.Location = new System.Drawing.Point(387, 78);
			this.btnRemoveSelectedPgn.Name = "btnRemoveSelectedPgn";
			this.btnRemoveSelectedPgn.Size = new System.Drawing.Size(94, 22);
			this.btnRemoveSelectedPgn.TabIndex = 7;
			this.btnRemoveSelectedPgn.Text = "Remove";
			this.btnRemoveSelectedPgn.UseVisualStyleBackColor = true;
			this.btnRemoveSelectedPgn.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnAddPgnFolder
			// 
			this.btnAddPgnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAddPgnFolder.Location = new System.Drawing.Point(387, 52);
			this.btnAddPgnFolder.Name = "btnAddPgnFolder";
			this.btnAddPgnFolder.Size = new System.Drawing.Size(94, 22);
			this.btnAddPgnFolder.TabIndex = 8;
			this.btnAddPgnFolder.Text = "Add folder";
			this.btnAddPgnFolder.UseVisualStyleBackColor = true;
			this.btnAddPgnFolder.Click += new System.EventHandler(this.btnAddPgnFolder_Click);
			// 
			// txtExpFilePath
			// 
			this.txtExpFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtExpFilePath.Location = new System.Drawing.Point(91, 141);
			this.txtExpFilePath.Name = "txtExpFilePath";
			this.txtExpFilePath.Size = new System.Drawing.Size(240, 26);
			this.txtExpFilePath.TabIndex = 9;
			// 
			// btnBrowseExpFile
			// 
			this.btnBrowseExpFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseExpFile.Location = new System.Drawing.Point(338, 140);
			this.btnBrowseExpFile.Name = "btnBrowseExpFile";
			this.btnBrowseExpFile.Size = new System.Drawing.Size(44, 22);
			this.btnBrowseExpFile.TabIndex = 10;
			this.btnBrowseExpFile.Text = "...";
			this.btnBrowseExpFile.UseVisualStyleBackColor = true;
			this.btnBrowseExpFile.Click += new System.EventHandler(this.btnBrowseExpFile_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 144);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "Experience file";
			// 
			// lblPgnInfo
			// 
			this.lblPgnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblPgnInfo.Location = new System.Drawing.Point(106, 10);
			this.lblPgnInfo.Name = "lblPgnInfo";
			this.lblPgnInfo.Size = new System.Drawing.Size(271, 13);
			this.lblPgnInfo.TabIndex = 12;
			this.lblPgnInfo.Text = "xxx PGN files (size = xxx)";
			this.lblPgnInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(108, 20);
			this.label2.TabIndex = 13;
			this.label2.Text = "Add PGN files";
			// 
			// ExpFromPgnControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblPgnInfo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnBrowseExpFile);
			this.Controls.Add(this.txtExpFilePath);
			this.Controls.Add(this.btnAddPgnFolder);
			this.Controls.Add(this.btnRemoveSelectedPgn);
			this.Controls.Add(this.btnAddPgnFiles);
			this.Controls.Add(this.listBoxPgnFiles);
			this.Controls.Add(this.btnStartStop);
			this.Name = "ExpFromPgnControl";
			this.Size = new System.Drawing.Size(484, 203);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.ListBox listBoxPgnFiles;
		private System.Windows.Forms.Button btnAddPgnFiles;
		private System.Windows.Forms.Button btnRemoveSelectedPgn;
		private System.Windows.Forms.Button btnAddPgnFolder;
		private System.Windows.Forms.TextBox txtExpFilePath;
		private System.Windows.Forms.Button btnBrowseExpFile;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblPgnInfo;
		private System.Windows.Forms.Label label2;
	}
}
