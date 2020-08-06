namespace PdfConverter
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
            this.btnStart = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblPb = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtDetails = new System.Windows.Forms.RichTextBox();
            this.btnBrowseArchive = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtArchive = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(644, 138);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "התחל";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(644, 29);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "בחר";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(750, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "בחר תיקייה";
            // 
            // txtDir
            // 
            this.txtDir.Location = new System.Drawing.Point(100, 32);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(516, 20);
            this.txtDir.TabIndex = 5;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(100, 114);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(619, 19);
            this.progressBar1.TabIndex = 9;
            this.progressBar1.Visible = false;
            // 
            // lblPb
            // 
            this.lblPb.AutoSize = true;
            this.lblPb.Location = new System.Drawing.Point(98, 67);
            this.lblPb.Name = "lblPb";
            this.lblPb.Size = new System.Drawing.Size(0, 13);
            this.lblPb.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(644, 352);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "סגור";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtDetails
            // 
            this.txtDetails.BackColor = System.Drawing.Color.Silver;
            this.txtDetails.Location = new System.Drawing.Point(26, 178);
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ReadOnly = true;
            this.txtDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(770, 154);
            this.txtDetails.TabIndex = 12;
            this.txtDetails.Text = "";
            // 
            // btnBrowseArchive
            // 
            this.btnBrowseArchive.Location = new System.Drawing.Point(644, 64);
            this.btnBrowseArchive.Name = "btnBrowseArchive";
            this.btnBrowseArchive.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseArchive.TabIndex = 15;
            this.btnBrowseArchive.Text = "בחר";
            this.btnBrowseArchive.UseVisualStyleBackColor = true;
            this.btnBrowseArchive.Click += new System.EventHandler(this.btnBrowseArchive_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(739, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "תקיית ארכיון";
            // 
            // txtArchive
            // 
            this.txtArchive.Location = new System.Drawing.Point(100, 67);
            this.txtArchive.Name = "txtArchive";
            this.txtArchive.Size = new System.Drawing.Size(516, 20);
            this.txtArchive.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 378);
            this.Controls.Add(this.btnBrowseArchive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtArchive);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblPb);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDir);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblPb;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox txtDetails;
        private System.Windows.Forms.Button btnBrowseArchive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtArchive;
    }
}

