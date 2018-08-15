namespace MusicX.Worker.Common
{
    using System;

    using Newtonsoft.Json;

    public class BaseTaskResult
    {
        internal static readonly JsonSerializerSettings ExceptionResultJsonSerializerSettings =
            new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

        public BaseTaskResult()
        {
            this.Ok = true;
        }

        public BaseTaskResult(Exception exception)
        {
            this.Ok = false;
            this.Exception = exception;
        }

        public bool Ok { get; set; }

        public Exception Exception { get; set; }
    }
}
