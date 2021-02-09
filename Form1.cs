using IronPdf;
using Microsoft.Office.Interop.Word;

using SimpleLogger;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace PdfConverter
{
    public partial class Form1 : Form
    {
        private List<string> ConverterErrorFiles = new List<string>();
        private List<string> MergerErrorFiles = new List<string>();
        WebBrowser myWebBrowser = new WebBrowser();
        
        public Form1()
        {
            InitializeComponent();
            
            var archivePath = Properties.Settings.Default.ArchivePath;
            if (!string.IsNullOrEmpty(archivePath))
            {
                txtArchive.Text = archivePath;
            }
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

            if (string.IsNullOrEmpty(txtDir.Text) || string.IsNullOrEmpty(txtArchive.Text))
            {
                MessageBox.Show("יש לבחור תיקייה");
                return;
            }
            SimpleLog.SetLogFile(".\\Log", "Log_");
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            System.Windows.Forms.Application.DoEvents();

            if (!PrepareInviorment())
            {
                MessageBox.Show("העתקת קבצים לארכיון נכשלה");
                return;
            }
            txtDetails.Text += @"Finnish copying files to archive" + Environment.NewLine;
            txtDetails.Text += @"Starting to convert files" + Environment.NewLine;
            txtDetails.SelectionStart = txtDetails.Text.Length;
            txtDetails.ScrollToCaret();
            System.Windows.Forms.Application.DoEvents();
            if (!ConvertFiles())
            {
                if(ConverterErrorFiles.Count() > 0)
                {
                    string sf = string.Empty;
                    ConverterErrorFiles.ForEach(ff =>
                    {
                        SimpleLog.Log("Failed to convert file : " + ff);
                        sf = sf + ff + Environment.NewLine;
                    });
                    MessageBox.Show("ההמרה נכשלה" + Environment.NewLine + "מספר קבצים שנכשלו : " + ConverterErrorFiles.Count() + Environment.NewLine + "הקבצים שנכשלו הם : " + Environment.NewLine + sf);
                }
                else
                {
                    MessageBox.Show("המרת הקבצים נכשלה");
                }
               
                return;
            }

            if (!MergeAndClean())
            {
                if (MergerErrorFiles.Count() > 0)
                {
                    string sf = string.Empty;
                    ConverterErrorFiles.ForEach(ff =>
                    {
                        SimpleLog.Log("Failed to merge file : " + ff);
                        sf = sf + ff + Environment.NewLine;
                    });
                    MessageBox.Show("איחוד הקבצים נכשל" + Environment.NewLine + "מספר קבצים שנכשלו : " + ConverterErrorFiles.Count() + Environment.NewLine + "הקבצים שנכשלו הם : " + Environment.NewLine + sf);
                }
                else
                {
                    MessageBox.Show("איחוד הקבצים נכשל");
                }

                return;
            }


            progressBar1.Value = 100;
            System.Windows.Forms.Application.DoEvents();
            MessageBox.Show("הפעולה הסתיימה בהצלחה");


        }
        private bool MergeAndClean()
        {
            string currentFile = string.Empty;
            var pdfDocuments = new List<PdfDocument>();
            try
            {
                string path = txtDir.Text;
                string targetDirectory = Path.Combine(path, DateTime.Now.ToString("yyyyMMddHHmmss"));
                Directory.CreateDirectory(targetDirectory);


                var dirInfo = new DirectoryInfo(path);
                var lDir = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
                var llfiles = new List<string>();
                foreach (var dir in lDir)
                {
                    if (dir.FullName == targetDirectory)
                    {
                        continue;
                    }
                    txtDetails.Text += @"Merging folder - " + dir.Name + Environment.NewLine;
                    txtDetails.SelectionStart = txtDetails.Text.Length;
                    txtDetails.ScrollToCaret();
                    System.Windows.Forms.Application.DoEvents();
                    var lFiles = Directory.GetFiles(dir.FullName, "*.pdf", SearchOption.TopDirectoryOnly);
                    foreach (var lfile in lFiles)
                    {
                        pdfDocuments.Add(PdfDocument.FromFile(lfile));
                        //  llfiles.Add(lfile);
                    }

                   // var files = llfiles.ToArray();
                    var outputFile = Path.Combine(targetDirectory, dir.Name + ".pdf");
                    currentFile = outputFile;

                    var mergedPdfDocument = PdfDocument.Merge(pdfDocuments);
                    mergedPdfDocument.SaveAs(outputFile);
                    //PdfDocumentBase doc = PdfDocument.MergeFiles(files);
                    //doc.Save(outputFile, FileFormat.PDF);
                    txtDetails.Text += @"Deleting folder - " + dir.Name + Environment.NewLine;
                    txtDetails.SelectionStart = txtDetails.Text.Length;
                    txtDetails.ScrollToCaret();
                    System.Windows.Forms.Application.DoEvents();
                    dir.Delete(true);
                    //llfiles = new List<string>();
                    pdfDocuments = new List<PdfDocument>();
                }

                return true;
            }
            catch (Exception ex)
            {
                MergerErrorFiles.Add("Error trying to merge " + currentFile);
                SimpleLog.Log(ex);
                txtDetails.Text += Environment.NewLine;
                txtDetails.Text += @"ERROR Merging files - " + ex.Message + Environment.NewLine;
                txtDetails.Text += Environment.NewLine;
                System.Windows.Forms.Application.DoEvents();
                return false;
            }
            //open pdf documents           
            

        }

        private bool PrepareInviorment()
        {
            try
            {
                string archive = txtArchive.Text;
                string targetDirectory = Path.Combine(archive, DateTime.Now.ToString("yyyyMMddHHmmss"));
                txtDetails.Text += @"Creating Folder : " + targetDirectory + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();

                Directory.CreateDirectory(targetDirectory);

                var diSource = new DirectoryInfo(txtDir.Text);
                var diTarget = new DirectoryInfo(targetDirectory);

                txtDetails.Text += @"Copy content from base folder to archive" + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();

                CopyAll(diSource, diTarget);
                return true;
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                txtDetails.Text += Environment.NewLine;
                txtDetails.Text += @"ERROR Copying files to archive - "  + ex.Message + Environment.NewLine;
                txtDetails.Text += Environment.NewLine;
                System.Windows.Forms.Application.DoEvents();
                return false;
            }

            
        }
        private bool ConvertFiles()
        {
           // PrinterClass.SetDefaultPrinter("Microsoft Print to PDF");
            bool isOK = true;
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
                    if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                    {
                        ImageToPdfConverter.ImageToPdf(file).SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        //using (Image objImage = Image.FromFile(file))
                        //{
                        //    PdfImage pdfimage = PdfImage.FromFile(file);

                        //    using (PdfDocument doc = new PdfDocument { PageSettings = { Size = PdfPageSize.A4 } })
                        //    {
                        //        PdfPageBase page = doc.Pages.Add(objImage.Size, new PdfMargins(0f));
                        //        page.Canvas.DrawImage(pdfimage, new PointF(0, 0), objImage.Size);
                        //        doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        //        doc.Close();
                        //    }
                        //        //PdfUnitConvertor uinit = new PdfUnitConvertor();
                        //        //SizeF pageSize = uinit.ConvertFromPixels(objImage.Size, PdfGraphicsUnit.Point);
                                
                        //}
                        //Load a tiff image from system

                        //doc.Close();
                        //PdfSection section = doc.Sections.Add();
                        //PdfPageBase page = doc.Pages.Add();


                        ////Set image display location and size in PDF
                        //float widthFitRate = image.PhysicalDimension.Width / page.Canvas.ClientSize.Width;
                        //float heightFitRate = image.PhysicalDimension.Height / page.Canvas.ClientSize.Height;
                        //float fitRate = Math.Max(widthFitRate, heightFitRate);
                        //float fitWidth = image.PhysicalDimension.Width / fitRate;
                        //float fitHeight = image.PhysicalDimension.Height / fitRate;
                        //page.Canvas.DrawImage(image, 0, 30, fitWidth, fitHeight);

                        ////save and launch the file
                        //doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        //doc.Close();
                    }
                    else if (ext.ToLower() == ".tiff" || ext.ToLower() == ".tif")
                    {
                        //Image tiffImage = Image.FromFile(file);
                        //Image[] images = SplitTIFFImage(tiffImage);
                        //var converted = ImageToPdfConverter.ImageToPdf(images, ImageBehavior.CropPage);
                        //converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));

                        var converted = IronPdf.ImageToPdfConverter.ImageToPdf(file);
                        converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        //using (PdfDocument doc = new PdfDocument())
                        //{
                        //    doc.PageSettings.Size = PdfPageSize.A4;
                        //    Image tiffImage = Image.FromFile(file);
                        //    Image[] images = SplitTIFFImage(tiffImage);

                        //    for (int i = 0; i < images.Length; i++)
                        //    {
                        //        PdfImage pdfImg = PdfImage.FromImage(images[i]);
                        //        PdfPageBase page = doc.Pages.Add(new SizeF(pdfImg.Width, pdfImg.Height));
                        //        page.Canvas.DrawImage(pdfImg, new PointF(0, 0), new SizeF(pdfImg.Width, pdfImg.Height));
                        //    }
                        //    doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));



                        //}

                    }
                    else if (ext.ToLower() == ".html" || ext.ToLower() == ".htm")
                    {
                       
                        myWebBrowser.DocumentCompleted += myWebBrowser_DocumentCompleted;
                        myWebBrowser.DocumentText = System.IO.File.ReadAllText(file);
                        //myWebBrowser.
                       

                        
                        //PrintDocument doc = new PrintDocument()
                        //{
                        //    PrinterSettings = new PrinterSettings()
                        //    {

                        //        // set the printer to 'Microsoft Print to PDF'
                        //        PrinterName = "Microsoft Print to PDF",

                        //        // tell the object this document will print to file
                        //        PrintToFile = true,

                        //        // set the filename to whatever you like (full path)
                        //        PrintFileName = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf")
                        //    }
                        //};

                        //doc.Print();
                        ////Spire.Pdf.HtmlConverter.Qt.HtmlConverter.PluginPath = @"plugins";
                        ////string outputFile = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf");
                        ////Spire.Pdf.HtmlConverter.Qt.HtmlConverter.Convert(file, outputFile, true, 120 * 1000, new SizeF(612, 792), new PdfMargins(0, 0));
                        ////// Spire.Pdf.HtmlConverter.Qt.HtmlConverter.Convert(file, outputFile, true, 100 * 1000, new SizeF(1000, 1000), new Spire.Pdf.Graphics.PdfMargins(20));
                        //try
                        //{
                        //    //Spire.Pdf.HtmlConverter.Qt.HtmlConverter.PluginPath = @"plugins";
                        //    //string outputFile = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf");
                        //    //Spire.Pdf.HtmlConverter.Qt.HtmlConverter.Convert(file, outputFile, true, 120 * 1000, new SizeF(612, 792), new PdfMargins(0, 0));
                        //    using (PdfDocument doc = new PdfDocument())
                        //    {
                        //        PdfPageSettings setting = new PdfPageSettings();

                        //        //setting.Height = 700;
                        //        //setting.Width = 2000;
                        //        //setting.Orientation = PdfPageOrientation.Landscape;
                        //        setting.Margins = new PdfMargins(50);
                        //        setting.Size = PdfPageSize.A4;


                        //        PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
                        //        htmlLayoutFormat.IsWaiting = true;
                        //        htmlLayoutFormat.FitToPage = Clip.Both;


                        //        var html = File.ReadAllText(file, Encoding.GetEncoding("Windows-1255"));

                        //        doc.LoadFromHTML(html, true, setting, htmlLayoutFormat, true);
                        //        //Thread thread = new Thread(() =>
                        //        //{ doc.LoadFromHTML(html, true, setting, htmlLayoutFormat, true); });
                        //        //thread.SetApartmentState(ApartmentState.STA);
                        //        //thread.Start();
                        //        //thread.Join();
                        //        //doc.PageSettings = setting;

                        //        //doc.LoadFromFile(file, FileFormat.HTML);
                        //        //doc.PageScaling = PdfPrintPageScaling.ActualSize;
                        //        // doc.PrintSettings.SelectSinglePageLayout(Spire.Pdf.Print.PdfSinglePageScalingMode.ActualSize);
                        //        doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                        //        doc.Close();
                        //    }
                        //}
                        //catch(OutOfMemoryException e)
                        //{
                        //    Spire.Pdf.HtmlConverter.Qt.HtmlConverter.PluginPath = @"plugins";
                        //    string outputFile = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf");
                        //    Spire.Pdf.HtmlConverter.Qt.HtmlConverter.Convert(file, outputFile, true, 120 * 1000, new SizeF(612, 792), new PdfMargins(0, 0));
                        //}
                        //catch (Exception ex)
                        //{
                        //    if(ex.Message.ToLower()== "out of memory.")
                        //    {


                        //    }
                        //}




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
                    isOK = false;
                    ConverterErrorFiles.Add(file);
                    SimpleLog.Log(@"ERROR Converting file - " + file + "----" + ex.Message);
                    SimpleLog.Log(ex);
                    txtDetails.Text += Environment.NewLine;
                    txtDetails.Text += @"ERROR Converting file - " + file + "----" + ex.Message + Environment.NewLine;
                    txtDetails.Text += Environment.NewLine;
                }

            }
            return isOK;
        }
        //private static void StartBrowser(string source)
        //{
        //    var th = new Thread(() =>
        //    {
        //        var webBrowser = new WebBrowser();
        //        webBrowser.ScrollBarsEnabled = false;
        //        webBrowser.IsWebBrowserContextMenuEnabled = true;
        //        webBrowser.AllowNavigation = true;

        //        webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
        //        webBrowser.DocumentText = source;

        //        Application.Run();
        //    });
        //    th.SetApartmentState(ApartmentState.STA);
        //    th.Start();
        //}
        //static void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        //{
        //    var webBrowser = (WebBrowser)sender;
        //    using (Bitmap bitmap =
        //        new Bitmap(
        //            webBrowser.Width,
        //            webBrowser.Height))
        //    {
        //        webBrowser
        //            .DrawToBitmap(
        //            bitmap,
        //            new System.Drawing
        //                .Rectangle(0, 0, bitmap.Width, bitmap.Height));
        //        bitmap.Save(@"filename.jpg",
        //            System.Drawing.Imaging.ImageFormat.Jpeg);
        //    }
        //}
        private void myWebBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            myWebBrowser.Print();
            //IHTMLDocument2 d2;
            //d2 = (IHTMLDocument2)((WebBrowser)sender).Document.DomDocument;

            //d2.execCommand("Print", false, null);
        }
        //public static Image[] SplitTIFFImage(Image tiffImage)
        //{
        //    int frameCount = tiffImage.GetFrameCount(FrameDimension.Page);
        //    Image[] images = new Image[frameCount];
        //    Guid objGuid = tiffImage.FrameDimensionsList[0];
        //    FrameDimension objDimension = new FrameDimension(objGuid);
        //    for (int i = 0; i<frameCount; i++)
        //    {
        //        tiffImage.SelectActiveFrame(objDimension, i);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            tiffImage.Save(ms, ImageFormat.Tiff);
        //            images[i] = Image.FromStream(ms);
        //        }
        //    }
        //    return images;
        //}

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnBrowseArchive_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Properties.Settings.Default.ArchivePath = fbd.SelectedPath;
                    Properties.Settings.Default.Save();
                    txtArchive.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
