namespace MusicX.Web.Client.Infrastructure
{
    using System.Threading.Tasks;

    using Microsoft.JSInterop;

    public static class JsInterop
    {
        public static Task<bool> SaveToken(string token)
        {
            // Implemented in jsInterop.js
            return JSRuntime.Current.InvokeAsync<bool>("tokenManager.save", token);
        }

        public static Task<string> ReadToken()
        {
            // Implemented in jsInterop.js
            return JSRuntime.Current.InvokeAsync<string>("tokenManager.read");
        }

        public static Task<bool> DeleteToken()
        {
            // Implemented in jsInterop.js
            return JSRuntime.Current.InvokeAsync<bool>("tokenManager.delete");
        }

        public static Task<bool> InitializeMediaPlayer()
        {
            // Implemented in jsInterop.js
            return JSRuntime.Current.InvokeAsync<bool>("mediaPlayer.initialize");
        }
    }
}
