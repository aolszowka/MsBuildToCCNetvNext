// <copyright file="Warning.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Microsoft.Build.Framework;

    /// <summary>
    /// An in-memory representation of a warning in MSBuild.
    /// </summary>
    public class Warning : ErrorOrWarningBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Warning"/> class.
        /// </summary>
        /// <param name="w">The <see cref="BuildWarningEventArgs"/> to wrap.</param>
        public Warning(BuildWarningEventArgs w)
            : base(w.Code, w.Message, w.File, w.LineNumber, w.ColumnNumber)
        {
        }

        /// <summary>
        /// Gets the XmlFragment for this Warning.
        /// </summary>
        public XElement XmlFragement
        {
            get
            {
                List<XAttribute> attributes = new List<XAttribute>();

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

                return new XElement("warning", attributes);
            }
        }
    }
}
