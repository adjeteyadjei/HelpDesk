angular.module('modelResources', ['ngResource']).
factory("Team", ['$resource', function ($resource) {
    return $resource("/api/team/:id", { id: '@id' }, {
        update: { method: 'PUT' }
    });
}]).factory("Ticket", ['$resource', function ($resource) {
    return $resource("/api/ticket/:id", { id: '@id' }, {
        update: { method: 'PUT' }
    });
}]).factory("User", ['$resource', function ($resource) {
    return $resource("/api/user/:id", { id: '@id' }, {
        update: { method: 'PUT' }
    });
}]).factory("Role", ['$resource', function ($resource) {
    return $resource("/api/role/:id", { id: '@id' }, {
        update: { method: 'PUT' }
    });
}]).factory("Project", ['$resource', function ($resource) {
    return $resource("/api/project/:id", { id: '@id' }, {
        update: { method: 'PUT' }
    });
}])