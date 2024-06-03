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
            components = new System.ComponentModel.Container();
            btnStart = new System.Windows.Forms.Button();
            btnBrowse = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            txtDir = new System.Windows.Forms.TextBox();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            lblPb = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            txtDetails = new System.Windows.Forms.RichTextBox();
            btnBrowseArchive = new System.Windows.Forms.Button();
            label2 = new System.Windows.Forms.Label();
            txtArchive = new System.Windows.Forms.TextBox();
            btnDel = new System.Windows.Forms.Button();
            bindingSource1 = new System.Windows.Forms.BindingSource(components);
            txtQuality = new System.Windows.Forms.NumericUpDown();
            label3 = new System.Windows.Forms.Label();
            bZip = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtQuality).BeginInit();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new System.Drawing.Point(751, 166);
            btnStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(88, 27);
            btnStart.TabIndex = 8;
            btnStart.Text = "התחל";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new System.Drawing.Point(751, 33);
            btnBrowse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new System.Drawing.Size(88, 27);
            btnBrowse.TabIndex = 7;
            btnBrowse.Text = "בחר";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(875, 39);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(66, 15);
            label1.TabIndex = 6;
            label1.Text = "בחר תיקייה";
            // 
            // txtDir
            // 
            txtDir.Location = new System.Drawing.Point(117, 37);
            txtDir.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDir.Name = "txtDir";
            txtDir.Size = new System.Drawing.Size(601, 23);
            txtDir.TabIndex = 5;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            progressBar1.Location = new System.Drawing.Point(117, 132);
            progressBar1.Margin = new System.Windows.Forms.Padding(2);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(722, 22);
            progressBar1.TabIndex = 9;
            progressBar1.Visible = false;
            // 
            // lblPb
            // 
            lblPb.AutoSize = true;
            lblPb.Location = new System.Drawing.Point(114, 77);
            lblPb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPb.Name = "lblPb";
            lblPb.Size = new System.Drawing.Size(0, 15);
            lblPb.TabIndex = 10;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(751, 386);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(88, 27);
            button1.TabIndex = 11;
            button1.Text = "סגור";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // txtDetails
            // 
            txtDetails.BackColor = System.Drawing.Color.Silver;
            txtDetails.Location = new System.Drawing.Point(37, 203);
            txtDetails.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtDetails.Name = "txtDetails";
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            txtDetails.Size = new System.Drawing.Size(898, 177);
            txtDetails.TabIndex = 12;
            txtDetails.Text = "";
            // 
            // btnBrowseArchive
            // 
            btnBrowseArchive.Location = new System.Drawing.Point(751, 74);
            btnBrowseArchive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBrowseArchive.Name = "btnBrowseArchive";
            btnBrowseArchive.Size = new System.Drawing.Size(88, 27);
            btnBrowseArchive.TabIndex = 15;
            btnBrowseArchive.Text = "בחר";
            btnBrowseArchive.UseVisualStyleBackColor = true;
            btnBrowseArchive.Click += btnBrowseArchive_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(862, 85);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(73, 15);
            label2.TabIndex = 14;
            label2.Text = "תקיית ארכיון";
            // 
            // txtArchive
            // 
            txtArchive.Location = new System.Drawing.Point(117, 77);
            txtArchive.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtArchive.Name = "txtArchive";
            txtArchive.Size = new System.Drawing.Size(601, 23);
            txtArchive.TabIndex = 13;
            // 
            // btnDel
            // 
            btnDel.Location = new System.Drawing.Point(523, 166);
            btnDel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnDel.Name = "btnDel";
            btnDel.Size = new System.Drawing.Size(88, 27);
            btnDel.TabIndex = 16;
            btnDel.Text = "מחק תקיות";
            btnDel.UseVisualStyleBackColor = true;
            btnDel.Click += btnDel_Click;
            // 
            // txtQuality
            // 
            txtQuality.Location = new System.Drawing.Point(117, 170);
            txtQuality.Name = "txtQuality";
            txtQuality.Size = new System.Drawing.Size(46, 23);
            txtQuality.TabIndex = 18;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(169, 172);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(347, 15);
            label3.TabIndex = 19;
            label3.Text = "איכות התמונה כך ש 100 זה האיכות הכי טובה ו 0 האיכות הכי גרועה";
            // 
            // bZip
            // 
            bZip.Location = new System.Drawing.Point(630, 166);
            bZip.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            bZip.Name = "bZip";
            bZip.Size = new System.Drawing.Size(88, 27);
            bZip.TabIndex = 20;
            bZip.Text = "חילוץ ZIPs";
            bZip.UseVisualStyleBackColor = true;
            bZip.Click += bZip_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(962, 415);
            Controls.Add(bZip);
            Controls.Add(label3);
            Controls.Add(txtQuality);
            Controls.Add(btnDel);
            Controls.Add(btnBrowseArchive);
            Controls.Add(label2);
            Controls.Add(txtArchive);
            Controls.Add(txtDetails);
            Controls.Add(button1);
            Controls.Add(lblPb);
            Controls.Add(progressBar1);
            Controls.Add(btnStart);
            Controls.Add(btnBrowse);
            Controls.Add(label1);
            Controls.Add(txtDir);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtQuality).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.NumericUpDown txtQuality;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bZip;
    }
}

