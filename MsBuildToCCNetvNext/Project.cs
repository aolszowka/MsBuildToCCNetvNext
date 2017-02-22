// <copyright file="Project.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;

    /// <summary>
    /// An in-memory representation of the results of this project in MSBuild.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// All of the errors associated with this project.
        /// </summary>
        private IList<Error> errors;

        /// <summary>
        /// All of the messages associated with this project.
        /// </summary>
        private IList<Message> messages;

        /// <summary>
        /// All of the warnings associatd with this project.
        /// </summary>
        private IList<Warning> warnings;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <param name="file">The project file that this is associated with.</param>
        public Project(string file)
        {
            this.File = file;
            this.errors = new List<Error>();
            this.messages = new List<Message>();
            this.warnings = new List<Warning>();
        }

        /// <summary>
        /// Gets the File associated with this project.
        /// </summary>
        public string File
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Errors associated with this Project.
        /// </summary>
        public IEnumerable<Error> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Gets the total number of errors in this project.
        /// </summary>
        public int ErrorCount
        {
            get
            {
                return this.errors.Count;
            }
        }

        /// <summary>
        /// Gets the Messages associated with this Project.
        /// </summary>
        public IEnumerable<Message> Messages
        {
            get
            {
                return this.messages;
            }
        }

        /// <summary>
        /// Gets the Warnings associated with this Project.
        /// </summary>
        public IEnumerable<Warning> Warnings
        {
            get
            {
                return this.warnings;
            }
        }

        /// <summary>
        /// Gets the total number of warnings in this project.
        /// </summary>
        public int WarningCount
        {
            get
            {
                return this.warnings.Count;
            }
        }

        /// <summary>
        /// Gets the XmlFragement for this Project.
        /// </summary>
        public XElement XmlFragment
        {
            get
            {
                var projectAttributes =
                    new XAttribute[]
                    {
                        new XAttribute("dir", Path.GetDirectoryName(this.File) ?? string.Empty),
                        new XAttribute("name", Path.GetFileName(this.File) ?? string.Empty)
                    };
                XElement projectElement = new XElement("project", projectAttributes);

                // Add the Errors
                foreach (var error in this.errors)
                {
                    projectElement.Add(error.XmlFragement);
                }

                // Then the Warnings
                foreach (var warning in this.warnings)
                {
                    projectElement.Add(warning.XmlFragement);
                }

                // Finally the Messages at the bottom
                foreach (var message in this.messages)
                {
                    projectElement.Add(message.XmlFragement);
                }

                return projectElement;
            }
        }

        /// <summary>
        /// Associates an <see cref="Error"/> with this <see cref="Project"/>.
        /// </summary>
        /// <param name="e">The <see cref="Error"/> to associate.</param>
        public void Add(Error e)
        {
            this.errors.Add(e);
        }

        /// <summary>
        /// Associates an <see cref="Message"/> with this <see cref="Project"/>.
        /// </summary>
        /// <param name="m">The <see cref="Message"/> to associate.</param>
        public void Add(Message m)
        {
            this.messages.Add(m);
        }

        /// <summary>
        /// Associates an <see cref="Warning"/> with this <see cref="Project"/>.
        /// </summary>
        /// <param name="w">The <see cref="Warning"/> to associate.</param>
        public void Add(Warning w)
        {
            this.warnings.Add(w);
        }
    }
}
