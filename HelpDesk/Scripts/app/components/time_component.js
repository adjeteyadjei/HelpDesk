angular.module("time_component",[]).directive("axTime", [function (){
	var linkFn = function (scope, linkElement,attrs,ngModel){
			var _attributes = {};
	    var ats = attrs.$attr; //console.log(attrs)
			var min = parseInt(attrs["min"]) || 0;
			var max = parseInt(attrs["max"]) || 23;

			for (var key in ats){
			  _attributes [ats[key]] = attrs[ats[key]];
			}
			_attributes["ng-model"] = attrs["ngModel"];
			var select = "<select " +
						"ng-options='time as time for time in times'" +
						" >" +
						"</select>";
			var el = $(select).attr(_attributes);			
			linkElement.append(el);

			function computeValues(){
				var times = [];
				for (var i=min; i < max + 1 ; i++){
				  	if (i < 10){
				   		times.push("0"+ i + ":00");
				  	}else{
				   		times.push(i + ":00");
				  	}
				}

				return times;
			}

			function appendValues(){
				var times = computeValues();
				times.forEach(function (time){
					el.append("<option>"+ time+"</option>");
				});
			}

			appendValues();

			scope.$watch(attrs.ngModel, function (value){
				if(!angular.isDate(value)){
					
				}
				el.val(value);
			});
	    el.bind("change", function (){
				var newValue = $(this).val();
				scope.$apply(function () {
					ngModel.$setViewValue(newValue);
		        });
			});
	};
    return  {
		link : linkFn,
		require: '?ngModel',
		//templateUrl: 'js/app/components/partials/time_widget.html',
		restrict : "E",
	};
}]);
