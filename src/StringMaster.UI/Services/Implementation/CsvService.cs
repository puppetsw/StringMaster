using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StringMaster.UI.Services.Interfaces;

namespace StringMaster.UI.Services.Implementation;

public class CsvService : ICsvService
{
    private const char DOUBLE_QUOTE = '"';
    private const char COMMA = ',';

    /// <summary>
    ///     Parses the specified CSV string into a CsvRecordList.
    /// </summary>
    /// <param name="csvString">The CSV string.</param>
    /// <returns></returns>
    public CsvRecordList Parse(string csvString)
    {
        var records = new CsvRecordList();
        {
            string[] lines = csvString.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                records.Add(ParseLine(line.Trim()));
            }
        }

        return records;
    }

    /// <summary>
    /// Reads the CSV records from specified path at once.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns></returns>
    public CsvRecordList Read(string path)
    {
        return Read(path, Encoding.UTF8);
    }

    /// <summary>
    ///     Reads a single line from the the currently open reader.
    ///     If a reader
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public CsvRecord ReadLine(StreamReader reader)
    {
        string line = null;
        if ((line = reader.ReadLine()) != null) return ParseLine(line);

        return null;
    }

    /// <summary>
    ///     Reads the specified csv file in the given file path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="encoding">The encoding.</param>
    /// <returns></returns>
    public CsvRecordList Read(string path, Encoding encoding)
    {
        string[] csvLines = File.ReadAllLines(path, encoding);
        var csvRecords = new CsvRecordList();

        for (var lineIndex = 0; lineIndex < csvLines.Length; lineIndex++)
        {
            string currentLine = csvLines[lineIndex];
            var record = ParseLine(currentLine);
            csvRecords.Add(record);
        }

        return csvRecords;
    }

    /// <summary>
    ///     Parses the line into a list of strings.
    /// </summary>
    /// <param name="line">The line.</param>
    /// ///
    /// <returns></returns>
    public CsvRecord ParseLine(string line)
    {
        var values = new CsvRecord();
        var currentValue = new StringBuilder(1024);
        var currentState = ReadState.WaitingForNewField;

        for (var charIndex = 0; charIndex < line.Length; charIndex++)
        {
            // Get the current and next character
            char currentChar = line[charIndex];
            char? nextChar = charIndex < line.Length - 1 ? line[charIndex + 1] : new char?();

            // Perform logic based on state and decide on next state
            switch (currentState)
            {
                case ReadState.WaitingForNewField:
                {
                    currentValue.Clear();
                    switch (currentChar)
                    {
                        case DOUBLE_QUOTE:
                            currentState = ReadState.PushingQuoted;
                            continue;
                        case COMMA:
                            values.Add(currentValue.ToString());
                            currentState = ReadState.WaitingForNewField;
                            continue;
                        default:
                            currentValue.Append(currentChar);
                            currentState = ReadState.PushingNormal;
                            continue;
                    }
                }
                case ReadState.PushingNormal:
                {
                    switch (currentChar)
                    {
                        // Handle field content delimiter by comma
                        case COMMA:
                            currentState = ReadState.WaitingForNewField;
                            values.Add(currentValue.ToString().Trim());
                            currentValue.Clear();
                            continue;
                        // Handle double quote escaping
                        case DOUBLE_QUOTE when nextChar == DOUBLE_QUOTE:
                            // advance 1 character now. The loop will advance one more.
                            currentValue.Append(currentChar);
                            charIndex++;
                            continue;
                    }

                    currentValue.Append(currentChar);
                    break;
                }
                case ReadState.PushingQuoted:
                {
                    switch (currentChar)
                    {
                        // Handle field content delimiter by ending double quotes
                        case DOUBLE_QUOTE when nextChar != DOUBLE_QUOTE:
                            currentState = ReadState.PushingNormal;
                            continue;
                        // Handle double quote escaping
                        case DOUBLE_QUOTE when nextChar == DOUBLE_QUOTE:
                            // advance 1 character now. The loop will advance one more.
                            currentValue.Append(currentChar);
                            charIndex++;
                            continue;
                    }

                    currentValue.Append(currentChar);
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // push anything that has not been pushed (flush)
        values.Add(currentValue.ToString().Trim());
        return values;
    }

    /// <summary>
    ///     Defines the 3 different read states
    /// </summary>
    private enum ReadState
    {
        WaitingForNewField,
        PushingNormal,
        PushingQuoted
    }
}


public class CsvRecord : List<string>
{
    public override string ToString()
    {
        return string.Join(",", this.Select(s => $"\"{s.Replace("\"", "\"\"")}\"").ToArray());
    }
}

public class CsvRecordList : List<CsvRecord>
{
}
