# PdfConverter - AI Development Guide

## Project Overview

**PdfConverter** is a Windows Forms desktop application designed for batch converting multiple document formats into PDF files, merging them by directory, and managing archive operations. This is an enterprise-level document processing tool with sophisticated file handling, logging, and archive management capabilities.

### Key Information
- **Type**: Windows Forms Application (.NET 8.0)
- **Language**: C# with Hebrew UI/messaging
- **Version**: 2.0.3.1
- **Platform**: Windows 7.0+
- **Architecture**: Desktop application with COM interop for Office automation

---

## Project Structure

```
PdfConverter/
├── Properties/                    # Project properties and resources
│   ├── AssemblyInfo.cs           # Assembly metadata
│   ├── Resources.Designer.cs     # Resource file designer
│   ├── Resources.resx            # Resource definitions
│   ├── Settings.Designer.cs      # Application settings designer
│   └── Settings.settings         # Saved user settings (paths)
├── Form1.cs                      # Main application logic (1,300+ lines)
├── Form1.Designer.cs             # UI layout definition
├── Form1.resx                    # Form resources
├── Program.cs                    # Application entry point
├── SimpleLogger.cs               # Custom logging utility (MIT licensed)
├── PrinterClass.cs               # Windows printer interop (currently unused)
├── App.config                    # Application configuration
├── PdfConverter.csproj           # Project file
└── .gitignore                    # Git ignore rules
```

---

## Technology Stack

### Core Framework
- **.NET 8.0** - Modern .NET with Windows compatibility
- **Windows Forms** - Desktop UI framework
- **C# 11+** - Primary language

### PDF & Document Processing
1. **IronPDF 2024.3.4** - Primary PDF library
   - PDF merging capability
   - ChromePdfRenderer for HTML→PDF conversion
   - Image to PDF conversion

2. **Spire.PDF 8.1.4** - Secondary PDF processing library
   - Licensed with activation code
   - Backup/parallel processing option

### Microsoft Office Interop
- **Microsoft.Office.Interop.Word** (v8.6) - Word document conversion
- **Microsoft.Office.Core** (v2.7) - Office core functionality

### Data & Utility Libraries
- **ClosedXML 0.104.2** - Excel file reading for deletion rules
- **SixLabors.ImageSharp 3.1.7** - Image manipulation
- **System.Text.Json 8.0.5** - JSON processing
- **System.Data.SqlClient 4.8.6** - SQL database support
- **System.IO.Compression** - ZIP archive handling

---

## Key Components

### 1. Form1.cs - Main Application Logic

The heart of the application, containing all core processing logic:

#### Core Methods

| Method | Purpose | Location |
|--------|---------|----------|
| `ConvertFiles()` | Main orchestration - scans directories, processes files, handles merging | Main workflow |
| `ProcessJpg()` | Converts JPEG images to PDF | Line ~393 |
| `ProcessTiff()` | Converts TIFF images to PDF using compression | Image processing |
| `ProcessHtml()` | Converts HTML files to PDF using Chrome renderer | HTML conversion |
| `ConvertWord()` | Converts DOC/DOCX to PDF via Office Interop | Word processing |
| `MergeSingleDir()` | Merges all PDFs in a directory into single document | PDF merging |
| `MoveToPdfsDirectory()` | Moves converted PDFs, manages cleanup | File management |
| `DeleteDirs()` | Deletes directories based on Excel criteria | Cleanup |
| `ExtractZip()` | Extracts ZIP archives | Archive handling |
| `ArchiveDir()` | Creates archive directory structure | Archiving |
| `IsValidDirectory()` | Validates directory names (18-character check) | Validation |

### 2. Program.cs - Application Entry Point

- Initializes Windows Forms application
- Sets up IronPDF and Spire.PDF licenses
- Launches Form1 (main window)

### 3. SimpleLogger.cs - Logging Framework

MIT-licensed custom logger providing:
- File-based logging to `.\Log\` directory
- Thread-safe operation
- Severity levels: Info, Warning, Error, Exception
- Automatic file rotation
- Rich detail capture

### 4. App.config - Application Configuration

Key settings:
- `ExcelForDelete`: Path to Excel file (Del_strings.xlsx) for deletion rules
- `DirPath`: User-selected working directory (persisted)
- `ArchivePath`: User-selected archive destination (persisted)

---

## Application Workflow

### Typical Processing Flow

1. **User Input**
   - User selects source directory (contains subdirectories with documents)
   - User selects archive destination directory

2. **File Processing Pipeline**
   ```
   Scan Subdirectories
   → Detect File Types (.jpg, .tiff, .html, .doc, .docx)
   → Convert to PDF (using appropriate converter)
   → Track Success/Failure
   ```

3. **Post-Conversion Operations**
   ```
   Merge PDFs (all PDFs in directory → single PDF)
   → Move/Archive Results
   → Clean up source files (optional)
   → Delete based on Excel rules
   ```

4. **Output**
   - Merged PDF per source subdirectory
   - Archive copies
   - Detailed logs to `.\Log\` directory

5. **Error Handling**
   - Moves files with errors to error directory
   - Logs all exceptions with file names
   - Progress tracking with UI updates

---

## Features

- **Multi-format Support**: JPG, TIFF, HTML, DOC, DOCX
- **Batch Processing**: Handles directory trees automatically
- **PDF Merging**: Combines multiple PDFs into single document per folder
- **Archive Management**: ZIP extraction, directory archiving
- **Intelligent Deletion**: Deletes files/directories based on Excel criteria
- **Comprehensive Logging**: Thread-safe file-based logging
- **Hebrew Language Support**: UI and encoding support
- **License Management**: Licensed enterprise PDF libraries
- **Progress Tracking**: Real-time progress bar and detail updates

---

## Development Guidelines

### When Working with This Codebase

1. **File Paths**: Always use absolute paths for file operations
2. **Error Handling**: Wrap file operations in try-catch blocks; use SimpleLogger
3. **Thread Safety**: Logger is thread-safe; UI updates must use Invoke/BeginInvoke
4. **Settings Persistence**: Use `Properties.Settings.Default` for user preferences
5. **PDF Libraries**: Primary is IronPDF; Spire.PDF is backup
6. **Encoding**: Hebrew text requires proper encoding (already configured)

### Common Tasks

#### Adding a New File Format Converter

1. Create a new `Process{Format}()` method in Form1.cs
2. Add file type detection in `ConvertFiles()`
3. Handle errors and log with SimpleLogger
4. Update progress bar and detail text
5. Test with sample files

#### Modifying the Merge Logic

- Core merging logic is in `MergeSingleDir()`
- Uses IronPDF's `PdfDocument.Merge()` method
- Handles page fitting and compression

#### Adding New Configuration Settings

1. Add to `App.config` in `<appSettings>` section
2. Access via `ConfigurationManager.AppSettings["YourKey"]`
3. For user settings, add to `Properties/Settings.settings`

---

## Building and Running

### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- Windows OS (7 or later)
- Microsoft Office (for Word conversion)
- Valid IronPDF and Spire.PDF licenses

### Build Configuration

- **Platforms**: AnyCPU, x64 (prefer x64 for production)
- **Target**: .NET 8.0-windows7.0
- **Output**: Windows executable (WinExe)

### License Setup

Update in `Program.cs`:
```csharp
IronPdf.License.LicenseKey = "YOUR_IRONPDF_LICENSE";
Spire.Pdf.License.LicenseProvider.SetLicenseKey("YOUR_SPIRE_LICENSE");
```

---

## Recent Changes (Git History)

- **v2.0.3.1**: Simplified UI (removed Cancel, Delete, Zip buttons)
- **v2.0.2**: Added TIFF compression, FitToPage HTML rendering
- **v2.0.1**: Upgraded to IronPDF, .NET 7, updated licensing
- **Earlier**: Migrated from .NET Framework 4.8 to .NET 8.0

---

## Debugging Tips

### Common Issues

1. **PDF Conversion Fails**
   - Check license keys are valid
   - Verify input file is not corrupted
   - Check logs in `.\Log\` directory

2. **Word Conversion Issues**
   - Ensure Microsoft Office is installed
   - Check COM interop is enabled
   - Verify Word application can be instantiated

3. **Directory Processing Errors**
   - Validate directory name format (18 characters expected)
   - Check file permissions
   - Review Excel deletion rules file

### Logging

All operations are logged to `.\Log\` directory:
- Info messages: General operation flow
- Warning messages: Non-critical issues
- Error messages: Failed operations
- Exception messages: Detailed stack traces

---

## Testing Strategy

### Manual Testing Checklist

- [ ] JPG to PDF conversion
- [ ] TIFF to PDF conversion (with compression)
- [ ] HTML to PDF conversion (Chrome renderer)
- [ ] DOC/DOCX to PDF conversion
- [ ] Multi-file merging per directory
- [ ] Archive directory creation
- [ ] ZIP extraction
- [ ] Excel-based deletion rules
- [ ] Progress bar updates
- [ ] Error handling and logging
- [ ] Settings persistence

### Test Data Location

Place test files in subdirectories under your selected source directory:
```
SourceDir/
├── Project001/
│   ├── document.docx
│   ├── image.jpg
│   └── page.html
├── Project002/
│   └── archive.zip
```

---

## Future Enhancements (Potential)

- Support for additional file formats (PowerPoint, Excel)
- Parallel processing for faster batch operations
- Cloud storage integration
- OCR support for scanned documents
- Email notifications on completion
- Web-based monitoring interface
- Configuration UI for Excel deletion rules

---

## Support and Troubleshooting

### Log Files
- **Location**: `.\Log\` directory
- **Format**: Date-stamped text files
- **Contents**: All operations, errors, and exceptions

### Configuration Files
- **App.config**: Application settings and assembly bindings
- **Properties/Settings.settings**: User preferences (paths)
- **Del_strings.xlsx**: Directory deletion rules

### Key Dependencies
- All NuGet packages must be restored before building
- Office Interop requires Microsoft Office installation
- PDF libraries require valid license keys

---

## License Information

### Application
- Proprietary application code
- Licensed PDF libraries (IronPDF, Spire.PDF)

### Third-Party Components
- **SimpleLogger.cs**: MIT License
- Other libraries: See individual NuGet package licenses

---

## Contact and Contribution

This is a production application. When making changes:
1. Create feature branches with `claude/` prefix
2. Test thoroughly with sample documents
3. Update this documentation as needed
4. Commit with clear, descriptive messages
5. Push to designated branches only

---

*Last Updated: 2025-11-14*
*Documentation Version: 1.0*
