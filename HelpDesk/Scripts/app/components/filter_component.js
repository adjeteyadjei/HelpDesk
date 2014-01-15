
angular.module("filter_component", []).directive("aaFilter", ['FilterService', function (FilterService) {


	var compileFn = function (element,attrs){
	var useDates = true;
	var errrorMsg = "";
	var params = attrs["params"];
	var filterId;
	var noFilterIdMsg = "aaFilter needs a params attribute with a filter of format filterId:your_filter_id";
	if(params){
		var data = params.split(";");
		var defaults = {};
	    for (var i = 0; i < data.length; i++) {				
			var param = data[i].split(":");
			defaults [param[0]] = param[1];
		};
		
		filterId = defaults["filterId"];
		if(!filterId)
			throw new Error(noFilterIdMsg);

		if($.parseJSON(defaults["useDates"]) === false)
			useDates = false;
	}else{
		throw new Error(errrorMsg);
	}
		

	
	var datesTemplate = "<div class='form-group'>" +
			    "<label for='start_date'>Start Date:</label>" +
			    "<input type='text' class='form-control' id='start_date' ng-model='aaFilter.startDate'>" +
			    "</div>" +
			    "<div class='form-group'>" +
			    "<label for='end_date'>End Date:</label>" +
			    "<input type='text' id='end_date' class='form-control' ng-model='aaFilter.endDate'>" +
			    "</div>" ;

	var template = "<div class='btn-group filter_box'>" +
    "<button type='button' class='btn btn-default dropdown-toggle' data-toggle='dropdown'>Search <span class='caret'></span></button>" +
    "<div class='dropdown-menu'>" +
	    	"<form class='form-horizontal'  role='form'>" +
	    	"<div class='extra_controls'></div> " ;
	    		
	if(useDates)
		template +=  datesTemplate;
	
	var templateEnd = "<div class='btns'>"+
	"<button type='button' class='btn btn-primary' id='filter_button'><i class='iconfa-search'></i>Search</button>" +
	"<button class='btn btn-default' id='clear_button'>Clear</button></div> " +

	    	"</form>" +
	    "</div>" +
	"</div>" ;

	template += templateEnd;
	
	var children = element.children();
	element.html(template);
	//this is a little hack to stop the form from being hiding when clicked
	$(".filter_box form").bind("click", function () {
		return false;
	});
	
	$(element).find(".filter_box .extra_controls").append(children);

	
	return {
			pre : function (scope, element){
				var filterButton = $(element).find("#filter_button");
				var filter = FilterService.get(filterId);
				var params = {};
				filterButton.bind("click",function (){
					if(useDates){
						angular.extend(params, scope["aaFilter"]);
					}
					
					angular.extend(params, scope[filterId]);

					for(var p in params){
						if (!params[p]){
							delete params[p];
						}
					}

					filter.addParams(params);
					filter.run();

				});

				var clearButton = $(element).find("#clear_button");
				clearButton.bind("click", function () {
				    console.log('This is to close the search form');
					if(useDates){
						for (var i in scope["aaFilter"]){
							scope["aaFilter"][i] = undefined;
						}
					}
					
					for (var i in scope[filterId]){
						scope[filterId][i] = undefined;
					}
					
					filter.clear();
					scope.$digest();
				});
			},
			post : function (scope, element){
				//console.log(element.children()[0])
			}
		};
	};
    var out = {
		compile : compileFn,
		//link : linkFn,
		//templateUrl: 'js/app/components/partials/filter_widget.html',
		restrict : "E",
		replace: true
	};
    return  out;
}]);
