using System.IO;
using System.Text;
using StringMaster.UI.Services.Implementation;

namespace StringMaster.UI.Services.Interfaces;

public interface ICsvService
{
    /// <summary>
    ///     Parses the specified CSV string into a CsvRecordList.
    /// </summary>
    /// <param name="csvString">The CSV string.</param>
    /// <returns></returns>
    CsvRecordList Parse(string csvString);

    /// <summary>
    /// Reads the CSV records from specified path at once.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    CsvRecordList Read(string path);

    /// <summary>
    ///     Reads the specified csv file in the given file path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns></returns>
    CsvRecordList Read(string path, Encoding encoding);

    /// <summary>
    ///     Reads a single line from the the currently open reader.
    ///     If a reader
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    CsvRecord ReadLine(StreamReader reader);

    /// <summary>
    ///     Parses the line into a list of strings.
    /// </summary>
    /// <param name="line">The line.</param>
    /// ///
    /// <returns></returns>
    CsvRecord ParseLine(string line);
}
