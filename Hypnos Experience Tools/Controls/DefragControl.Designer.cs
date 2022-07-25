
namespace Hypnos.Experience.Tools
{
	partial class DefragControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxExperienceFile = new System.Windows.Forms.TextBox();
			this.btnBrowseExperienceFile = new System.Windows.Forms.Button();
			this.btnDefrag = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(4, 68);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(124, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select file to defragment:";
			// 
			// textBoxExperienceFile
			// 
			this.textBoxExperienceFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxExperienceFile.Location = new System.Drawing.Point(197, 64);
			this.textBoxExperienceFile.Name = "textBoxExperienceFile";
			this.textBoxExperienceFile.Size = new System.Drawing.Size(166, 20);
			this.textBoxExperienceFile.TabIndex = 1;
			// 
			// btnBrowseExperienceFile
			// 
			this.btnBrowseExperienceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowseExperienceFile.Location = new System.Drawing.Point(374, 62);
			this.btnBrowseExperienceFile.Name = "btnBrowseExperienceFile";
			this.btnBrowseExperienceFile.Size = new System.Drawing.Size(94, 22);
			this.btnBrowseExperienceFile.TabIndex = 2;
			this.btnBrowseExperienceFile.Text = "Browse";
			this.btnBrowseExperienceFile.UseVisualStyleBackColor = true;
			this.btnBrowseExperienceFile.Click += new System.EventHandler(this.btnBrowseExperienceFile_Click);
			// 
			// btnDefrag
			// 
			this.btnDefrag.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnDefrag.Location = new System.Drawing.Point(177, 176);
			this.btnDefrag.Name = "btnDefrag";
			this.btnDefrag.Size = new System.Drawing.Size(130, 22);
			this.btnDefrag.TabIndex = 3;
			this.btnDefrag.Text = "Defrag";
			this.btnDefrag.UseVisualStyleBackColor = true;
			this.btnDefrag.Click += new System.EventHandler(this.btnDefrag_Click);
			// 
			// DefragControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.btnDefrag);
			this.Controls.Add(this.btnBrowseExperienceFile);
			this.Controls.Add(this.textBoxExperienceFile);
			this.Controls.Add(this.label1);
			this.Name = "DefragControl";
			this.Size = new System.Drawing.Size(484, 203);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxExperienceFile;
		private System.Windows.Forms.Button btnBrowseExperienceFile;
		private System.Windows.Forms.Button btnDefrag;
	}
}
