using SharpIpp;
using SharpIpp.Models;
using SharpIpp.Protocol.Models;

if (args.Length < 4)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(
        "Usage: PrintPDFToIPP <printer_ip> <color_mode> <print_mode> <pdf_directory_or_files>");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("  printer_ip: IP address of the printer, and port number if needed");
    Console.WriteLine("  color_mode: monochrome, color or auto");
    Console.WriteLine(
        "  print_mode: all (print all PDFs in the directory) or named (print specific PDF files)");
    Console.WriteLine("  pdf_directory_or_files: Path to directory or list of PDF files");
    Console.ResetColor();
    Console.WriteLine();
    return;
}

var printerIp = args[0];
var colorMode = args[1].ToLower();
var printMode = args[2].ToLower();
var target = args[3];
string[] pdfFiles;

if (!Directory.Exists(target) && !File.Exists(target))
{
    Console.WriteLine($"Path not found: {target}");
    return;
}

if (colorMode != "monochrome" && colorMode != "color" && colorMode != "auto")
{
    Console.WriteLine($"Invalid color mode: {colorMode}. Valid options are color, monochrome and auto");
    return;
}

switch (printMode)
{
    case "all" when Directory.Exists(target):
        pdfFiles = Directory.GetFiles(target, "*.pdf");
        break;
    case "named" when File.Exists(target):
        pdfFiles = args.Skip(3).ToArray();
        break;
    default:
        Console.WriteLine("Invalid print mode / target combination.");
        Console.WriteLine($"Print mode {printMode} with target: {target}");
        Console.WriteLine(
            "  print_mode: all (print all PDFs in the directory) or named (print specific PDF files)");
        return;
}

if (pdfFiles.Length == 0)
{
    Console.WriteLine($"No PDF files found in {target}");
    return;
}

Console.WriteLine($"Printing {pdfFiles.Length} PDF files to {printerIp} in {colorMode} mode");

foreach (var pdfFile in pdfFiles) await PrintPdfToIpp(printerIp, pdfFile, colorMode);
return;

static async Task PrintPdfToIpp(string printerIp, string pdfFile, string colorMode)
{
    try
    {
        SharpIppClient client = new();

        await using var fileStream = File.OpenRead(pdfFile);

        PrintJobRequest printJobRequest =
            new()
            {
                PrinterUri = new Uri($"ipp://{printerIp}/ipp/print"),
                Document = fileStream,
                RequestingUserName = Environment.UserName,
                NewJobAttributes = new NewJobAttributes
                {
                    JobName = Path.GetFileName(pdfFile),
                    AdditionalJobAttributes = new List<IppAttribute>
                    {
                        new(Tag.Keyword, "print-color-mode", colorMode)
                    }
                },
                DocumentAttributes = new DocumentAttributes
                {
                    DocumentFormat = "application/pdf",
                    DocumentName = Path.GetFileName(pdfFile),
                    DocumentNaturalLanguage = "no"
                }
            };

        var response = await client.PrintJobAsync(printJobRequest);

        Console.WriteLine(response.StatusCode == IppStatusCode.SuccessfulOk
            ? $"Successfully printed {pdfFile} to {printerIp}"
            : $"Error printing {pdfFile} to {printerIp}. Status: {response.StatusCode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(
            $"An error occurred while trying to print {pdfFile} to {printerIp}: {ex.Message}"
        );
    }
}