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

        $scope.videoAdded = function (video) {
            $scope.videos = $scope.videos.concat(new Array(video));
        };

        $scope.videoFullUpdate = function (videoData) {
            $scope.videos = videoData.Videos;
            $scope.totalTime = videoData.TotalTime;
        };

        $scope.videoRemoved = function (video) {
            var videoIndex = -1;
            for (var i = 0; i < $scope.videos.length; i++) {
                if ($scope.videos[i].ID == video.ID) {
                    videoIndex = i;
                    break;
                }
            }

            console.log("removing " + videoIndex);
            console.log(video.ID + " - " + $scope.videos[videoIndex].ID);
            if (videoIndex > -1) {
                $scope.videos.splice(videoIndex, 1);
            }
        };

        $scope.nowPlayingUpdated = function (nowPlaying) {

            //if (nowPlaying == null) {
            //    console.log("nowPlaying null");
            //} else {
            //    console.log(nowPlaying.Title);
            //}

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

            videos.client.videoAdded = function (video) {
                console.log("videoAdded");
                $scope.videoAdded(video);
            };

            videos.client.videoRemoved = function (video) {
                console.log("videoRemoved");
                $scope.videoRemoved(video);
            };

            videos.client.nowPlayingUpdated = function (video) {
                console.log("nowPlayingUpdated");
                $scope.nowPlayingUpdated(video);
            };

            videos.client.videoFullUpdate = function (videoData) {
                console.log("videoFullUpdate");
                $scope.videoFullUpdate(videoData);
            };

            $.connection.hub.logging = true;

            // Start the connection
            $.connection.hub.start().done(function () {
                console.log('SignalR connected as ID: ' + $.connection.hub.id);
            });
        });
    }]);
})();
