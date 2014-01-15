angular.module("print_component",[]).directive("axPrinter", [function (pagingService, FilterService){
	var linkFn = function (scope, linkElement,attrs){
		var url = "print_generic?";
			var atts = attrs;
			
			var params = attrs["params"];

			if(!params)
				throw new Error("printer component needs a params attribute with values in it");
			
			var data = params.split(";");

			var defaults = {};

			for (var i = 0; i < data.length; i++) {				
				var param = data[i].split(":");
				defaults [param[0]] = param[1];
			};

			
			var fileToPrint = defaults["file"];

			if(!fileToPrint)
				throw new Error("printer needs a file element specified as file:name_of_file_to_print");
				
			scope.printPage = function (type){
				var filterId = defaults["filterId"];
				var filter;
				if(filterId){
					filter = FilterService.get(filterId);
				}

				var filterString = [];
				if(filter){
					var filterObj = filter.getParams();
					for(var x in filterObj){
						filterString.push( x + '=' + filterObj[x]);	
					}
				}
					
				var query = "";
				switch(type){
					case "all":
						query = "type=all" + "&" + filterString.join("&");
					break;
					case "single":
						var pager = pagingService.get(defaults["pagerId"]);
						query = "type=single&page=" + pager.page() + "&limit=" + pager.limit() + "&" + filterString.join("&");
					break;
				}
				
				var win = window.open(url + "print_page=" + fileToPrint +"#query?" + query, null,'height=720,width=960', false);
				//win.print()
			};
	};
    return  {
		link : linkFn,
		templateUrl: 'js/app/components/partials/print_widget.html',
		restrict : "E"
	};
}]);
