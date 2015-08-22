(function () {
    angular.module('BridgePortal')
        .factory('logger', function () {
            return {
                success: function (message) {
                    toastr.success(message);
                },
                error: function (err) {
                    toastr.error((err && err.data && err.data.exceptionMessage) ? err.data.exceptionMessage : 'An error has occurred');
                }
            }
        });
})();