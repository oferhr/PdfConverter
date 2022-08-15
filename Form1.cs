using IronPdf;
using Microsoft.Office.Interop.Word;
using SimpleLogger;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace PdfConverter
{
    public partial class Form1 : Form
    {
        //private List<string> ConverterErrorFiles = new List<string>();
        //private List<DirectoryInfo> GoodDirs = new List<DirectoryInfo>();
        //private List<string> MergerErrorFiles = new List<string>();
        WebBrowser myWebBrowser = new WebBrowser();
        
        public Form1()
        {
            InitializeComponent();


            //   IronPdf.Logging.Logger.EnableDebugging = true;
           // IronPdf.Logging.Logger.
            IronPdf.Logging.Logger.LogFilePath = "Default.log"; //May be set to a directory name or full file
            IronPdf.Logging.Logger.LoggingMode = IronPdf.Logging.Logger.LoggingModes.All;

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
            btnStart.Enabled = false;
            SimpleLog.SetLogFile(".\\Log", "Log_");
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            System.Windows.Forms.Application.DoEvents();

            
            txtDetails.Text += @"Starting to convert files" + Environment.NewLine;
            txtDetails.SelectionStart = txtDetails.Text.Length;
            txtDetails.ScrollToCaret();
            System.Windows.Forms.Application.DoEvents();
            var ok = ConvertFiles();
            var msg = ok ? "הפעולה הסתיימה בהצלחה" : "הפעולה נכשלה. אנא בדוק את התקיות ואת קובץ הלוג";
            progressBar1.Value = 100;
            System.Windows.Forms.Application.DoEvents();
            MessageBox.Show(msg);
            //if (ConverterErrorFiles.Count() > 0)
            //{
            //    string sf = string.Empty;
            //    ConverterErrorFiles.ForEach(ff =>
            //    {
            //        SimpleLog.Log("Failed to convert file : " + ff);
            //        sf = sf + ff + Environment.NewLine;
            //    });
            //    MessageBox.Show("ההמרה נכשלה" + Environment.NewLine + "מספר קבצים שנכשלו : " + ConverterErrorFiles.Count() + Environment.NewLine + "הקבצים שנכשלו הם : " + Environment.NewLine + sf);
            //}
            //if (!ConvertFiles())
            //{
            //    if(ConverterErrorFiles.Count() > 0)
            //    {
            //        string sf = string.Empty;
            //        ConverterErrorFiles.ForEach(ff =>
            //        {
            //            SimpleLog.Log("Failed to convert file : " + ff);
            //            sf = sf + ff + Environment.NewLine;
            //        });
            //        MessageBox.Show("ההמרה נכשלה" + Environment.NewLine + "מספר קבצים שנכשלו : " + ConverterErrorFiles.Count() + Environment.NewLine + "הקבצים שנכשלו הם : " + Environment.NewLine + sf);
            //    }
            //    else
            //    {
            //        MessageBox.Show("המרת הקבצים נכשלה");
            //    }

            //    return;
            //}

            //if (!MergeAndClean())
            //{
            //    if (MergerErrorFiles.Count() > 0)
            //    {
            //        string sf = string.Empty;
            //        ConverterErrorFiles.ForEach(ff =>
            //        {
            //            SimpleLog.Log("Failed to merge file : " + ff);
            //            sf = sf + ff + Environment.NewLine;
            //        });
            //        MessageBox.Show("איחוד הקבצים נכשל" + Environment.NewLine + "מספר קבצים שנכשלו : " + ConverterErrorFiles.Count() + Environment.NewLine + "הקבצים שנכשלו הם : " + Environment.NewLine + sf);
            //    }
            //    else
            //    {
            //        MessageBox.Show("איחוד הקבצים נכשל");
            //    }

            //    return;
            //}


            
            
            btnStart.Enabled = true;

        }
        

       
        private bool ConvertFiles()
        {
           // PrinterClass.SetDefaultPrinter("Microsoft Print to PDF");
            bool convertOK = true;
            bool processOK = true;
            string path = txtDir.Text;
            string archive = txtArchive.Text;
            string archiveDirectory = Path.Combine(archive, DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!Directory.Exists(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }
            //string dPath = Path.Combine(path, "PDF");
            var dirInfo = new DirectoryInfo(path);
            
            var lDir = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
            var lallFiles = dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            var files = new List<string>();
            var goodFiles = new List<string>();
            var badFiles = new List<string>();
            var Now = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (var dir in lDir)
            {
                files = new List<string>();
                goodFiles = new List<string>();
                badFiles = new List<string>();
                var dest = Path.Combine(archiveDirectory, dir.Name);
                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }
                var archiveDirInfo = new DirectoryInfo(dest);
                CopyAll(dir, archiveDirInfo);

                var lFiles = Directory.GetFiles(dir.FullName, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var lfile in lFiles)
                {
                    files.Add(lfile);
                }
                var counter = 0;
                convertOK = true;
                foreach (var file in files)
                {
                    try
                    {
                        counter++;
                        var pct = Convert.ToDouble(Convert.ToDouble(counter) / Convert.ToDouble(lallFiles.Count())) * 100;
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
                            // ImageToPdfConverter.ImageToPdf(file).SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                            using (Image objImage = Image.FromFile(file))
                            {
                                PdfImage pdfimage = PdfImage.FromFile(file);

                                using (Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument { PageSettings = { Size = PdfPageSize.A4 } })
                                {
                                    PdfPageBase page = doc.Pages.Add(objImage.Size, new PdfMargins(0f));
                                    page.Canvas.DrawImage(pdfimage, new PointF(0, 0), objImage.Size);
                                    doc.SaveToFile(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                                    doc.Close();
                                }
                                //PdfUnitConvertor uinit = new PdfUnitConvertor();
                                //SizeF pageSize = uinit.ConvertFromPixels(objImage.Size, PdfGraphicsUnit.Point);

                            }
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
                            using(var converted = IronPdf.ImageToPdfConverter.ImageToPdf(file))
                            {
                                converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
                            }
                            
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
                            using (var Renderer = new HtmlToPdf())
                            {
                                Renderer.PrintOptions.InputEncoding = Encoding.GetEncoding(1255);
                                Renderer.PrintOptions.PrintHtmlBackgrounds = false;
                                Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
                                Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Print;

                                // Renderer.PrintOptions.PaperSize = IronPdf.Rendering.PdfCssMediaType.
                                // Renderer.PrintOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;

                                //Renderer.PrintOptions.EnableJavaScript = true;
                                //Renderer.PrintOptions.ViewPortWidth = 1280;
                                //Renderer.PrintOptions.RenderDelay = 500; //milliseconds
                                Renderer.PrintOptions.MarginLeft = 10;
                                Renderer.PrintOptions.MarginRight = 10;
                                Renderer.PrintOptions.MarginTop = 10;
                                Renderer.PrintOptions.MarginBottom = 10;
                                Renderer.PrintOptions.Zoom = 120;

                                using (var PDF = Renderer.RenderHTMLFileAsPdf(file))
                                {
                                    var OutputPath = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf");
                                    PDF.SaveAs(OutputPath);
                                }

                            }
                            //myWebBrowser.DocumentCompleted += myWebBrowser_DocumentCompleted;
                            //myWebBrowser.DocumentText = System.IO.File.ReadAllText(file);
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
                    
                        goodFiles.Add(file);
                        
                    }
                    catch (Exception ex)
                    {
                        convertOK = false;
                        badFiles.Add(file);
                        //ConverterErrorFiles.Add(file);
                        SimpleLog.Log(@"ERROR Converting file - " + file + "----" + ex.Message);
                        SimpleLog.Log(ex);
                        txtDetails.Text += Environment.NewLine;
                        txtDetails.Text += @"ERROR Converting file - " + file + "----" + ex.Message + Environment.NewLine;
                        txtDetails.Text += Environment.NewLine;
                    }

                }
                if(goodFiles.Count > 0)
                {
                    var mergedOK = MergeSingleDir(dir, Now, convertOK);
                    var archivedOK = ArchiveDirectory(goodFiles, dir, mergedOK, archiveDirectory);
                    if(convertOK && mergedOK && archivedOK)
                    {
                        Directory.Delete(dir.FullName, true);
                    }
                    else
                    {
                        processOK = false;
                    }
                }
                else
                {
                    processOK = false;
                }
                
                //if(isOK)
                //{
                //    try
                //    {
                //        GoodDirs.Add(new DirectoryInfo(dir.FullName));
                //        txtDetails.Text += @"Copy content from directory " + dir.Name + " to archive" + Environment.NewLine;
                //        txtDetails.SelectionStart = txtDetails.Text.Length;
                //        txtDetails.ScrollToCaret();
                //        System.Windows.Forms.Application.DoEvents();
                //        DirectoryInfo diSource = new DirectoryInfo(dir.FullName);
                //        DirectoryInfo diTarget = new DirectoryInfo(Path.Combine(archiveDirectory, dir.Name));
                //        CopyAll(diSource, diTarget);
                //    }
                //    catch (Exception ex)
                //    {
                //        SimpleLog.Log(ex);
                //        txtDetails.Text += Environment.NewLine;
                //        txtDetails.Text += @"ERROR Copying files from  " + dir.Name + "  to archive - " + ex.Message + Environment.NewLine;
                //        txtDetails.Text += Environment.NewLine;
                //        System.Windows.Forms.Application.DoEvents();
                //    }
                //}
                
            }

            return processOK;
            // var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            // return isOK;
        }
       private bool MergeSingleDir(DirectoryInfo dir, string Now, bool convertedOK)
        {
            string outputFile = string.Empty;
            var pdfDocuments = new List<IronPdf.PdfDocument>();
            try
            {
                txtDetails.Text += @"Merging folder - " + dir.Name + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();

                string targetDirectory = string.Empty;
                if (convertedOK)
                {
                    targetDirectory = Path.Combine(dir.Parent.FullName, Now);
                    if (!Directory.Exists(targetDirectory))
                    {
                        Directory.CreateDirectory(targetDirectory);
                    }
                    
                }
                else
                {
                    targetDirectory = dir.FullName;
                }

                

                var lFiles = Directory.GetFiles(dir.FullName, "*.pdf", SearchOption.TopDirectoryOnly);
                foreach (var lfile in lFiles)
                {
                    pdfDocuments.Add(IronPdf.PdfDocument.FromFile(lfile));
                }

                outputFile = Path.Combine(targetDirectory, dir.Name + ".pdf");
                using (var mergedPdfDocument = IronPdf.PdfDocument.Merge(pdfDocuments))
                {
                    mergedPdfDocument.SaveAs(outputFile);
                }
                
                

               

                return true;
            }
            catch (Exception ex)
            {
                //MergerErrorFiles.Add("Error trying to merge " + outputFile);
                SimpleLog.Log(ex);
                txtDetails.Text += Environment.NewLine;
                txtDetails.Text += @"ERROR Merging files - " + ex.Message + Environment.NewLine;
                txtDetails.Text += Environment.NewLine;
                System.Windows.Forms.Application.DoEvents();
                return false;
            }
        }
        private bool ArchiveDirectory(List<string> goodFiles, DirectoryInfo dir, bool DelAfterCopy, string archiveDirectory)
        {
            try
            {
                txtDetails.Text += @"Preparing folder :" + dir.Name + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();
                
                var innerDest = Path.Combine(dir.FullName, "PDFs");
                if(!DelAfterCopy && !Directory.Exists(innerDest))
                {
                    Directory.CreateDirectory(innerDest);
                }
                foreach (var gFile in goodFiles)
                {
                    var gFilePdf = Path.Combine(Path.GetDirectoryName(gFile), Path.GetFileNameWithoutExtension(gFile) + ".pdf");
                    if (DelAfterCopy)
                    {
                        File.Delete(gFile);
                        if (Path.GetExtension(gFile) != ".pdf" && File.Exists(gFilePdf))
                        {
                            File.Delete(gFilePdf);
                        }
                    }
                    else
                    {
                        if (Path.GetExtension(gFile) != ".pdf" && File.Exists(gFilePdf))
                        {
                            var gFilePdfDest = Path.Combine(innerDest, Path.GetFileNameWithoutExtension(gFile) + ".pdf");
                            File.Move(gFilePdf, gFilePdfDest);
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                SimpleLog.Log(ex);
                txtDetails.Text += Environment.NewLine;
                txtDetails.Text += @"ERROR Archiving  " + dir.Name + " - " + ex.Message + Environment.NewLine;
                txtDetails.Text += Environment.NewLine;
                System.Windows.Forms.Application.DoEvents();
                return false;
            }
        }
        //private void MergeFiles(List<IronPdf.PdfDocument> pdfDocuments)
        //{

        //}
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
        //private bool PrepareInviorment()
        //{
        //    try
        //    {
        //        string archive = txtArchive.Text;
        //        string targetDirectory = Path.Combine(archive, DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        txtDetails.Text += @"Creating Folder : " + targetDirectory + Environment.NewLine;
        //        txtDetails.SelectionStart = txtDetails.Text.Length;
        //        txtDetails.ScrollToCaret();
        //        System.Windows.Forms.Application.DoEvents();

        //        Directory.CreateDirectory(targetDirectory);

        //        var diSource = new DirectoryInfo(txtDir.Text);
        //        var diTarget = new DirectoryInfo(targetDirectory);

        //        txtDetails.Text += @"Copy content from base folder to archive" + Environment.NewLine;
        //        txtDetails.SelectionStart = txtDetails.Text.Length;
        //        txtDetails.ScrollToCaret();
        //        System.Windows.Forms.Application.DoEvents();

        //        CopyAll(diSource, diTarget);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        SimpleLog.Log(ex);
        //        txtDetails.Text += Environment.NewLine;
        //        txtDetails.Text += @"ERROR Copying files to archive - " + ex.Message + Environment.NewLine;
        //        txtDetails.Text += Environment.NewLine;
        //        System.Windows.Forms.Application.DoEvents();
        //        return false;
        //    }


        //}
        //private bool MergeAndClean()
        //{
        //    string currentFile = string.Empty;
        //    var pdfDocuments = new List<IronPdf.PdfDocument>();
        //    try
        //    {
        //        // string path = txtDir.Text;

        //        var Now = DateTime.Now.ToString("yyyyMMddHHmmss");




        //        foreach (var dir in GoodDirs)
        //        {
        //            string targetDirectory = Path.Combine(dir.Parent.FullName, Now);
        //            Directory.CreateDirectory(targetDirectory);

        //            txtDetails.Text += @"Merging folder - " + dir.Name + Environment.NewLine;
        //            txtDetails.SelectionStart = txtDetails.Text.Length;
        //            txtDetails.ScrollToCaret();
        //            System.Windows.Forms.Application.DoEvents();
        //            var lFiles = Directory.GetFiles(dir.FullName, "*.pdf", SearchOption.TopDirectoryOnly);
        //            foreach (var lfile in lFiles)
        //            {
        //                pdfDocuments.Add(IronPdf.PdfDocument.FromFile(lfile));
        //                //  llfiles.Add(lfile);
        //            }

        //            // var files = llfiles.ToArray();
        //            var outputFile = Path.Combine(targetDirectory, dir.Name + ".pdf");
        //            currentFile = outputFile;

        //            var mergedPdfDocument = IronPdf.PdfDocument.Merge(pdfDocuments);
        //            mergedPdfDocument.SaveAs(outputFile);
        //            //PdfDocumentBase doc = PdfDocument.MergeFiles(files);
        //            //doc.Save(outputFile, FileFormat.PDF);
        //            txtDetails.Text += @"Deleting folder - " + dir.Name + Environment.NewLine;
        //            txtDetails.SelectionStart = txtDetails.Text.Length;
        //            txtDetails.ScrollToCaret();
        //            System.Windows.Forms.Application.DoEvents();
        //            dir.Delete(true);
        //            //llfiles = new List<string>();
        //            pdfDocuments = new List<IronPdf.PdfDocument>();
        //        }



        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MergerErrorFiles.Add("Error trying to merge " + currentFile);
        //        SimpleLog.Log(ex);
        //        txtDetails.Text += Environment.NewLine;
        //        txtDetails.Text += @"ERROR Merging files - " + ex.Message + Environment.NewLine;
        //        txtDetails.Text += Environment.NewLine;
        //        System.Windows.Forms.Application.DoEvents();
        //        return false;
        //    }
        //    //open pdf documents           


        //}
    }
}
