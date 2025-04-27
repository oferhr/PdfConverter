using IronPdf;
using Microsoft.Office.Interop.Word;
using SimpleLogger;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Xml.Linq;
//using Excel = Microsoft.Office.Interop.Excel;


namespace PdfConverter
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            IronPdf.Logging.Logger.LoggingMode = IronPdf.Logging.Logger.LoggingModes.DebugOutputWindow;
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

            SimpleLog.Log(@"Start of operation - " + DateTime.Now.ToString());
            txtDetails.Text += @"Starting to convert files" + Environment.NewLine;
            txtDetails.SelectionStart = txtDetails.Text.Length;
            txtDetails.ScrollToCaret();
            System.Windows.Forms.Application.DoEvents();
            var ok = ConvertFiles();
            var msg = ok ? "הפעולה הסתיימה בהצלחה" : "הפעולה נכשלה. אנא בדוק את התקיות ואת קובץ הלוג";
            progressBar1.Value = 100;
            System.Windows.Forms.Application.DoEvents();
            SimpleLog.Log(@"End of operation - " + DateTime.Now.ToString());
            MessageBox.Show(msg);





            btnStart.Enabled = true;

        }



        private bool ConvertFiles()
        {
            var files = new List<string>();
            var goodFiles = new List<string>();
            var badFiles = new List<string>();
            var Now = DateTime.Now.ToString("yyyyMMddHHmmss");
            var counter = 0;
            bool convertOK = true;
            bool processOK = true;
            string path = txtDir.Text;
            string archiveDirectory = createArchivePath();
            //string dPath = Path.Combine(path, "PDF");
            var dirInfo = new DirectoryInfo(path);

            var lDir = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).ToList();
            var lallFiles = dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
           
            ExtractZip(false);
            DeleteDirs();
            foreach (var dir in lDir)
            {
                files = new List<string>();
                goodFiles = new List<string>();
                badFiles = new List<string>();
                DirectoryInfo archiveDirInfo = ArchiveDir(archiveDirectory, dir);

                var lFiles = Directory.GetFiles(dir.FullName, "*.*", SearchOption.TopDirectoryOnly);
                foreach (var lfile in lFiles)
                {
                    files.Add(lfile);
                }

                convertOK = true;
                foreach (var file in files)
                {
                    try
                    {
                        counter++;
                        UpdareProgressBar(lallFiles, counter, file);

                        UpdateDetails(@"Converting file : " + file);

                        var ext = Path.GetExtension(file);
                        var fn = Path.GetFileNameWithoutExtension(file);
                        if (ext.ToLower() == ".jpg" || ext.ToLower() == ".jpeg")
                        {
                            ProcessJpg(file, fn);
                        }
                        else if (ext.ToLower() == ".tiff" || ext.ToLower() == ".tif")
                        {
                            ProcessTiff(file, fn);

                        }
                        else if (ext.ToLower() == ".html" || ext.ToLower() == ".htm")
                        {
                            ProcessHtml(file, fn);

                        }
                        else if (ext.ToLower() == ".doc" || ext.ToLower() == ".docx")
                        {
                            ConvertWord(file, fn);
                        }

                        goodFiles.Add(file);

                    }
                    catch (Exception ex)
                    {
                        convertOK = false;
                        badFiles.Add(file);
                        
                        SimpleLog.Log(@"ERROR Converting file - " + file + "----" + ex.Message);
                        SimpleLog.Log(ex);

                        UpdateDetails(@"ERROR Converting file - " + file + "----" + ex.Message);

                        moveToErrorDir(dir, archiveDirInfo);

                    }

                }
                if (goodFiles.Count > 0)
                {
                    var mergedOK = MergeSingleDir(dir, Now, convertOK);
                    var archivedOK = MoveToPdfsDirectory(goodFiles, dir, mergedOK);
                    if (convertOK && mergedOK && archivedOK)
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
            }

            return processOK;
        }
        private static void ConvertWord(string file, string fn)
        {
            var appWord = new Microsoft.Office.Interop.Word.Application();
            appWord.Visible = false;
            var wordDocument = appWord.Documents.Open(file);
            wordDocument.ExportAsFixedFormat(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"), WdExportFormat.wdExportFormatPDF);
            wordDocument.Close();
            appWord.Quit();
        }
        private static void ProcessHtml(string file, string fn)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var Renderer = new ChromePdfRenderer();
            Renderer.RenderingOptions.InputEncoding = Encoding.GetEncoding(1255);
            Renderer.RenderingOptions.PrintHtmlBackgrounds = false;
            //Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
            //Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Print;

            Renderer.RenderingOptions.PaperSize = IronPdf.Rendering.PdfPaperSize.A4;
            Renderer.RenderingOptions.CssMediaType = IronPdf.Rendering.PdfCssMediaType.Print;

            //Renderer.PrintOptions.EnableJavaScript = true;
            //Renderer.PrintOptions.ViewPortWidth = 1280;
            //Renderer.PrintOptions.RenderDelay = 500; //milliseconds
            Renderer.RenderingOptions.MarginLeft = 10;
            Renderer.RenderingOptions.MarginRight = 10;
            Renderer.RenderingOptions.MarginTop = 10;
            Renderer.RenderingOptions.MarginBottom = 10;
            Renderer.RenderingOptions.Zoom = 100;

            using (var PDF = Renderer.RenderHtmlFileAsPdf(file))
            {
                var OutputPath = Path.Combine(Path.GetDirectoryName(file), fn + ".pdf");
                PDF.SaveAs(OutputPath);
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
        private static void ProcessTiff(string file, string fn)
        {
            //Image tiffImage = Image.FromFile(file);
            ////tiffImage.
            //Image[] images = SplitTIFFImage(tiffImage);
            ////var converted = ImageToPdfConverter.ImageToPdf(images, ImageBehavior.CropPage);
            ////converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
            //List<AnyBitmap> gifBitmaps = new List<AnyBitmap>();
            //Stream fileStrim = null;
            //for (int i = 0; i < images.Length; i++)
            //{
            //    AnyBitmap img = new AnyBitmap(images[i]);
            //    img.ExportStream(fileStrim, AnyBitmap.ImageFormat.Jpeg, 100);
            //    gifBitmaps.Add(img);
            //}
            //AnyBitmap bitmap = new AnyBitmap(file);
            //bitmap.ExportFile("losslogo.jpg", AnyBitmap.ImageFormat.Jpeg, 100);
            //AnyBitmap multiFrameTiff = AnyBitmap.CreateMultiFrameTiff(tiffBitmaps);
            //multiFrameTiff.SaveAs("multiTiffwcrops.tiff");
            //AnyBitmap bitmap = new AnyBitmap(file);

            //   AnyBitmap multiFrameGif = AnyBitmap.CreateMultiFrameGif(ConvertTiffToJpeg(file));
            //var paths = ConvertTiffToJpeg(file);
            using (var converted = ImageToPdfConverter.ImageToPdf(file))
            {
                //converted.CompressImages(10);
                converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
            }


            //FileInfo fi = new FileInfo(file);
            //var size = fi.Length;
            //if (size > 5000000)
            //{
            //    //var paths = ConvertTiffToJpeg(file);
            //    //using (var converted = ImageToPdfConverter.ImageToPdf(paths, IronPdf.Imaging.ImageBehavior.FitToPageAndMaintainAspectRatio))
            //    //{
            //    //    converted.CompressImages(10);
            //    //    converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
            //    //}
            //}
            //else
            //{
            //    using (var converted = ImageToPdfConverter.ImageToPdf(file, IronPdf.Imaging.ImageBehavior.FitToPageAndMaintainAspectRatio))
            //    {
            //        // converted.CompressImages(10);
            //        converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
            //    }
            //}
            //using (var converted = ImageToPdfConverter.ImageToPdf(file, IronPdf.Imaging.ImageBehavior.FitToPageAndMaintainAspectRatio))
            //{
            //    converted.CompressImages(10);
            //    converted.SaveAs(Path.Combine(Path.GetDirectoryName(file), fn + ".pdf"));
            //}
            // var paths = ConvertTiffToJpeg(file);





            //using (Spire.Pdf.PdfDocument doc = new Spire.Pdf.PdfDocument())
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
        private static void ProcessJpg(string file, string fn)
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
        private bool MergeSingleDir(DirectoryInfo dir, string Now, bool convertedOK)
        {
            var pdfDocuments = new List<IronPdf.PdfDocument>();
            try
            {
                UpdateDetails(@"Merging files in directory - " + dir.Name);
               
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
                string lfileName = string.Empty;
                try
                {
                    foreach (var lfile in lFiles)
                    {
                        lfileName = lfile;
                        pdfDocuments.Add(IronPdf.PdfDocument.FromFile(lfile));
                    }
                }
                catch (IronPdf.Exceptions.IronPdfNativeException)
                {
                    SimpleLog.Log("PDF FILE IS CORRUPT - " + lfileName);
                }
                catch (Exception)
                {
                    SimpleLog.Log("PDF FILE IS CORRUPT - " + lfileName);
                }

                string outputFile = Path.Combine(targetDirectory, dir.Name + ".pdf");
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
                UpdateDetails(@"ERROR Merging files - " + ex.Message);
                return false;
            }
        }
        private bool MoveToPdfsDirectory(List<string> goodFiles, DirectoryInfo dir, bool DelAfterCopy)
        {
            try
            {
                txtDetails.Text += @"Preparing folder :" + dir.Name + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();

                var innerDest = Path.Combine(dir.FullName, "PDFs");
                if (!DelAfterCopy && !Directory.Exists(innerDest))
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
                UpdateDetails(@"ERROR Archiving files - " + ex.Message);
                return false;
            }
        }
        public  void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                UpdateDetails($"Copying {target.FullName} => {fi.Name}");
                //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
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
        private void DeleteDirs()
        {
            var excelPath = System.Configuration.ConfigurationManager.AppSettings["ExcelForDelete"];
            if (string.IsNullOrEmpty(excelPath))
            {
                MessageBox.Show("נתיב לקובץ אקסל לא קיים");
                return;
            }
            
            string path = txtDir.Text;
            var dirInfo = new DirectoryInfo(path);
            var lallFiles = dirInfo.GetFiles("*.*", SearchOption.AllDirectories).ToList();
            var lst = new List<string>();
            var lstXml = new List<string>();
            try
            {
                UpdateDetails("Open Excel File");
                
                using (var workbook = new XLWorkbook(excelPath))
                {
                    // Get the first worksheet
                    var worksheet = workbook.Worksheet(1);

                    // Get the used range (only cells with data)
                    var range = worksheet.RangeUsed();

                    // Get the number of rows
                    int rowCount = range.RowCount();

                    // Skip the header row (if you have headers)
                    // Start from row 2 if you have headers, otherwise start from row 1
                    int startRow = 2; // Change to 1 if you don't have headers

                    // Read each row and add to appropriate list
                    for (int row = startRow; row <= rowCount; row++)
                    {
                        // Read column 1 (A) and add to first list
                        if (!worksheet.Cell(row, 1).IsEmpty())
                        {
                            string value1 = worksheet.Cell(row, 1).GetValue<string>();
                            lst.Add(value1.Trim());
                        }

                        // Read column 2 (B) and add to second list
                        if (!worksheet.Cell(row, 2).IsEmpty())
                        {
                            string value2 = worksheet.Cell(row, 2).GetValue<string>();
                            lstXml.Add(value2.Trim());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                MessageBox.Show("נכשל בקריאת נתונים מאקסל");
                return;
            }
            
            string fileName = string.Empty;
            try
            {
                UpdateDetails("Found " + lallFiles.Count() + " files to parse");
                
                foreach (var file in lallFiles)
                {
                    if(!IsValidDirectory(Path.GetDirectoryName(file.FullName)))
                    {
                        continue;
                    }
                    fileName = Path.GetFileNameWithoutExtension(file.FullName);
                    foreach (var phrase in lst)
                    {

                        if (fileName.StartsWith(phrase))
                        {
                            UpdateDetails(@"Deleting file : " + fileName);
                           
                            File.Delete(file.FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails(@"Error deleting file : " + fileName + " - " + ex.Message);
            }

            ProcessDirectories(lstXml);

            MessageBox.Show("הקבצים נמחקו בהצלחה");

        }
        private bool IsValidDirectory(string directoryPath)
        {
            // Get the directory name from the provided path
            string directoryName = Path.GetFileName(directoryPath);

            // Check if the directory name contains an 18-character string
            var match = System.Text.RegularExpressions.Regex.Match(directoryName, @"\d{18}");
            if (match.Success)
            {
                // Extract the 18-character string
                string matchedString = match.Value;

                // Check the last character of the string
                if (matchedString[^1] == '1') // Last character is '1'
                {
                    return false;
                }
            }

            // Return true if no match or last character is not '1'
            return true;
        }
        public void ProcessDirectories(List<string> lstXml)
        {
            try
            {
                var rootPath = txtDir.Text;
                // Process the current directory and all subdirectories recursively
                foreach (var directory in Directory.GetDirectories(rootPath, "*", SearchOption.AllDirectories)
                                                 .Prepend(rootPath)) // Include the root directory
                {
                    if (!IsValidDirectory(directory))
                    {
                        continue;
                    }
                    ProcessSingleDirectory(directory, lstXml);
                }
                UpdateDetails(@"Processing completed successfully.");
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails(@"Error during directory processing: " + ex.Message);
            }
        }
        private void ProcessSingleDirectory(string directoryPath, List<string> lstXml)
        {
            try
            {
                UpdateDetails(@"Processing directory: " + directoryPath);
                

                // Find the XML file matching the pattern Index_XXXXXXXXX.xml
                string xmlFile = FindIndexFile(directoryPath);

                if (xmlFile == null)
                {
                   // Console.WriteLine($"No matching XML file found in {directoryPath}. Skipping.");
                    return;
                }

                

                // Process the XML file
                ProcessXmlFile(xmlFile, directoryPath, lstXml);
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails($"Error processing directory {directoryPath}: {ex.Message}");
            }
        }
        private string FindIndexFile(string directoryPath)
        {
            // Search for files matching the pattern
            string[] matchingFiles = Directory.GetFiles(directoryPath, "index_*.xml");


            return matchingFiles.Length > 0 ? matchingFiles[0] : null;
        }
        private void ProcessXmlFile(string xmlFilePath, string directoryPath, List<string> lstXml)
        {
            try
            {
                XDocument doc = XDocument.Load(xmlFilePath);

                // Dictionary to store doc_ada_id and corresponding doc_type_desc for later renaming
                Dictionary<string, string> idToDescMap = new Dictionary<string, string>();

                // First pass: Find and delete files based on criteria
                foreach (var ada in doc.Root.Elements("Ada"))
                {
                    string docTypeDesc = ada.Element("doc_type_desc")?.Value;
                    string docAdaId = ada.Element("doc_ada_id")?.Value;

                    // Store for later use in renaming phase
                    if (!string.IsNullOrEmpty(docAdaId) && !string.IsNullOrEmpty(docTypeDesc))
                    {
                        idToDescMap[docAdaId] = docTypeDesc;
                    }

                    // Check if doc_type_desc is in lstXml
                    if (!string.IsNullOrEmpty(docTypeDesc) && !string.IsNullOrEmpty(docAdaId) &&
                        lstXml.Any(phrase => docTypeDesc.Contains(phrase)))
                    {
                        // Find and delete file with name matching doc_ada_id (exclude XML files)
                        DeleteFilesByDocId(directoryPath, docAdaId);
                    }
                }

                // Second pass: Rename remaining files
                RenameFiles(directoryPath, idToDescMap);
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails($"Error processing XML file {xmlFilePath}: {ex.Message}");
            }
        }
        private void DeleteFilesByDocId(string directoryPath, string docAdaId)
        {
            try
            {
                // Get all files in the directory
                string[] allFiles = Directory.GetFiles(directoryPath);

                // Find files whose name matches the doc_ada_id (excluding XML files)
                var filesToDelete = allFiles.Where(file =>
                    Path.GetFileNameWithoutExtension(file) == docAdaId &&
                    !string.Equals(Path.GetExtension(file), ".xml", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                foreach (var file in filesToDelete)
                {
                    try
                    {
                        File.Delete(file);
                        UpdateDetails($"Deleted file: {file}");
                    }
                    catch (Exception ex)
                    {
                        SimpleLogger.SimpleLog.Log(ex);
                        UpdateDetails($"Error deleting file {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails($"Error searching for files to delete: {ex.Message}");
            }
        }
        private void RenameFiles(string directoryPath, Dictionary<string, string> idToDescMap)
        {
            try
            {
                // Get all files in the directory
                string[] allFiles = Directory.GetFiles(directoryPath);

                foreach (var file in allFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string extension = Path.GetExtension(file);

                    // Skip XML files
                    if (string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase))
                        continue;

                    // Check if filename matches any doc_ada_id in our map
                    if (idToDescMap.TryGetValue(fileName, out string newName))
                    {
                        try
                        {
                            // Replace invalid characters in the new filename
                            string sanitizedName = SanitizeFileName(newName);
                            string newFilePath = Path.Combine(directoryPath, sanitizedName + extension);

                            // Ensure we don't have naming conflicts
                            newFilePath = GetUniqueFilePath(newFilePath);

                            File.Move(file, newFilePath);
                            UpdateDetails($"Renamed: {file} -> {newFilePath}");
                        }
                        catch (Exception ex)
                        {
                            SimpleLogger.SimpleLog.Log(ex);
                            UpdateDetails($"Error renaming file {file}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                UpdateDetails($"Error during file renaming process: {ex.Message}");
            }
        }
        private string SanitizeFileName(string fileName)
        {
            // Replace invalid characters with underscore
            string invalid = new string(Path.GetInvalidFileNameChars());
            foreach (char c in invalid)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Trim to reasonable length if needed
            if (fileName.Length > 100)
            {
                fileName = fileName.Substring(0, 100);
            }

            return fileName;
        }
        private string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
                return filePath;

            string directory = Path.GetDirectoryName(filePath);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int counter = 1;
            string newFilePath;

            do
            {
                newFilePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{extension}");
                counter++;
            } while (File.Exists(newFilePath));

            return newFilePath;
        }
        private void ExtractZip(bool delXml)
        {
            UpdateDetails("Extracting Zip Files");
            
            string path = txtDir.Text;
            var dirInfo = new DirectoryInfo(path);
            var lallFiles = dirInfo.GetFiles("*.zip", SearchOption.AllDirectories).ToList();

            UpdateDetails("Found " + lallFiles.Count() + " files to extract");
            

            foreach (var zipFilePath in lallFiles)
            {
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(zipFilePath.FullName);
                    string dirName = Path.GetDirectoryName(zipFilePath.FullName);
                    string extractionDirectory = Path.Combine(dirName, filename);

                    if (!Directory.Exists(extractionDirectory))
                    {
                        Directory.CreateDirectory(extractionDirectory);
                    }
                    UpdateDetails(@"extracting zip file : " + filename);
                    

                    ZipFile.ExtractToDirectory(zipFilePath.FullName, extractionDirectory);

                    
                    if (delXml)
                    {
                        var extractedDirInfo = new DirectoryInfo(extractionDirectory);
                        var extractedFiles = dirInfo.GetFiles("*.xml", SearchOption.AllDirectories).ToList();
                        foreach (var file in extractedFiles)
                        {
                            File.Delete(file.FullName);
                        }
                    }
                    File.Delete(zipFilePath.FullName);
                }
                catch (Exception ex)
                {
                    SimpleLogger.SimpleLog.Log(ex);
                    UpdateDetails(@"Failed to extract file : " + zipFilePath.Name + " - " + ex.Message);
                }
            }
        }
        private void moveToErrorDir(DirectoryInfo dir, DirectoryInfo archiveDirInfo)
        {
            try
            {

                string newDirectoryName = dir.Name + "_Error";
                string newDirectoryPath = Path.Combine(dir.Parent.FullName, newDirectoryName);

                DirectoryInfo newDirectory = Directory.CreateDirectory(newDirectoryPath);
                Directory.Delete(dir.FullName);
                CopyAll(archiveDirInfo, newDirectory);
            }
            catch { }
        }
        private string createArchivePath()
        {
            string archive = txtArchive.Text;
            string archiveDirectory = Path.Combine(archive, DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!Directory.Exists(archiveDirectory))
            {
                Directory.CreateDirectory(archiveDirectory);
            }

            return archiveDirectory;
        }
        private DirectoryInfo ArchiveDir(string archiveDirectory, DirectoryInfo dir)
        {
            var archiveDir = Path.Combine(archiveDirectory, dir.Name);
            if (!Directory.Exists(archiveDir))
            {
                Directory.CreateDirectory(archiveDir);
            }
            var archiveDirInfo = new DirectoryInfo(archiveDir);
            CopyAll(dir, archiveDirInfo);
            return archiveDirInfo;
        }
        private void UpdateDetails(string message)
        {
            txtDetails.Text += message + Environment.NewLine;
            txtDetails.SelectionStart = txtDetails.Text.Length;
            txtDetails.ScrollToCaret();
            System.Windows.Forms.Application.DoEvents();
        }
        private void UpdareProgressBar(List<FileInfo> lallFiles, int counter, string file)
        {
            var pct = Convert.ToDouble(Convert.ToDouble(counter) / Convert.ToDouble(lallFiles.Count())) * 100;
            lblPb.Text = Path.GetFileName(file);

            var dVal = pct;
            var val = Convert.ToInt32(dVal);
            if (val > 100)
            {
                val = 100;
            }
            progressBar1.Value = val;
        }

        //public string[] ConvertTiffToJpeg(string fileName)
        //{
        //    var qualityDecimal = txtQuality.Value;
        //    if (qualityDecimal < 0) { qualityDecimal = 0; } else if (qualityDecimal > 100) { qualityDecimal = 100; }

        //    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        //    System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
        //    EncoderParameters qualityEncoderParameters = new EncoderParameters(1);
        //    var quality = Convert.ToInt64(qualityDecimal);

        //    EncoderParameter qualityEncoderParameter = new EncoderParameter(qualityEncoder, quality);
        //    qualityEncoderParameters.Param[0] = qualityEncoderParameter;

        //    using (Image imageFile = Image.FromFile(fileName))
        //    {
        //        int frameNum = imageFile.GetFrameCount(FrameDimension.Page);
        //        FrameDimension frameDimensions = new FrameDimension(imageFile.FrameDimensionsList[0]);
        //        //int frameNum = imageFile.GetFrameCount(frameDimensions);
        //        string[] jpegPaths = new string[frameNum];

        //        for (int frame = 0; frame < frameNum; frame++)
        //        {
        //            imageFile.SelectActiveFrame(frameDimensions, frame);


        //            //    Image[] images = new Image[frameCount];
        //            //    Guid objGuid = tiffImage.FrameDimensionsList[0];
        //            //    FrameDimension objDimension = new FrameDimension(objGuid);
        //            //    for (int i = 0; i < frameCount; i++)
        //            //    {
        //            //        tiffImage.SelectActiveFrame(objDimension, i);
        //            //Byte[] bytes;
        //            //using (var ms = new MemoryStream())
        //            //{
        //            //    imageFile.Save(ms, imageFile.RawFormat, new EncoderParameters() { Param=EncoderParameters.});
        //            //    bytes =  ms.ToArray();
        //            //}

        //            //var path = String.Format("{0}\\{1}_{2}.jpg",
        //            //        Path.GetDirectoryName(fileName),
        //            //        Path.GetFileNameWithoutExtension(fileName),
        //            //        frame);


        //            //AnyBitmap bitmp = new AnyBitmap(bytes);
        //            //bitmp.ExportFile(path, AnyBitmap.ImageFormat.Jpeg, 10);
        //            //jpegPaths[frame] = path;


        //            using (Bitmap bmp = new Bitmap(imageFile))
        //            {
        //                jpegPaths[frame] = String.Format("{0}\\{1}_{2}.jpg",
        //                    Path.GetDirectoryName(fileName),
        //                    Path.GetFileNameWithoutExtension(fileName),
        //                    frame);
        //                bmp.Save(jpegPaths[frame], jpgEncoder, qualityEncoderParameters);
        //            }
        //        }

        //        return jpegPaths;
        //    }
        //}

        //private ImageCodecInfo GetEncoder(ImageFormat format)
        //{
        //    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

        //    foreach (ImageCodecInfo codec in codecs)
        //    {
        //        if (codec.FormatID == format.Guid)
        //        {
        //            return codec;
        //        }
        //    }

        //    return null;
        //}
        //private void MergeFiles(List<IronPdf.PdfDocument> pdfDocuments)
        //{

        //}
        //public static Image[] SplitTIFFImage(Image tiffImage)
        //{
        //    int frameCount = tiffImage.GetFrameCount(FrameDimension.Page);
        //    Image[] images = new Image[frameCount];
        //    Guid objGuid = tiffImage.FrameDimensionsList[0];
        //    FrameDimension objDimension = new FrameDimension(objGuid);
        //    for (int i = 0; i < frameCount; i++)
        //    {
        //        tiffImage.SelectActiveFrame(objDimension, i);
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            tiffImage.Save(ms, ImageFormat.Tiff);
        //            images[i] = Image.FromStream(ms);
        //            //imagesMs[i] = ms;
        //        }
        //    }
        //    return images;
        //}

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
