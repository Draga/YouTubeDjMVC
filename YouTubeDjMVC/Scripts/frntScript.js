(function () {
    var app = angular.module('dj', ['ui.bootstrap']);

    app.controller('VideoController', ['$http', '$scope', '$q', '$timeout', function ($http, $scope, $q, $timeout) {
        $scope.search = {};
        $scope.results = [];
        $scope.errorMessage = '';
        $scope.videos = [];
        $scope.nowPlayingPercent = 0;
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
            .success(function (data) {
                if (!data.Success) {
                    $scope.errorMessage = data.Message;
                }
                $scope.results = [];
                $scope.search = {};
            })
            .error(function (data) {
                alert("bye");
                if (!data.Success) {
                    alert(data.Message);
                    $scope.errorMessage = data.Message;
                }
            });
        };

        $scope.getVideoList = function () {
            $http({
                url: '/api/VideoApi/GetVideoData',
                method: "GET"
            })
                .success(function (videoData) {
                    $scope.videos = videoData.Videos;
                    $scope.totalTime = videoData.TotalTime;
                })
                .error(function (data) {
                    //TODO: use json messages for videoList.php
                    $scope.errorMessage = data.message;
                });
        };

        $scope.getNowPlaying = function () {

            $http({
                url: '/api/VideoApi/GetNowPlaying',
                method: "GET"
            })
                .success(function (nowPlaying) {

                    //Fix me
                    if (nowPlaying != null && nowPlaying.PlayedTime == null) {
                        nowPlaying.PlayedTime = "00:00:00";
                    }

                    if (nowPlaying == null) {
                        $scope.nowPlaying = null;
                        $scope.nowPlayingPercent = 0;
                    } else {
                        $scope.nowPlaying = nowPlaying;
                        $scope.nowPlayingPercent = parseSeconds(nowPlaying.PlayedTime) * 100 / parseSeconds(nowPlaying.Length);

                        var autoUpdateNowPlaying = function () {
                            if ($scope.nowPlaying != null) {
                                var currentTime = $scope.nowPlaying.PlayedTime;
                                var currentSeconds = parseSeconds(currentTime);
                                
                                var totalTime = $scope.nowPlaying.Length;
                                var totalSeconds = parseSeconds(totalTime);
                                
                                if (currentSeconds != 0 && currentSeconds < totalSeconds) {
                                    currentSeconds++;
                                    var newTime = formatSeconds(currentSeconds);
                                    $scope.nowPlaying.PlayedTime = newTime;
                                    $scope.nowPlayingPercent = parseSeconds(nowPlaying.PlayedTime) * 100 / parseSeconds(nowPlaying.Length);
                                }
                            }
                            
                            $scope.autoUpdateNowPlayingTimeout = $timeout(autoUpdateNowPlaying, 1000);
                        };
                        
                        $timeout.cancel($scope.autoUpdateNowPlayingTimeout);
                        $scope.autoUpdateNowPlayingTimeout = $timeout(autoUpdateNowPlaying, 1000);
                    }
                    
                })
                .error(function (data) {
                    //TODO: use json messages for videoList.php
                    $scope.errorMessage = data.message;
                });

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

            videos.client.nowPlayingUpdated = function () {
                //alert(scope);
                $scope.getNowPlaying();
                //alert(app);
            };

            $.connection.hub.logging = true;

            // Start the connection
            $.connection.hub.start().done(function () {
                console.log('SignalR connected as ID: ' + $.connection.hub.id);
            });
        });

        $scope.getVideoList();
        $scope.getNowPlaying();
    }]);
})();
