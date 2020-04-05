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
            return runtime.InvokeAsync<bool>("tokenManager.save", token).AsTask();
        }

        public static Task<string> ReadToken(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<string>("tokenManager.read").AsTask();
        }

        public static Task<bool> DeleteToken(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("tokenManager.delete").AsTask();
        }

        public static Task<bool> StorageSave(this IJSRuntime runtime, string key, string value)
        {
            return runtime.InvokeAsync<bool>("storageManager.save", key, value).AsTask();
        }

        public static Task<string> StorageRead(this IJSRuntime runtime, string key)
        {
            return runtime.InvokeAsync<string>("storageManager.read", key).AsTask();
        }

        public static Task<bool> MediaPlayerInitialize(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.initialize").AsTask();
        }

        public static Task<bool> MediaPlayerPlay(this IJSRuntime runtime)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.play").AsTask();
        }

        public static Task<bool> MediaPlayerSetSource(this IJSRuntime runtime, string source)
        {
            return runtime.InvokeAsync<bool>("mediaPlayer.setSrc", source).AsTask();
        }
    }
}
