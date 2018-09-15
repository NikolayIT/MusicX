namespace MusicX.Worker.Common
{
    public class BaseTaskOutput
    {
        public BaseTaskOutput()
        {
            this.Ok = true;
            this.Error = null;
        }

        public bool Ok { get; set; }

        public string Error { get; set; }
    }
}
