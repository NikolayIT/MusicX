namespace MusicX.Web.Client.Infrastructure
{
    using System.Threading.Tasks;

    using Microsoft.JSInterop;

    // Implemented in jsInterop.js
    public static class JsInterop
    {
        public static Task<bool> SaveToken(string token)
        {
            return JSRuntime.Current.InvokeAsync<bool>("tokenManager.save", token);
        }

        public static Task<string> ReadToken()
        {
            return JSRuntime.Current.InvokeAsync<string>("tokenManager.read");
        }

        public static Task<bool> DeleteToken()
        {
            return JSRuntime.Current.InvokeAsync<bool>("tokenManager.delete");
        }

        public static Task<bool> MediaPlayerInitialize()
        {
            return JSRuntime.Current.InvokeAsync<bool>("mediaPlayer.initialize");
        }

        public static Task<bool> MediaPlayerPlay()
        {
            return JSRuntime.Current.InvokeAsync<bool>("mediaPlayer.play");
        }

        public static Task<bool> MediaPlayerSetSource(string source)
        {
            return JSRuntime.Current.InvokeAsync<bool>("mediaPlayer.setSrc", source);
        }
    }
}
