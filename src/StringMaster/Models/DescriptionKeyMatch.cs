using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace StringMaster.Models;

/// <summary>
/// DescriptionKeyMatch class
/// </summary>
public sealed class DescriptionKeyMatch
{
    public DescriptionKey DescriptionKey { get; }

    public Dictionary<string, List<SurveyPoint>> SurveyPoints { get; }

    public DescriptionKeyMatch(DescriptionKey descriptionKey)
    {
        DescriptionKey = descriptionKey;
        SurveyPoints = new Dictionary<string, List<SurveyPoint>>();
    }

    /// <summary>
    /// Builds the Regex match pattern for each <see cref="DescriptionKey"/>.
    /// </summary>
    /// <param name="descriptionKey">The description key.</param>
    /// <param name="matchSpecial"></param>
    /// <remarks>This pattern should detect when there is a whitespace between the code and the number.</remarks>
    /// <returns>A string containing the regex pattern using the given <see cref="DescriptionKey"/>.</returns>
    private static string BuildPattern(DescriptionKey descriptionKey, bool matchSpecial = false)
    {
        if (matchSpecial)
        {
            return "^(" + descriptionKey.Key.Replace("#", ")(\\s?\\d\\d?\\d?)(\\.\\w\\w?\\w?\\w?)?").Replace("*", ".*?");
        }

        return "^(" + descriptionKey.Key.Replace("#", ")(\\s?\\d\\d?\\d?)").Replace("*", ".*?");
    }

    /// <summary>
    /// Gets the description from the CogoPoint's RawDescription
    /// </summary>
    /// <param name="rawDescription"></param>
    /// <param name="descriptionKey"></param>
    /// <returns></returns>
    public static string Description(string rawDescription, DescriptionKey descriptionKey)
    {
        Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey));
        if (!regMatch.Success)
        {
            return string.Empty;
        }

        var match = regMatch.Groups[1].Value;
        return match;
    }

    /// <summary>
    /// Gets the line number from the CogoPoint's RawDescription
    /// </summary>
    /// <param name="rawDescription"></param>
    /// <param name="descriptionKey"></param>
    /// <returns></returns>
    public static string LineNumber(string rawDescription, DescriptionKey descriptionKey)
    {
        Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey));
        if (!regMatch.Success)
        {
            return string.Empty;
        }

        var match = regMatch.Groups[2].Value;
        return match;
    }

    /// <summary>
    /// Gets the special code if one exists in the RawDescription.
    /// </summary>
    /// <param name="rawDescription"></param>
    /// <param name="descriptionKey"></param>
    /// <returns></returns>
    public static string SpecialCode(string rawDescription, DescriptionKey descriptionKey)
    {
        Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey, true));
        if (!regMatch.Success)
        {
            return string.Empty;
        }

        var match = regMatch.Groups[3].Value;
        return match;
    }

    /// <summary>
    /// Returns true if the CogoPoint's RawDescription is a match to the <see cref="DescriptionKey"/>.
    /// </summary>
    /// <param name="rawDescription">The raw description from the CogoPoint.</param>
    /// <param name="descriptionKey">The <see cref="DescriptionKey"/> to match against.</param>
    /// <param name="logger">Optional logger.</param>
    /// <returns></returns>
    public static bool IsMatch(string rawDescription, DescriptionKey descriptionKey)
    {
        var matchPattern = BuildPattern(descriptionKey, true);
        Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), matchPattern);

        if (regMatch.Success)
        {
            Console.WriteLine($"KEY MATCH! Pattern={matchPattern}, RawDes={rawDescription}");
        }

        return regMatch.Success;
    }

    /// <summary>
    /// Adds the <paramref name="civilPoint"/> to the <see cref="SurveyPoints"/>
    /// </summary>
    /// <param name="cogoPoint"></param>
    /// <param name="lineNumber"></param>
    /// <param name="specialCode"></param>
    /// <remarks>
    /// Checks if the <see cref="SurveyPoints"/> contains the current line number
    /// for the point. If it does, add the current point to that dictionary using the key
    /// else, create a new list of points and add it using the key.
    /// </remarks>
    public void AddCogoPoint(Point cogoPoint, string lineNumber, string specialCode)
    {
        var joinablePoint = new SurveyPoint(cogoPoint, specialCode);

        if (SurveyPoints.ContainsKey(lineNumber))
        {
            SurveyPoints[lineNumber].Add(joinablePoint);
            // Console.WriteLine($"Adding to existing list {joinablePoint.CogoPoint.RawDescription}");
        }
        else
        {
            var cogoPoints = new List<SurveyPoint> { joinablePoint };
            SurveyPoints.Add(lineNumber, cogoPoints);
            // Console.WriteLine($"Creating new list {joinablePoint.CogoPoint.RawDescription}");
        }
    }
}
