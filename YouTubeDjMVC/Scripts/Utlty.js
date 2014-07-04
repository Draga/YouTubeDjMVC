function parseSeconds(timeString) {
    return (Date.parse("01/01/01 " + timeString) - Date.parse("01/01/01 00:00:00")) / 1000;
}

function formatSeconds(seconds) {
    return new Date(seconds * 1000).toTimeString().substring(0, 8);
}