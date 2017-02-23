# MsBuildToCCNetvNext
A Next Generation MsBuildToCCNet Logger that is Multi-Processor-Aware.

Based heavily off the original Rodemeyer.MsBuildToCCnet.dll - a better MsBuild logger for CruiseControl.Net You can find a clone of that original project here: https://github.com/Evengard/MsBuildToCCNet

The design of this is very similar to the original MSBuildToCCNet logger originally designed by Christian Rodemeyer (christian@atombrenner.de) however that original logger was written before a time MSBuild supported multiple threads building.

The original design utilized a Stack queue and state to know which project to associate the events to. However when MSBuild added multithreading support you were no longer guaranteed that events would come in order. To address this Microsoft added a new property (ProjectFile as well as ProjectId) to every EventArg bubbled up by MSBuild's logging.

Therefore the new design utilizes a Dictionary which is Keyed off of the name of the project to understand which project should be modified with the newest log messages. The jury is still out on how much of a performance hit this incurs, especially if the log is particularly chatty.

The work flow is as follows:
* On Initialization:
 * Grab the log file name
 * Initialize our lookup dictionary
 * Register all of our callbacks for each log event
* In each of the callbacks:
 * Call into the Common method to either return or create and then return the associated Project.
 * Append to the project as appropriate.
* On Shutdown:
 * Write the logged messages to the XML File.

Kept from the original design is the fact that the entire build log is stored in memory until Shutdown is called. Again the jury is still out on how much of a hit that takes with a particularly chatty log. However the design remains the same as it was before in that aspect so we wouldn't expect much of a hit.

Another slight difference in the design is the utilization of the XDocument API (which was unavailable at the time of initial writing). This API (at least in this authors opinion) is much clearer/cleaner than the XmlWriter API, however none of this has been perf tested. Because of a switch to this API the XML Serialization of elements was localized to each object which seemed to better suit the design and felt much cleaner.

Yet another slight difference is that this logger will log errors AND warnings in the situation where there is an error logged. This was thought to be the better solution, instead focusing the filtering of the warnings as the user desires in the XLS Transform.
