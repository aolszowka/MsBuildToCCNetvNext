// <copyright file="MsBuildToCCNetvNext.cs" company="Ace Olszowka">
// Copyright (c) 2017 Ace Olszowka (GitHub @aolszowka). All rights reserved.
// </copyright>

namespace MsBuildToCCNetvNext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// A next generation MsBuildToCCNet Logger that is Multi-thread
    /// aware.
    /// </summary>
    /// <remarks>
    ///   The design of this is very similar to the original MSBuildToCCNet
    /// logger originally designed by Christian Rodemeyer (christian@atombrenner.de)
    /// however that original logger was written before a time MSBuild
    /// supported multiple threads building.
    ///
    ///   The original design utilized a Stack queue and state to know
    /// which project to associate the events to. However when MSBuild
    /// added multithreading support you were no longer guaranteed that
    /// events would come in order. To address this Microsoft added a
    /// new property (ProjectFile as well as ProjectId) to every EventArg
    /// bubbled up by MSBuild's logging.
    ///
    ///   Therefore the new design utilizes a Dictionary which is Keyed
    /// off of the name of the project to understand which project should
    /// be modified with the newest log messages. The jury is still out
    /// on how much of a performance hit this incurs, especially if the
    /// log is particularly chatty.
    ///
    ///   The work flow is as follows:
    ///     * On Initialization:
    ///       * Grab the log file name
    ///       * Initialize our lookup dictionary
    ///       * Register all of our callbacks for each log event
    ///     * In each of the callbacks:
    ///       * Call into the Common method to either return or create and
    ///         then return the associated Project.
    ///       * Append to the project as appropriate.
    ///     * On Shutdown:
    ///       * Write the logged messages to the XML File.
    ///
    ///   Kept from the original design is the fact that the entire build log
    /// is stored in memory until Shutdown is called. Again the jury is still
    /// out on how much of a hit that takes with a particularly chatty log.
    /// However the design remains the same as it was before in that aspect
    /// so we wouldn't expect much of a hit.
    ///
    ///   Another slight difference in the design is the utilization of the
    /// XDocument API (which was unavailable at the time of initial writing).
    /// This API (at least in this authors opinion) is much clearer/cleaner
    /// than the XmlWriter API, however none of this has been perf tested.
    ///
    ///   Because of a switch to this API the XML Serialization of elements
    /// was localized to each object which seemed to better suit the design
    /// and felt much cleaner.
    /// </remarks>
    public class MsBuildToCCNetvNext : Logger
    {
        /// <summary>
        /// The internal backing store to store all of our projects.
        /// </summary>
        private IDictionary<string, Project> projects;

        /// <summary>
        /// The name/path of the log file.
        /// </summary>
        private string logFile;

        /// <inheritdoc/>
        public override void Initialize(IEventSource eventSource)
        {
            this.logFile = ParseForLogFileName(this.Parameters);
            this.projects = new Dictionary<string, Project>();

            eventSource.ProjectStarted += this.OnProjectStarted;
            eventSource.ErrorRaised += this.OnErrorRaised;
            eventSource.WarningRaised += this.OnWarningRaised;
            eventSource.MessageRaised += this.OnMessageRaised;
        }

        /// <inheritdoc/>
        public override void Shutdown()
        {
            XDocument logFile = new XDocument();

            var errorAndWarningCount = GetTotalErrorAndWarningCount(this.projects.Values);
            var rootNodeAttributes =
                new XAttribute[]
                {
                    new XAttribute("warning_count", errorAndWarningCount.Item2),
                    new XAttribute("error_count", errorAndWarningCount.Item1)
                };
            XElement rootNode = new XElement("msbuild", rootNodeAttributes);

            foreach (var project in this.projects.Values)
            {
                rootNode.Add(project.XmlFragment);
            }

            logFile.Add(rootNode);
            logFile.Save(this.logFile);
        }

        /// <summary>
        /// Given an Enumerable of <see cref="Project"/> return a <see cref="Tuple{T1, T2}"/>
        /// that contains the aggregate number of errors as its first element
        /// and the aggregate number of warnings as its second element.
        /// </summary>
        /// <param name="projects">The list of projects to evaluate.</param>
        /// <returns>A <see cref="Tuple{T1, T2}"/> in which the first element is the number of errors and the second the warnings.</returns>
        private static Tuple<int, int> GetTotalErrorAndWarningCount(IEnumerable<Project> projects)
        {
            int errorCount = 0;
            int warningCount = 0;

            foreach (var project in projects)
            {
                errorCount += project.ErrorCount;
                warningCount += project.WarningCount;
            }

            return new Tuple<int, int>(errorCount, warningCount);
        }

        /// <summary>
        /// Given the parameters sent to this logger, parse for a log file name.
        /// </summary>
        /// <param name="parameters">The parameters sent to this logger.</param>
        /// <returns>The log file name if specified; otherwise, a default of msbuild-output.xml</returns>
        private static string ParseForLogFileName(string parameters)
        {
            string logFileName = "msbuild-output.xml";

            if (!string.IsNullOrWhiteSpace(parameters))
            {
                logFileName = parameters.Split(';').First();
            }

            return logFileName;
        }

        /// <summary>
        /// Determines what message importance levels should be logged.
        /// </summary>
        /// <param name="verbosityLevel">The current verbosity level of the logger.</param>
        /// <param name="messageLevel">The message level of the message to be logged.</param>
        /// <returns><c>true</c> if the message should be logged; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This is adapted from the guidance given here https://msdn.microsoft.com/en-us/library/microsoft.build.framework.loggerverbosity.aspx
        /// </remarks>
        private static bool ShouldLogMessage(LoggerVerbosity verbosityLevel, MessageImportance messageLevel)
        {
            bool shouldLog = false;

            switch (verbosityLevel)
            {
                case LoggerVerbosity.Detailed:
                    {
                        // Detailed verbosity, which displays errors, warnings,
                        // messages with MessageImportance values of High or
                        // Normal, all status events, and a build summary.
                        if (messageLevel == MessageImportance.High || messageLevel == MessageImportance.Normal)
                        {
                            shouldLog = true;
                        }

                        break;
                    }

                case LoggerVerbosity.Diagnostic:
                    {
                        // Diagnostic verbosity, which displays all errors,
                        // warnings, messages, status events, and a build
                        // summary.
                        shouldLog = true;
                        break;
                    }

                case LoggerVerbosity.Minimal:
                    {
                        // Minimal verbosity, which displays errors, warnings,
                        // messages with MessageImportance values of High, and
                        // a build summary.
                        if (messageLevel == MessageImportance.High)
                        {
                            shouldLog = true;
                        }

                        break;
                    }

                case LoggerVerbosity.Normal:
                    {
                        // Normal verbosity, which displays errors, warnings,
                        // messages with MessageImportance values of High,
                        // some status events, and a build summary.
                        if (messageLevel == MessageImportance.High)
                        {
                            shouldLog = true;
                        }

                        break;
                    }

                case LoggerVerbosity.Quiet:
                    {
                        // Quiet verbosity, which displays a build summary.
                        shouldLog = false;
                        break;
                    }

                default:
                    {
                        shouldLog = true;
                        break;
                    }
            }

            return shouldLog;
        }

        /// <summary>
        /// Given the name of the project either lookup or create
        /// the project in our backing Dictionary.
        /// </summary>
        /// <param name="projectFile">The path to the project file.</param>
        /// <returns>A <see cref="Project"/> that is associated with the given path.</returns>
        private Project GetOrCreateAssociatedProject(string projectFile)
        {
            Project project = null;

            // MSBuild will often times log messages that aren't
            // associated with any project, make this the default.
            if (string.IsNullOrEmpty(projectFile))
            {
                projectFile = "MSBuild";
            }

            // See if it already exists in our dictionary
            if (!this.projects.TryGetValue(projectFile, out project))
            {
                project = new Project(projectFile);
                this.projects.Add(projectFile, project);
            }

            return project;
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            Project currentProject = this.GetOrCreateAssociatedProject(e.ProjectFile);
            currentProject.Add(new Error(e));
        }

        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (ShouldLogMessage(this.Verbosity, e.Importance))
            {
                Project currentProject = this.GetOrCreateAssociatedProject(e.ProjectFile);
                currentProject.Add(new Message(e));
            }
        }

        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            Project currentProject = this.GetOrCreateAssociatedProject(e.ProjectFile);
            currentProject.Add(new Warning(e));
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            this.GetOrCreateAssociatedProject(e.ProjectFile);
        }
    }
}
