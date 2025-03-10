using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Assessments.Transformation.TextSanitizer
{
    internal class CsvFileHandler
    {
        /// <summary>
        /// Reads the file and returns a list of lines
        /// </summary>
        /// <param name="basePath">The path to the file, including trailing slash</param>
        /// <param name="file">The name of the file itself</param>
        /// <returns></returns>
        internal static IEnumerable<dynamic> ReadFile(byte[] file)
        {
            try
            {
                var reader = new StreamReader(new MemoryStream(file), System.Text.Encoding.UTF8); // .GetEncoding("iso-8859-1"));
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";"
                };
                var csvReader = new CsvReader(reader, config);
                

                var records = csvReader.GetRecords<dynamic>().ToList();

                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nUnable to read file. Exception: {e}");
                return null;
            }
        }

        /// <summary>
        /// Write the provided list of lines to a csv file
        /// </summary>
        /// <param name="basePath">The path to the file, including trailing slash</param>
        /// <param name="fileName">The name of the file itself</param>
        /// <param name="lines">The list of lines to be written</param>
        /// <returns></returns>
        internal static byte[] WriteFile(IEnumerable<dynamic> lines)
        {
            try
            {
                var writer = new MemoryStream();
                var stream = new StreamWriter(writer, Encoding.UTF8);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    //Quote = '"',
                    //ShouldQuote = args =>
                    //{
                    //    if (!Regex.IsMatch(args.Field, @"^\d+$")) return true;
                    //    return false;
                    //}
                };
                var csvWriter = new CsvWriter(stream, config);

                csvWriter.WriteRecords(lines);
                stream.Flush();
                writer.Position = 0;
                var bytes = writer.ToArray();
                writer.Close();
                return bytes;
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }
    }
}
