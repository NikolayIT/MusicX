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
