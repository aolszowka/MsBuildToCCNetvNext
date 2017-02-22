// <copyright file="Error.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.Build.Framework;

    /// <summary>
    /// An in-memory representation of an error in MSBuild.
    /// </summary>
    public class Error : ErrorOrWarningBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="e">The <see cref="BuildErrorEventArgs"/> to populate this <see cref="Error"/>.</param>
        public Error(BuildErrorEventArgs e)
            : base(e.Code, e.Message, e.File, e.LineNumber, e.ColumnNumber)
        {
        }

        /// <summary>
        /// Gets the XmlFragement for this Error.
        /// </summary>
        public XElement XmlFragement
        {
            get
            {
                var attributes = new List<XAttribute>();

                attributes.Add(new XAttribute("code", this.Code));
                attributes.Add(new XAttribute("message", this.Text));

                // If we're given a file we need to split this up into
                // usable pieces like the original MsBuildToCCNet does
                if (!string.IsNullOrWhiteSpace(this.File))
                {
                    attributes.Add(new XAttribute("dir", Path.GetDirectoryName(this.File) ?? string.Empty));
                    attributes.Add(new XAttribute("name", Path.GetFileName(this.File) ?? string.Empty));
                    attributes.Add(new XAttribute("pos", string.Format("({0}, {1})", this.Line, this.Column)));
                }

                return new XElement("error", attributes);
            }
        }
    }
}
