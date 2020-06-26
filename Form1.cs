using Microsoft.Office.Interop.Word;
using SimpleLogger;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.HtmlConverter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace PdfConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var dirPath = Properties.Settings.Default.DirPath;
            if (!string.IsNullOrEmpty(dirPath))
            {
                txtDir.Text = dirPath;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.DirPath = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                    txtDir.Text = fbd.SelectedPath;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtDir.Text))
            {
                MessageBox.Show("יש לבחור תיקייה");
                return;
            }
            SimpleLog.SetLogFile(".\\Log", "Log_");
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            string path = txtDir.Text;
            //string dPath = Path.Combine(path, "PDF");
            var dirInfo = new DirectoryInfo(path);
            var lDir = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
            var files = new List<string>();
            foreach (var dir in lDir)
            {
                var lFiles = Directory.GetFiles(dir.FullName, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var lfile in lFiles)
                {
                    files.Add(lfile);
                }
            }
            var counter = 0;

            // var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                try
                {
                    counter++;
                    var pct = Convert.ToDouble(Convert.ToDouble(counter) / Convert.ToDouble(files.Count())) * 100;
                    lblPb.Text = Path.GetFileName(file);

                    var dVal = progressBar1.Value + pct;
                    var val = Convert.ToInt32(dVal);
                    if (val > 100)
                    {
                        val = 100;
                    }
                    progressBar1.Value = val;
                    txtDetails.Text += @"Converting file : " + file + Environment.NewLine;
                    txtDetails.SelectionStart = txtDetails.Text.Length;
                    txtDetails.ScrollToCaret();
                    System.Windows.Forms.Application.DoEvents();
                    var ext = Path.GetExtension(file);
                    var fn = Path.GetFileNameWithoutExtension(file);
                    if (ext.ToLower() == ".tiff" || ext.ToLower() == ".tif" || ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                    {
                        PdfDocument doc = new PdfDocument();
                        PdfSection section = doc.Sections.Add();
                        PdfPageBase page = doc.Pages.Add();

                        //Load a tiff image from system
                        PdfImage image = PdfImage.FromFile(file);
                        //Set image display location and size in PDF
                        float widthFitRate = image.PhysicalDimension.Width / page.Canvas.ClientSize.Width;
                        float heightFitRate = image.PhysicalDimension.Height / page.Canvas.ClientSize.Height;
                        float fitRate = Math.Max(widthFitRate, heightFitRate);
                        float fitWidth = image.PhysicalDimension.Width / fitRate;
                        float fitHeight = image.PhysicalDimension.Height / fitRate;
                        page.Canvas.DrawImage(image, 0, 30, fitWidth, fitHeight);

                        //save and launch the file
                        doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        doc.Close();
                    }
                    else if (ext.ToLower() == ".html" || ext.ToLower() == ".htm")
                    {
                        PdfDocument doc = new PdfDocument();

                        PdfPageSettings setting = new PdfPageSettings();

                        setting.Size = new SizeF(1000, 1000);
                        setting.Margins = new PdfMargins(20);

                        PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
                        htmlLayoutFormat.IsWaiting = true;

                        Thread thread = new Thread(() =>
                        { doc.LoadFromFile(file, FileFormat.HTML); });
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start();
                        thread.Join();
                        //Save pdf file.
                        doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        doc.Close();
                    }
                    else if (ext.ToLower() == ".doc" || ext.ToLower() == ".docx")
                    {
                        var appWord = new Microsoft.Office.Interop.Word.Application();
                        appWord.Visible = false;
                        var wordDocument = appWord.Documents.Open(file);
                        wordDocument.ExportAsFixedFormat(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"), WdExportFormat.wdExportFormatPDF);
                        wordDocument.Close();
                        appWord.Quit();
                    }
                }
                catch (Exception ex)
                {
                    SimpleLog.Log(ex);
                    txtDetails.Text += Environment.NewLine;
                    txtDetails.Text += @"ERROR Converting file - " + file + "----" + ex.Message + Environment.NewLine;
                    txtDetails.Text += Environment.NewLine;
                }

            }
            progressBar1.Value = 100;
            System.Windows.Forms.Application.DoEvents();
            MessageBox.Show("הפעולה הסתיימה בהצלחה");


        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
