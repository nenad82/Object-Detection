namespace ObjectMonitor
{
    partial class Form1
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.settignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSBCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iPCameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startDetectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.Location = new System.Drawing.Point(0, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(849, 517);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settignToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(849, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // settignToolStripMenuItem
            // 
            this.settignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectCameraToolStripMenuItem,
            this.startDetectToolStripMenuItem});
            this.settignToolStripMenuItem.Name = "settignToolStripMenuItem";
            this.settignToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.settignToolStripMenuItem.Text = "Camera Setting";
            // 
            // selectCameraToolStripMenuItem
            // 
            this.selectCameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSBCameraToolStripMenuItem,
            this.iPCameraToolStripMenuItem});
            this.selectCameraToolStripMenuItem.Name = "selectCameraToolStripMenuItem";
            this.selectCameraToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.selectCameraToolStripMenuItem.Text = "Select Camera";
            // 
            // uSBCameraToolStripMenuItem
            // 
            this.uSBCameraToolStripMenuItem.Checked = true;
            this.uSBCameraToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.uSBCameraToolStripMenuItem.Name = "uSBCameraToolStripMenuItem";
            this.uSBCameraToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.uSBCameraToolStripMenuItem.Text = "USB Camera";
            this.uSBCameraToolStripMenuItem.Click += new System.EventHandler(this.uSBCameraToolStripMenuItem_Click);
            // 
            // iPCameraToolStripMenuItem
            // 
            this.iPCameraToolStripMenuItem.Name = "iPCameraToolStripMenuItem";
            this.iPCameraToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.iPCameraToolStripMenuItem.Text = "IP Camera";
            this.iPCameraToolStripMenuItem.Click += new System.EventHandler(this.iPCameraToolStripMenuItem_Click);
            // 
            // startDetectToolStripMenuItem
            // 
            this.startDetectToolStripMenuItem.Name = "startDetectToolStripMenuItem";
            this.startDetectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.startDetectToolStripMenuItem.Text = "Connect Camera";
            this.startDetectToolStripMenuItem.Click += new System.EventHandler(this.startDetectToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.label1.Location = new System.Drawing.Point(383, 261);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Engine Initializing ...";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 539);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Object Detect";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startDetectToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem selectCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uSBCameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iPCameraToolStripMenuItem;
    }
}

