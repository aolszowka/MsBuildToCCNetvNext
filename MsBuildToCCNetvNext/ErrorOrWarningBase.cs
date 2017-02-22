// <copyright file="ErrorOrWarningBase.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    /// <summary>
    /// A base class that represents an in-memory Error or Warning.
    /// </summary>
    public abstract class ErrorOrWarningBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOrWarningBase"/> class.
        /// </summary>
        /// <param name="code">The Code associated with this Error or Warning.</param>
        /// <param name="text">The Text associated with this Error or Warning.</param>
        /// <param name="file">The File associated with this Error or Warning.</param>
        /// <param name="line">The Line associated with this Error or Warning.</param>
        /// <param name="column">The Column associated with this Error or Warning.</param>
        public ErrorOrWarningBase(string code, string text, string file, int line, int column)
        {
            this.Code = code ?? string.Empty;
            this.Text = text ?? string.Empty;
            this.File = file ?? string.Empty;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets the Code associated with this Error or Warning.
        /// </summary>
        public string Code
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Text associated with this Error or Warning.
        /// </summary>
        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the File associated with this Error or Warning.
        /// </summary>
        public string File
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Line associated with this Error or Warning.
        /// </summary>
        public int Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Column associated with this Error or Warning.
        /// </summary>
        public int Column
        {
            get;
            private set;
        }
    }
}
