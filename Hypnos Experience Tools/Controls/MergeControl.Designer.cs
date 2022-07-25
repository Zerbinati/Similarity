
namespace Hypnos.Experience.Tools
{
	partial class MergeControl
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
			this.btnMerge = new System.Windows.Forms.Button();
			this.listBoxExperienceFiles = new System.Windows.Forms.ListBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnMerge
			// 
			this.btnMerge.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnMerge.Location = new System.Drawing.Point(177, 176);
			this.btnMerge.Name = "btnMerge";
			this.btnMerge.Size = new System.Drawing.Size(130, 22);
			this.btnMerge.TabIndex = 4;
			this.btnMerge.Text = "Merge";
			this.btnMerge.UseVisualStyleBackColor = true;
			this.btnMerge.Click += new System.EventHandler(this.btnMerge_Click);
			// 
			// listBoxExperienceFiles
			// 
			this.listBoxExperienceFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listBoxExperienceFiles.HorizontalScrollbar = true;
			this.listBoxExperienceFiles.ItemHeight = 20;
			this.listBoxExperienceFiles.Location = new System.Drawing.Point(13, 26);
			this.listBoxExperienceFiles.Name = "listBoxExperienceFiles";
			this.listBoxExperienceFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
			this.listBoxExperienceFiles.Size = new System.Drawing.Size(368, 104);
			this.listBoxExperienceFiles.TabIndex = 5;
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(387, 26);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(94, 22);
			this.btnAdd.TabIndex = 6;
			this.btnAdd.Text = "Add";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.Location = new System.Drawing.Point(387, 52);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(94, 22);
			this.btnRemove.TabIndex = 7;
			this.btnRemove.Text = "Remove";
			this.btnRemove.UseVisualStyleBackColor = true;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// MergeControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.listBoxExperienceFiles);
			this.Controls.Add(this.btnMerge);
			this.Name = "MergeControl";
			this.Size = new System.Drawing.Size(484, 203);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnMerge;
		private System.Windows.Forms.ListBox listBoxExperienceFiles;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
	}
}
