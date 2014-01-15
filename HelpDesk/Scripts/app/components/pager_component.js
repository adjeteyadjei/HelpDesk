angular.module("pager_component", []).directive("aaPager", ['pagingService', function (pagingService) {
    var linkFn;

    linkFn = function(scope, linkElement, iAttrs) {
        var params = iAttrs["params"];
        if (!params)
            throw new Error("The pager needs a params attribute");

        var tokenized = params.split(":");
        if (tokenized.length < 1)
            throw new Error("The pager needs a pagerId attribute");

        var _pagerId = tokenized[1];
        var pager = pagingService.get(_pagerId);

        var nextBtn = $(linkElement).find("#next");
        var prevBtn = $(linkElement).find("#prev");

        nextBtn.bind("click", function() {
            var page = pager.page();
            if (page < pager.lastPage()) {
                pager.setPage(++page);
            }

            scope.$emit('pageBroadCast');
        });

        prevBtn.bind("click", function() {
            var page = pager.page();
            if (page > 1) {
                pager.setPage(--page);
            }
            scope.$emit('pageBroadCast');
        });

        scope.$on('pageBroadCast', function() {

        });

        scope.$on('dataReceived', function(event, args) {
            summarize(args["pagerId"]);
        });

        function summarize(pagerId) {
            //this line is need else all other pagers will be triggered 
            //thus the last pager to be triggered will override all existing pagers
            if (pagerId !== _pagerId)
                return;
            var pager = window.xx = pagingService.get(pagerId);
            scope.total = pager.totalRecords();
            scope.start = 1 + ((pager.page() - 1) * pager.limit());
            var end = pager.page() * pager.limit();
            if (end > scope.total)
                scope.end = scope.total;
            else
                scope.end = end;
        }
    };
    return {
        restrict: "E",
        link: linkFn,
        templateUrl: "/Scripts/App/Components/partials/pager_widget.html",
        transclude: true,
        scope: {
            total: "@",
            start: "@",
            end: "@"
        }
    };
}]);