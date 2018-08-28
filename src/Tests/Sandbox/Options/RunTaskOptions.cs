namespace Sandbox
{
    using CommandLine;

    [Verb("task", HelpText = "Run worker task.")]
    public class RunTaskOptions
    {
        [Option('t', "taskName", Required = true, HelpText = "The task class name to run.")]
        public string TaskName { get; set; }

        [Option('p', "taskParameters", Required = false, Default = "", HelpText = "The task parameters as a JSON string.")]
        public string Parameters { get; set; }
    }
}
