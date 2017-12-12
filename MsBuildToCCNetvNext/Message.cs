// <copyright file="Message.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    using System;
    using System.Xml.Linq;
    using Microsoft.Build.Framework;

    /// <summary>
    /// An in-memory representation of a message in MSBuild.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Message"/> class.
        /// </summary>
        /// <param name="e">The <see cref="BuildMessageEventArgs"/> to wrap.</param>
        public Message(BuildMessageEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            this.Text = e.Message;
            this.Importance = e.Importance;
        }

        /// <summary>
        /// Gets the Text associated with this Message.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="MessageImportance"/> associated with this message.
        /// </summary>
        public MessageImportance Importance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the XmlFragement for this Message.
        /// </summary>
        public XElement XmlFragement
        {
            get
            {
                return new XElement("message", new XAttribute("importance", this.Importance), Utilities.SanitizeMessageForXml(this.Text));
            }
        }
    }
}
