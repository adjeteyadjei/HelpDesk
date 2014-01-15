var filterModule = angular.module('filterServices', []);
filterModule.factory('FilterService', ['$rootScope', function ($rootScope) {

	/**
	 * A collection of filters
	 * @type {Array}
	 */
	var filters = [];

	/**
	 * Return a single filter
	 * @param  {string} name, the uniue identifier	
	 * @return {Filter}   our filter object
	 */
	function getFilter(name){
		
		var filter = {
			name : null,
			filter : null
		};

		for(var i = 0; i < filters.length ; i++){
			if(filters[i]["name"] === name){
				filter = filters[i];
				break;
			}
		}

		return filter["filter"];
	}

	/**
	 * Add a filter to the list of filters
	 * @param {string} name , the unique identifier
	 */
	function addFilter (name){
		var filter = getFilter(name);
		if(!filter){
			filter = {
				name : name,
				filter : new AaFilter(name)
			};

			filters.push(filter);
		}
		return filter;
		
	}

	function AaFilter(name){
		/**
		 * A reference to itseld
		 * @type {object}
		 */
		var self = this;

		/**
		 * The uniq id of the filter
		 * @type {String}
		 */
		this.name = name;

		var queryString = "";
		/**
		 * Holds all query parameters
		 * @type {Object}
		 */				
		var queryParams = { };

		this.addParams  = function (obj){
			//angular.extend(queryParams, obj);
			queryParams = obj;
		};
	    this.run = function (){
			$rootScope.$broadcast('recordsFilteredBroadCast', { filterId : this.name });
		};
	    this.getParams = function(){
			return queryParams;
		};
	    this.clear = function (){
			queryParams = {};
		};
	    this.query  = function (){
			return angular.toJson(queryParams);
		};
	}

	return {
		add :  addFilter,
		get : getFilter,
	};
}]);