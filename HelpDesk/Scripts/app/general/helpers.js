// Provide the wiring information in a module
angular.module('helpers', [])
    .factory("ARR", [function () {
        return {
            sort: function (arr, fieldToUse) {
                var name = fieldToUse || "name";

                function compare(a, b) {
                    if (a[name] < b[name])
                        return -1;
                    if (a[name] > b[name])
                        return 1;
                    return 0;
                }

                return arr.sort(compare);
            }
        };
    }]).factory("DateHelper", ['$filter', function ($filter) {
        return {
            _default: "dd-mm-yyyy",
            formats: {
                "dd-mm-yyyy": "dd-MM-yyyy",
                "dd/mm/yyyy": "dd/MM/yyyy",
                "yyyy/mm/dd": "yyyy/MM/dd",
                "yyyy-mm-dd": "yyyy-MM-dd"
            },
            formatDate: function (date, format) {
                if (!format) {
                    format = this._default;
                }
                var out = $filter('date')(date, this.formats[format]);
                return out;
            },
            toDate: function (date) {
                date = date.substring(0, 10);
                return date;
            },
            datediff: function (startDate, endDate) {
                var one_day = 1000 * 60 * 60 * 24;
                var sTime = new Date(startDate).getTime();
                var eTime = new Date(endDate).getTime();
                var out = Math.ceil((eTime - sTime)) / one_day;
                return out;
            },
            toDay: function () {
                var today = this.formatDate(new Date(), "yyyy-mm-dd");
                return today;
            },
            addDay: function (date, days) {
                var result = new Date(date).getTime() + ((1000 * 60 * 60 * 24) * days);
                var out = this.formatDate(result, "yyyy-mm-dd");
                return out;
            }
        };
    }]).factory("MsgBox", [function () {
        return {
            show: function (msg, status) {
                $.pnotify({
                    title: status.toUpperCase(),
                    text: msg,
                    type: status,
                    history: false,
                    closer: true
                });
            },
            success: function (msg) {
                this.show(msg, "success");
            },
            error: function (msg) {
                this.show(msg, "error");
            },
            info: function (msg) {
                this.show(msg, "info");
            },
            notice: function (msg) {
                this.show(msg, "notice");
            }
        };
    }]).factory("OBJ", [function () {
        return {
            rectify: function (obj, _default) {
                var out = {};
                if (obj) {
                    for (var i in _default) {
                        obj[i] = obj[i] || _default[i];
                    }

                    out = obj;
                } else {
                    out = _default;
                }
                return out;
            }
        };
    }]);