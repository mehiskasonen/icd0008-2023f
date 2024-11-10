angular.module('Cards',['ngSanitize'])
    .directive('card', function() {
        return {
            restrict: 'E',
            template: $('#card-template').html(),
            scope: {
                type: '@',
                content: '@',
                smallContent: '@',
                color: '@'
            },
            link: function(scope, element, attrs, controller) {
                scope.color = scope.color || 'black'
                switch(scope.type) {
                    case 'skip':
                        scope.content = $('#skip-template').html();
                        break;
                    case 'plus-two':
                        scope.content = $('#plus-two-template').html();
                        scope.smallContent = '+2';
                        break;
                    case 'reverse':
                        scope.content = $('#reverse-template').html();
                        break;
                    case 'plus-four':
                        scope.smallContent = '+4';
                        scope.content = $('#plus-four-template').html();
                        console.log(scope.content)
                        break;
                    case 'wild':
                        scope.content = $('#wild-template').html();
                }
                scope.smallContent = scope.smallContent || scope.content;
            }
        }
    });
angular.module('App',['Cards'])