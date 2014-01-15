var sharedModules = angular.module('pagingServices', []);

sharedModules.service("pagingService", ['$rootScope', function ($rootScope) {
		/**
		 * A collection of all pagers within the current page
		 * @type {Array}
		 */
		var pagers = [];

		/**
		 * Pager class that need to be created for every pager component
		 * @param {string} name the uniqe Id for the pager component
		 */
		function Pager(name){
			
			/**
			 * A reference to itseld
			 * @type {object}
			 */
			var self = this;

			/**
			 * The uniq id of the pager
			 * @type {String}
			 */
			this.name = name;
			
			/**
			 * The page parameters
			 * @type {Object}
			 */	
			var grid = {
				page  : 1,
				lastPage: 1, //This is should be overwritten with the total
				limit : 25,
				count : 25,
				totalRecords : 25 
			};

			/**
			 * Should be called every time to recompute the all pager parameters. After this is done  
			 * it will notify all its listeners 
			 * @param  {Array} _in the incoming data
			 * @return {void}   
			 */
			this.recalibrate = function (_in){
				grid.totalRecords = _in.total;
				grid.count = _in.data.length;
				grid.lastPage = Math.ceil( grid.totalRecords / grid.limit);
				$rootScope.$broadcast('dataReceived', { pagerId : this.name });
			};
		    this.page = function ( ){
				return grid.page;
			};
		    this.reset = function (){
				grid.page = 1;
			};
		    this.setPage =  function (value){
				grid.page = value;
			};
		    this.limit = function (){
				return grid.limit;
		    };
		    this.totalRecords = function (){
				return grid.totalRecords;
			};
		    this.lastPage =  function () {
				return grid.lastPage;
			};
		}

		/**
		 * Return a single pager
		 * @param  {string} name, the uniue identifier	
		 * @return {Pager}   our pager object
		 */
		function getPager(name){
			
			var pager;
			//console.log(pagers)
			for(var i = 0; i < pagers.length ; i++){
				if(pagers[i]["name"] === name){
					pager = pagers[i];
					break;
				}
			}
			return pager;
			
		}

		/**
		 * Add a pager to the list of pagers
		 * @param {string} name , the unique identifier
		 */
		function addPager (name){
			var pager = getPager(name);
			if(!pager){
				pager = new Pager(name);
				pagers.push(pager);
			}

			return pager;
			
		}

		return {
			add : addPager,
			get : getPager
		};
}]);