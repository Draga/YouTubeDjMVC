var player;
var playerTimeout;
var currentVideo;
var videoHub;

$(function () {
    console.log("document.ready");
    
    // Declare a function on the chat hub so the server can invoke it
    videoHub = $.connection.videos;

    videoHub.client.videoAdded = function () {
        /*console.log("player status: " + player.getPlayerState());
        if (player != null && (player.getPlayerState() == YT.PlayerState.ENDED || player.getPlayerState() == YT.PlayerState.CUED)) {
            playNextVideo();
        }*/
    };

    $.connection.hub.logging = true;

    // Start the connection
    $.connection.hub.start().done(function () {
        console.log('SignalR connected as ID: ' + $.connection.hub.id);
        playNextVideo();
    });

    console.log("document.ready end");
});

function playNextVideo() {

    clearTimeout(playerTimeout);
    
    $.getJSON("/api/VideoApi/PopVideo", function (video) {
        currentVideo = video;
        
        if (video != null) {
            player.loadVideoById(video.YouTubeID);
            updatePlayed();
        }
        
    });
}

function updatePlayed() {

    if (currentVideo != null) {
        videoHub.server.updatePlayTime(currentVideo.ID, player.getCurrentTime());
    }
    
    playerTimeout = setTimeout("updatePlayed()", 5000);
}

function onPlayerReady(event) {
    console.log('player ready');
    //playNextVideo();
}

function onPlayerStateChange(event) {
    if (event.target.getPlayerState() == YT.PlayerState.ENDED) {
        playNextVideo();
    }
}

function onYouTubeIframeAPIReady() {
    $(document).ready(function () {
        player = new YT.Player('player', {
            events: {
                'onReady': onPlayerReady,
                'onStateChange': onPlayerStateChange
            }
        });
    });
}



function addVideoTitles() {
    var videoIDs = Array();
    $("ul li").each(function () {
        videoIDs.push($(this).text());
    });
    $.getJSON("https://www.googleapis.com/youtube/v3/videos",
        {
            part: "snippet",
            id: videoIDs.join(",")
        },
        function (data) {
            alert(data);
        });
}