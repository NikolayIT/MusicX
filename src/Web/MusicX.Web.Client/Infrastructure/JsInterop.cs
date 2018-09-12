namespace MusicX.Web.Client.Infrastructure
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.JSInterop;

    // Implemented in jsInterop.js
    public static class JsInterop
    {
        public static event Action PlayerEndedPlaybackEvent;

        // ReSharper disable once UnusedMember.Global
        [JSInvokable]
        public static void PlayerEndedPlayback()
        {
            PlayerEndedPlaybackEvent?.Invoke();
        }

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

        public static Task<bool> StorageSave(string key, string value)
        {
            return JSRuntime.Current.InvokeAsync<bool>("storageManager.save", key, value);
        }

        public static Task<string> StorageRead(string key)
        {
            return JSRuntime.Current.InvokeAsync<string>("storageManager.read", key);
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

        public static Task<string> GetElementValue(string id)
        {
            return JSRuntime.Current.InvokeAsync<string>("htmlHelper.getElementValue", id);
        }
    }
}
