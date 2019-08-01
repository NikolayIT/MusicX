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

        public static Task<bool> SaveToken(this IJSRuntime runtime, string token)
        {
            return runtime.InvokeAsync<bool>("tokenManager.save", token);
        }

        public static Task<string> ReadToken(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<string>("tokenManager.read");
        }

        public static Task<bool> DeleteToken(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("tokenManager.delete");
        }

        public static Task<bool> StorageSave(this IJSRuntime runtime, string key, string value)
        {
            return runtime.InvokeAsync<bool>("storageManager.save", key, value);
        }

        public static Task<string> StorageRead(this IJSRuntime runtime, string key)
        {
            return runtime.InvokeAsync<string>("storageManager.read", key);
        }

        public static Task<bool> MediaPlayerInitialize(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.initialize");
        }

        public static Task<bool> MediaPlayerPlay(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.play");
        }

        public static Task<bool> MediaPlayerSetSource(this IJSRuntime runtime, string source)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.setSrc", source);
        }

        public static Task<string> GetElementValue(this IJSRuntime runtime, string id)
        {
            return runtime.InvokeAsync<string>("htmlHelper.getElementValue", id);
        }
    }
}
