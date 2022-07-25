
namespace Hypnos.Experience.Tools
{
	partial class MainForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.tabControl = new System.Windows.Forms.TabControl();
			this.tabDefrag = new System.Windows.Forms.TabPage();
			this.defragControl = new Hypnos.Experience.Tools.DefragControl();
			this.tabMerge = new System.Windows.Forms.TabPage();
			this.mergeControl = new Hypnos.Experience.Tools.MergeControl();
			this.tabExpFromPgn = new System.Windows.Forms.TabPage();
			this.expFromPgnControl = new Hypnos.Experience.Tools.ExpFromPgnControl();
			this.btnExit = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textEnginePath = new System.Windows.Forms.TextBox();
			this.btnBrowseEngine = new System.Windows.Forms.Button();
			this.richEngineOutput = new System.Windows.Forms.RichTextBox();
			this.tabControl.SuspendLayout();
			this.tabDefrag.SuspendLayout();
			this.tabMerge.SuspendLayout();
			this.tabExpFromPgn.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabDefrag);
			this.tabControl.Controls.Add(this.tabMerge);
			this.tabControl.Controls.Add(this.tabExpFromPgn);
			this.tabControl.Location = new System.Drawing.Point(14, 31);
			this.tabControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(496, 233);
			this.tabControl.TabIndex = 3;
			// 
			// tabDefrag
			// 
			this.tabDefrag.Controls.Add(this.defragControl);
			this.tabDefrag.Location = new System.Drawing.Point(4, 22);
			this.tabDefrag.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabDefrag.Name = "tabDefrag";
			this.tabDefrag.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabDefrag.Size = new System.Drawing.Size(488, 207);
			this.tabDefrag.TabIndex = 0;
			this.tabDefrag.Text = "Defrag";
			this.tabDefrag.UseVisualStyleBackColor = true;
			// 
			// defragControl
			// 
			this.defragControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.defragControl.Location = new System.Drawing.Point(2, 2);
			this.defragControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.defragControl.Name = "defragControl";
			this.defragControl.Size = new System.Drawing.Size(484, 203);
			this.defragControl.TabIndex = 0;
			// 
			// tabMerge
			// 
			this.tabMerge.Controls.Add(this.mergeControl);
			this.tabMerge.Location = new System.Drawing.Point(4, 22);
			this.tabMerge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabMerge.Name = "tabMerge";
			this.tabMerge.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabMerge.Size = new System.Drawing.Size(488, 207);
			this.tabMerge.TabIndex = 1;
			this.tabMerge.Text = "Merge";
			this.tabMerge.UseVisualStyleBackColor = true;
			// 
			// mergeControl
			// 
			this.mergeControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mergeControl.Location = new System.Drawing.Point(2, 2);
			this.mergeControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.mergeControl.Name = "mergeControl";
			this.mergeControl.Size = new System.Drawing.Size(484, 203);
			this.mergeControl.TabIndex = 0;
			// 
			// tabExpFromPgn
			// 
			this.tabExpFromPgn.Controls.Add(this.expFromPgnControl);
			this.tabExpFromPgn.Location = new System.Drawing.Point(4, 22);
			this.tabExpFromPgn.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabExpFromPgn.Name = "tabExpFromPgn";
			this.tabExpFromPgn.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.tabExpFromPgn.Size = new System.Drawing.Size(488, 207);
			this.tabExpFromPgn.TabIndex = 2;
			this.tabExpFromPgn.Text = "Experience from PGN";
			this.tabExpFromPgn.UseVisualStyleBackColor = true;
			// 
			// expFromPgnControl
			// 
			this.expFromPgnControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.expFromPgnControl.Location = new System.Drawing.Point(2, 2);
			this.expFromPgnControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.expFromPgnControl.Name = "expFromPgnControl";
			this.expFromPgnControl.Size = new System.Drawing.Size(484, 203);
			this.expFromPgnControl.TabIndex = 0;
			// 
			// btnExit
			// 
			this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnExit.Location = new System.Drawing.Point(225, 448);
			this.btnExit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new System.Drawing.Size(63, 20);
			this.btnExit.TabIndex = 5;
			this.btnExit.Text = "Exit";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(14, 266);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Engine output:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 10);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Hypnos engine: ";
			// 
			// textEnginePath
			// 
			this.textEnginePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEnginePath.Location = new System.Drawing.Point(107, 8);
			this.textEnginePath.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.textEnginePath.Name = "textEnginePath";
			this.textEnginePath.ReadOnly = true;
			this.textEnginePath.Size = new System.Drawing.Size(335, 20);
			this.textEnginePath.TabIndex = 1;
			// 
			// btnBrowseEngine
			// 
			this.btnBrowseEngine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseEngine.Location = new System.Drawing.Point(445, 8);
			this.btnBrowseEngine.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.btnBrowseEngine.Name = "btnBrowseEngine";
			this.btnBrowseEngine.Size = new System.Drawing.Size(63, 20);
			this.btnBrowseEngine.TabIndex = 2;
			this.btnBrowseEngine.Text = "Browse";
			this.btnBrowseEngine.UseVisualStyleBackColor = true;
			this.btnBrowseEngine.Click += new System.EventHandler(this.btnBrowseEngine_Click);
			// 
			// richEngineOutput
			// 
			this.richEngineOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richEngineOutput.BackColor = System.Drawing.Color.Black;
			this.richEngineOutput.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richEngineOutput.ForeColor = System.Drawing.Color.White;
			this.richEngineOutput.Location = new System.Drawing.Point(14, 284);
			this.richEngineOutput.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.richEngineOutput.Name = "richEngineOutput";
			this.richEngineOutput.ReadOnly = true;
			this.richEngineOutput.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.richEngineOutput.Size = new System.Drawing.Size(497, 154);
			this.richEngineOutput.TabIndex = 4;
			this.richEngineOutput.Text = "";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnExit;
			this.ClientSize = new System.Drawing.Size(523, 480);
			this.Controls.Add(this.richEngineOutput);
			this.Controls.Add(this.btnBrowseEngine);
			this.Controls.Add(this.textEnginePath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.tabControl);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.MaximumSize = new System.Drawing.Size(1339, 1070);
			this.MinimumSize = new System.Drawing.Size(539, 436);
			this.Name = "MainForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MainForm";
			this.tabControl.ResumeLayout(false);
			this.tabDefrag.ResumeLayout(false);
			this.tabMerge.ResumeLayout(false);
			this.tabExpFromPgn.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabDefrag;
		private System.Windows.Forms.TabPage tabMerge;
		private System.Windows.Forms.Button btnExit;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textEnginePath;
		private System.Windows.Forms.Button btnBrowseEngine;
		private DefragControl defragControl;
		private MergeControl mergeControl;
		private System.Windows.Forms.RichTextBox richEngineOutput;
		private System.Windows.Forms.TabPage tabExpFromPgn;
		private ExpFromPgnControl expFromPgnControl;
	}
}
