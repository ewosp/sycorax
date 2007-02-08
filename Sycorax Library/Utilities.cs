using System;

namespace Sycorax {
    /// <summary>
    /// Provides utilities to other library components
    /// </summary>
    public static class Utilities {
        /// <summary>
        /// Escape a string, sql way.
        /// </summary>
        /// <param name="toEscape">The string to escape.</param>
        /// <returns>The escaped string</returns>
        public static string SqlEscape (string toEscape) {
            toEscape = toEscape.Replace("\\", "\\\\");
            toEscape = toEscape.Replace("\'", "\\\'");
            toEscape = toEscape.Replace("\"", "\\\"");
            toEscape = toEscape.Replace("`", "\\`");
            toEscape = toEscape.Replace("´", "\\´");
            toEscape = toEscape.Replace("’", "\\’");
            toEscape = toEscape.Replace("‘", "\\‘");
            return toEscape;
        }
    }
}