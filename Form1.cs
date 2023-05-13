using IronPdf;
using Microsoft.Office.Interop.Word;
using SimpleLogger;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace PdfConverter
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public partial class Form1 : Form
    {
        //private List<string> ConverterErrorFiles = new List<string>();
        //private List<DirectoryInfo> GoodDirs = new List<DirectoryInfo>();
        //private List<string> MergerErrorFiles = new List<string>();
        WebBrowser myWebBrowser = new WebBrowser();
        
        public Form1()
        {
            InitializeComponent();


            IronPdf.Logging.Logger.EnableDebugging = true;
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
            var counter = 0;
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
                
                convertOK = true;
                foreach (var file in files)
                {
                    try
                    {
                        counter++;
                        var pct = Convert.ToDouble(Convert.ToDouble(counter) / Convert.ToDouble(lallFiles.Count())) * 100;
                        lblPb.Text = Path.GetFileName(file);

                        var dVal = pct;
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
                            var paths = ConvertTiffToJpeg(file);
                            using (var converted = ImageToPdfConverter.ImageToPdf(paths, IronPdf.Imaging.ImageBehavior.FitToPage))
                            {
                               // converted.CompressImages(10);
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
                        try
                        {
                            Directory.Delete(dir.FullName);
                            Directory.CreateDirectory(dir.FullName);
                            CopyAll(archiveDirInfo, dir);
                        }
                        catch { }

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
                string lfileName = string.Empty;
                try
                {
                    foreach (var lfile in lFiles)
                    {
                        lfileName = lfile;
                        pdfDocuments.Add(IronPdf.PdfDocument.FromFile(lfile));
                    }
                }
                catch(IronPdf.Exceptions.IronPdfNativeException)
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

        public string[] ConvertTiffToJpeg(string fileName)
        {
            var qualityDecimal = txtQuality.Value;
            if (qualityDecimal < 0) { qualityDecimal = 0; } else if (qualityDecimal > 100) { qualityDecimal = 100; }

            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder qualityEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters qualityEncoderParameters = new EncoderParameters(1);
            var quality = Convert.ToInt64(qualityDecimal);
            
            EncoderParameter qualityEncoderParameter = new EncoderParameter(qualityEncoder, quality);
            qualityEncoderParameters.Param[0] = qualityEncoderParameter;

            using (Image imageFile = Image.FromFile(fileName))
            {
                int frameNum = imageFile.GetFrameCount(FrameDimension.Page);
                FrameDimension frameDimensions = new FrameDimension(imageFile.FrameDimensionsList[0]);
                //int frameNum = imageFile.GetFrameCount(frameDimensions);
                string[] jpegPaths = new string[frameNum];
                
                for (int frame = 0; frame < frameNum; frame++)
                {
                    imageFile.SelectActiveFrame(frameDimensions, frame);

                    
                    //    Image[] images = new Image[frameCount];
                    //    Guid objGuid = tiffImage.FrameDimensionsList[0];
                    //    FrameDimension objDimension = new FrameDimension(objGuid);
                    //    for (int i = 0; i < frameCount; i++)
                    //    {
                    //        tiffImage.SelectActiveFrame(objDimension, i);
                    //Byte[] bytes;
                    //using (var ms = new MemoryStream())
                    //{
                    //    imageFile.Save(ms, imageFile.RawFormat, new EncoderParameters() { Param=EncoderParameters.});
                    //    bytes =  ms.ToArray();
                    //}

                    //var path = String.Format("{0}\\{1}_{2}.jpg",
                    //        Path.GetDirectoryName(fileName),
                    //        Path.GetFileNameWithoutExtension(fileName),
                    //        frame);


                    //AnyBitmap bitmp = new AnyBitmap(bytes);
                    //bitmp.ExportFile(path, AnyBitmap.ImageFormat.Jpeg, 10);
                    //jpegPaths[frame] = path;


                    using (Bitmap bmp = new Bitmap(imageFile))
                    {
                        jpegPaths[frame] = String.Format("{0}\\{1}_{2}.jpg",
                            Path.GetDirectoryName(fileName),
                            Path.GetFileNameWithoutExtension(fileName),
                            frame);
                        bmp.Save(jpegPaths[frame], jpgEncoder, qualityEncoderParameters);
                    }
                }

                return jpegPaths;
            }
        }
        
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
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

        private void btnDel_Click(object sender, EventArgs e)
        {
            var xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            Excel.Range xlRange = null;
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
            try
            {
                txtDetails.Text += "Open Excel File" + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();
                xlWorkbook = xlApp.Workbooks.Open(excelPath);
                xlApp.Visible = false;
                xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[1];
                var lastRow = xlWorksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing).Row;
                for (int i = 2; i <= lastRow; i++)
                {
                    var gid = xlWorksheet.Range["A" + i, "A" + i].Value2.ToString();
                    lst.Add(gid);
                }
            }
            catch(Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                MessageBox.Show("נכשל בקריאת נתונים מאקסל");
                return;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //rule of thumb for releasing com objects:
                //  never use two dots, all COM objects must be referenced and released individually
                //  ex: [somthing].[something].[something] is bad

                //release com objects to fully kill excel process from running in the background
                if (xlRange != null)
                    Marshal.ReleaseComObject(xlRange);
                if (xlWorksheet != null)
                    Marshal.ReleaseComObject(xlWorksheet);

                //close and release
                if (xlWorksheet != null)
                {
                    xlWorkbook.Close();
                    Marshal.ReleaseComObject(xlWorkbook);
                }

                if (xlApp != null)
                {
                    //quit and release
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);
                }
            }
            string fileName = string.Empty;
            try
            {
                txtDetails.Text += "Found " + lallFiles.Count() + " files to parse" + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();
                foreach (var file in lallFiles)
                {
                    fileName = Path.GetFileNameWithoutExtension(file.FullName);
                    foreach (var phrase in lst)
                    {

                        if (fileName.StartsWith(phrase))
                        {
                            txtDetails.Text += @"Deleting file : " + fileName + Environment.NewLine;
                            txtDetails.SelectionStart = txtDetails.Text.Length;
                            txtDetails.ScrollToCaret();
                            System.Windows.Forms.Application.DoEvents();
                            File.Delete(file.FullName);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                SimpleLogger.SimpleLog.Log(ex);
                txtDetails.Text += @"Failed to delete file : " + fileName + Environment.NewLine;
                txtDetails.SelectionStart = txtDetails.Text.Length;
                txtDetails.ScrollToCaret();
                System.Windows.Forms.Application.DoEvents();
            }

            MessageBox.Show("הקבצים נמחקו בהצלחה");

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
