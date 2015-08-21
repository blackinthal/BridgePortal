(function () {
    angular.module('BridgePortal')
        .factory('logger', function () {
            return {
                success: function (message) {
                    toastr.success(message);
                },
                error: function(err) {
                    toastr.error(err.message || 'An error has occurred');
                }
            }
        });
})();