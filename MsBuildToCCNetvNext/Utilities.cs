/// <copyright file="Utilities.cs" company="Ace Olszowka">
/// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
/// </copyright>

namespace MsBuildToCCNetvNext
{
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// Utility class for MsBuildToCCNetvNext
    /// </summary>
    internal class Utilities
    {
        /// <summary>
        /// Given a string determine if we need to sanitize it before serializing it to Xml.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <returns><c>true</c> if the message needs to be sanitized; otherwise <c>false</c>.</returns>
        internal static bool MessageNeedsSanitation(string input)
        {
            bool result =
                input.Any(currentChar => !XmlConvert.IsXmlChar(currentChar));
            return result;
        }

        /// <summary>
        /// Given a string, sanitize it for Xml Serialization and append a message indicating sanitation happened if it was required.
        /// </summary>
        /// <param name="input">The message to sanitize.</param>
        /// <returns>A string that is safe for Xml Serialization.</returns>
        internal static string SanitizeMessageForXml(string input)
        {
            string sanitizedMsg = input;

            if (MessageNeedsSanitation(input))
            {
                string invalidXmlCharactersRemoved =
                            new string(input.AsEnumerable().Where(currentChar => XmlConvert.IsXmlChar(currentChar)).ToArray());
                sanitizedMsg =
                    string.Format("WARNING This message contained invalid XML character(s) which have been removed: {0}", invalidXmlCharactersRemoved);
            }

            return sanitizedMsg;
        }
    }
}
