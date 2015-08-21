(function() {
    angular.module('BridgePortal')
        .factory('UrlBuilder', function() {
            return {
                build: function(url) {
                    return 'http://localhost/BridgeWebAPI/' + url;
                }    
            }
        });
})();