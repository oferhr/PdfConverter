# PdfConverter

A Windows Forms desktop application for batch converting multiple document formats to PDF with automatic merging and archive management.

## Overview

**PdfConverter** is an enterprise-level document processing tool designed for high-volume batch conversion operations. It scans directory structures, converts documents to PDF format, merges PDFs by directory, and manages archival operations.

## Features

- **Multi-Format Support**: Convert JPG, TIFF, HTML, DOC, and DOCX files to PDF
- **Batch Processing**: Automatically processes all subdirectories in the selected path
- **PDF Merging**: Combines all PDFs in each directory into a single consolidated document
- **Archive Management**: ZIP extraction and directory archiving capabilities
- **Smart Deletion**: Excel-based deletion rules for automated cleanup
- **Comprehensive Logging**: Detailed operation logs with error tracking
- **Hebrew Support**: UI and text encoding optimized for Hebrew language
- **Progress Tracking**: Real-time progress bar and detailed status updates

## Technology Stack

- **.NET 8.0** - Modern .NET framework
- **Windows Forms** - Desktop UI framework
- **IronPDF 2024.3.4** - Primary PDF processing library
- **Spire.PDF 8.1.4** - Secondary PDF library
- **Microsoft Office Interop** - Word document conversion
- **ClosedXML** - Excel file reading
- **SixLabors.ImageSharp** - Image processing

## Requirements

- Windows 7 or later
- .NET 8.0 Runtime
- Microsoft Office (for Word document conversion)
- Valid IronPDF and Spire.PDF licenses

## Installation

1. Clone the repository
2. Open `PdfConverter.sln` in Visual Studio 2022 or later
3. Restore NuGet packages
4. Update license keys in `Program.cs`
5. Build the solution (prefer x64 configuration)
6. Run the application

## Usage

1. **Select Source Directory**: Click Browse to select the directory containing subdirectories with documents to convert
2. **Select Archive Directory**: Choose the destination for archived/merged PDFs
3. **Click Start**: Begin the conversion process
4. **Monitor Progress**: Watch the progress bar and detailed status messages
5. **Review Logs**: Check the `.\Log` directory for detailed operation logs

### Workflow

```
Source Directory
├── Project001/
│   ├── document.docx  ──┐
│   ├── image.jpg       ─┼──> Convert to PDF ──> Merge ──> Project001.pdf
│   └── page.html       ─┘
├── Project002/
│   └── files...        ───> Convert to PDF ──> Merge ──> Project002.pdf
```

## Supported File Formats

| Format | Extension | Converter |
|--------|-----------|-----------|
| JPEG Images | .jpg, .jpeg | Spire.PDF |
| TIFF Images | .tiff, .tif | IronPDF ImageToPdfConverter |
| HTML Pages | .html, .htm | IronPDF ChromePdfRenderer |
| Word Documents | .doc, .docx | Office Interop |
| ZIP Archives | .zip | System.IO.Compression |

## Configuration

### App.config

- `ExcelForDelete`: Path to Excel file containing deletion rules
- `DirPath`: User-selected source directory (auto-saved)
- `ArchivePath`: User-selected archive directory (auto-saved)

### Settings

User preferences are automatically saved and restored between sessions.

## Logging

All operations are logged to the `.\Log` directory with:
- Operation timestamps
- File-level processing details
- Error messages with stack traces
- Success/failure status

## Documentation

For detailed development documentation, see [CLAUDE.md](CLAUDE.md).

## Version History

- **v2.0.3.1** (Current)
  - Simplified UI (consolidated process buttons)
  - Improved error handling

- **v2.0.2**
  - Added TIFF compression
  - FitToPage HTML rendering
  - Chrome renderer integration

- **v2.0.1**
  - Upgraded to IronPDF
  - Migrated to .NET 7 (later .NET 8)
  - Updated licensing

## License

This application uses licensed PDF libraries:
- IronPDF (Commercial License - expires Feb 25, 2028)
- Spire.PDF (Commercial License)
- SimpleLogger (MIT License)

## Support

For issues, errors, or questions:
1. Check the log files in `.\Log` directory
2. Review the [CLAUDE.md](CLAUDE.md) documentation
3. Verify all prerequisites are installed
4. Ensure license keys are valid and current

## Architecture

This is a single-window Windows Forms application with the following structure:

- **Program.cs**: Application entry point and license initialization
- **Form1.cs**: Main UI and processing logic (~1,300 lines)
- **SimpleLogger.cs**: Thread-safe logging framework (MIT licensed)
- **App.config**: Application configuration and settings

For detailed architecture documentation, see [CLAUDE.md](CLAUDE.md).
