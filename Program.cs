using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using iText.Html2pdf;
class Program
{
    static void Main(string[] args)
    {
        // Paths to template, JSON file, and output directory
        string htmlTemplate = "data/broker_template.html";
        string jsonFilePath = "data/broker_data.json";
        string outputDirectory = "data/output";

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        // Load JSON data
        var brokers = LoadBrokerJson(jsonFilePath);
        if (brokers is not null)
        {
            // Process each broker
            foreach (var broker in brokers)
            {
                GeneratePdfDocs(htmlTemplate, outputDirectory, broker);
            }
            Console.WriteLine("Pdf Docs generated successfully!");
        }

    }

    static void GeneratePdfDocs(string htmlTemplate, string outputDirectory, BrokerInformation broker)
    {
        // Pdf File Name
        string newPdfFile = outputDirectory + $"/{broker.BrokerName}.pdf";
        // Create new copy of the template
        string newFileName = outputDirectory + $"/{broker.BrokerName}.html";
        // Create a document.
        try
        {
            // Read the complete file and replace the text
            using (StreamReader reader = new StreamReader(htmlTemplate))
            {
                string content = reader.ReadToEnd();
                content = Regex.Replace(content, "#BrokerName", broker.BrokerName ?? "");
                content = Regex.Replace(content, "#TINNumber", broker.TINNumber ?? "");
                content = Regex.Replace(content, "#NPNNumber", broker.NPNNumber ?? "");
                content = Regex.Replace(content, "#BrokerContactName", broker.BrokerContactName ?? "");
                content = Regex.Replace(content, "#BrokerAddress", broker.BrokerAddress ?? "");
                content = Regex.Replace(content, "#BrokerCity", broker.BrokerCity ?? "");
                content = Regex.Replace(content, "#BrokerState", broker.BrokerState ?? "");
                content = Regex.Replace(content, "#BrokerZip", broker.BrokerZip ?? "");
                
                // Write the content back to the file
                using (StreamWriter writer = new StreamWriter(newFileName))
                {
                    writer.Write(content);
                    writer.Close();
                }
            }

            ConvertHtmlToPdf(newFileName,newPdfFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }


    static List<BrokerInformation>? LoadBrokerJson(string path)
    {
        string jsonContent = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<BrokerInformation>>(jsonContent);
    }


    // BrokerInformation Class
    public class BrokerInformation
    {
        public string? BrokerName { get; set; }
        public string? TINNumber { get; set; }
        public string? NPNNumber { get; set; }
        public string? BrokerContactName { get; set; }
        public string? BrokerAddress { get; set; }
        public string? BrokerCity { get; set; }
        public string? BrokerState { get; set; }
        public string? BrokerZip { get; set; }
    }

    static void ConvertHtmlToPdf(string htmlFile, string outputPath)
    {
        using (FileStream htmlSource = File.Open(htmlFile, FileMode.Open))
        using (FileStream pdfDest = File.Open(outputPath, FileMode.Create))
        {
            ConverterProperties converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(htmlSource, pdfDest, converterProperties);
        }
    }
}