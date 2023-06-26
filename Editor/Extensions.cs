namespace Ambratolm.ScriptGenerator.Extensions
{
    /// <summary>
    /// String extension methods.
    /// </summary>
    internal static class String
    {
        /// <summary>
        /// Returns a substring between two specified strings.
        /// </summary>
        /// <param name="inputString">The string to search in.</param>
        /// <param name="startString">The string to start from.</param>
        /// <param name="endString">The string to end at.</param>
        /// <returns>
        /// A substring between startString and endString, or an empty string if either of them is
        /// not found.
        /// </returns>
        public static string SubstringBetween(this string inputString, string startString, string endString)
        {
            int startIndex = inputString.IndexOf(startString);
            if (startIndex < 0) return "";
            startIndex += startString.Length;
            int endIndex = inputString.IndexOf(endString, startIndex);
            if (endIndex < 0) return "";
            return inputString[startIndex..endIndex];
        }
    }
}