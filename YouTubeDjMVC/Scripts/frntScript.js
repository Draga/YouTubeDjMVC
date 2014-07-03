(function () {
    var app = angular.module('dj', ['ui.bootstrap']);

    app.controller('VideoController', ['$http', '$scope', '$q', function ($http, $scope, $q) {
        $scope.search = {};
        $scope.results = [];
        $scope.errorMessage = '';
        $scope.videos = [];
        //$scope.videoRefreshTimeout = [];

        this.addVideo = function (video) {
            $scope.errorMessage = '';
            $http({
                url: '/api/VideoApi/PostYouTubeVideo',
                method: "POST",
                headers: { 'Content-Type': 'application/json; charset=utf-8' },
                dataType: 'json',
                data: JSON.stringify(video)
            })
            .success(function ($data) {
                if (!$data.Success) {
                    $scope.errorMessage = $data.Message;
                }
                $scope.results = [];
                $scope.search = {};
            })
            .error(function ($data) {
                alert("bye");
                if (!$data.Success) {
                    alert($data.Message);
                    $scope.errorMessage = $data.Message;
                }
            });
        };

        $scope.getVideoList = function () {
            $http({
                url: '/api/VideoApi/GetVideos',
                method: "GET"
            })
                .success(function ($videos) {
                    $scope.videos = $videos;
                })
                .error(function (data) {
                    //TODO: use json messages for videoList.php
                    $scope.errorMessage = data.message;
                });
            
            $http({
                url: '/api/VideoApi/GetNowPlaying',
                method: "GET"
            })
                .success(function ($nowPlaying) {
                    if ($nowPlaying == null) {
                        $scope.nowPlaying = null;
                    } else {
                        $scope.nowPlaying = $nowPlaying.Title;
                    }
                })
                .error(function (data) {
                    //TODO: use json messages for videoList.php
                    $scope.errorMessage = data.message;
                });
           /* clearTimeout($scope.videoRefreshTimeout);
            $scope.videoRefreshTimeout = setTimeout(function () {
                $scope.getVideoList();
            }, 60000);*/
        };

        this.search = function () {
            $scope.errorMessage = '';
            $http({
                url: 'http://gdata.youtube.com/feeds/api/videos?max-results=4&v=2&alt=json&q=' + $scope.search.text,
                method: "GET"
            })
                .success(function (data) {
                    $scope.results = data.feed.entry;
                });
        };

        $scope.getAutocompletion = function (query) {
            var dfr = $q.defer();
            $http.jsonp('http://suggestqueries.google.com/complete/search?ds=yt&client=firefox&callback=JSON_CALLBACK', {
                params: {
                    q: query
                }
            }).success(function (results) {
                var queries = [];
                angular.forEach(results[1], function (result) {
                    queries.push(result);
                });
                dfr.resolve(queries);
            });
            return dfr.promise;
        };

        $(document).ready(function () {
            $('input#searchText').focus();
            //$('input#searchText').typeahead();
    
            /** SIGNALR **/
   
            // Declare a function on the chat hub so the server can invoke it
            var videos = $.connection.videos;

            videos.client.videoAdded = function () {
                //alert(scope);
                $scope.getVideoList();
                //alert(app);
            };
    
            $.connection.hub.logging = true;
    
            // Start the connection
            $.connection.hub.start().done(function() {
                console.log('SignalR connected as ID: ' + $.connection.hub.id);
            });
        });
        
        $scope.getVideoList();
    }]);
})();
