using System;
using System.Text.RegularExpressions;

namespace BulletDance.Editor
{
    public static class EditorExt
    {
        public static string FormatPropertyName(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if(text.StartsWith("m_")) text = text.Remove(0, 2);
            if(text.StartsWith("_"))  text = text.Remove(0, 1);

            text = Regex.Replace(text, @"([a-z])([A-Z])", "$1 $2"); //Add space between each camel bump
            text = char.ToUpper(text[0]) + text.Substring(1).ToLower(); //Capitalize first letter only
            return text;
        }
    }
}