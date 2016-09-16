(function() {
    angular.module('BridgePortal')
        .factory('UrlBuilder', function() {
            return {
                build: function(url) {
                    return 'https://microsoft-apiappcb6b7bed641c445c925b13f9774fc058.azurewebsites.net/' + url;
                }    
            }
        });
})();