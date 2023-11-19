# PrintPDFtoIPP

Simple command line tool to send PDFs to IPP enabled printers.

```bash
PrintPDFToIPP <printer_ip> <colormode> <print_mode> <pdf_directory_or_files>
```

## Parameters

- **printer_ip**: IP address of the printer, and port number if needed.
  
- **colormode**: monochrome, color, or auto.
  
- **print_mode**: all (print all PDFs in the directory) or named (print specific PDF files).
  
- **pdf_directory_or_files**: Provide the path to a directory or a list of PDF files.
  - *Example*: `/path/to/pdf/directory` or `/path/to/file1.pdf /path/to/file2.pdf`
