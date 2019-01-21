var app = angular.module('myKeyPress', []);
app.directive('myEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.myEnter);
                });

                event.preventDefault();
            }
        });
    };
});

app.directive('myKeyCltrK', function () {


    return function (scope, element, attrs) {

        var map = { 17: false, 75: false };

        element.bind("keydown keypress", function (event) {
           // console.log(map);
            if (event.which in map) {
                map[event.which] = true;
             
                if (map[17] && map[75]) {

                    scope.$apply(function () {
                        scope.$eval(attrs.myKeyCltrK);
                    });

                    event.preventDefault();
                }
            }

        });
        element.bind("keyup keypress", function (event) {
          
            if (event.which in map) {
                map[event.which] = false;
            }

        });
    };
});

app.directive('myToolTip', function () {

    return {
        terminal: true,
        priority: 1001,
        link: function (scope, element, attributes) {
            // el.removeAttr('my-dir');
            element.attr('data-toggle', 'tooltip');
            element.attr('title', attributes.myToolTip);
        }
    };

});