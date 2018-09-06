// TODO: use another storage method. Read this: https://dev.to/rdegges/please-stop-using-local-storage-1i04
window.tokenManager = {
    save: function (token) {
        window.localStorage.setItem('jwt', token);
        console.log("Authentication token has been stored.");
        return true;
    },
    read: function () {
        var token = window.localStorage.getItem('jwt');
        console.log(token ? "Authentication token read from storage." : "No authentication token found in storage.");
        return token;
    },
    delete: function () {
        window.localStorage.removeItem('jwt');
        console.log("Authentication token has been deleted.");
        return true;
    }
};

window.storageManager = {
    save: function(key, data) {
        window.localStorage.setItem(key, data);
        return true;
    },
    read: function(key) {
        var value = window.localStorage.getItem(key);
        return value;
    }
};

window.mediaPlayer = (function() {
    var player = null;
    return {
        initialize: function () {
            if (player !== null) {
                return false;
            }

            player = new MediaElementPlayer('playerElement',
                {
                    success: function(playerInstance, node) {
                        playerInstance.addEventListener('ended',
                            function(e) {
                                DotNet.invokeMethodAsync('MusicX.Web.Client', 'PlayerEndedPlayback');
                            });
                        playerInstance.addEventListener('error',
                            function(e) {
                                console.log("MediaPlayer.js event error: " + e);
                                setTimeout(function () {
                                        // TODO: Report this song
                                        DotNet.invokeMethodAsync('MusicX.Web.Client', 'PlayerEndedPlayback');
                                    },
                                    1000);
                            });
                    },
                    error: function(e) {
                        console.log("MediaPlayer.js error: " + e);
                    }
                });
            return true;
        },
        getPlayer: function() {
            return player;
        },
        play: function () {
            player.play();
            return true;
        },
        setSrc: function (url) {
            player.setSrc(url);
            player.load();
            return true;
        }
    };
}());
