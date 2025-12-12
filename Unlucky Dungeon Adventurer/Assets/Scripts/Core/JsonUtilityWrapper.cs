using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class JsonUtilityWrapper
{
    /// <summary>
    /// Parse a simple dictionary json: { "key": ["a","b"] }
    /// </summary>
    public static Dictionary<string, List<string>> FromCityNamesJson(string json)
    {
        var dict = new Dictionary<string, List<string>>();
        if (string.IsNullOrWhiteSpace(json))
            return dict;

        var matches = Regex.Matches(json, "\"(?<k>[^\"]+)\"\\s*:\\s*\\[(?<v>[^\\]]*)\\]");
        foreach (Match m in matches)
        {
            string key = m.Groups["k"].Value;
            string arr = m.Groups["v"].Value;
            var values = arr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim().Trim('"'))
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList();
            dict[key] = values;
        }

        return dict;
    }
}
