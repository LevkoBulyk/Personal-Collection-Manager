(function () {
    "use strict";
    var connection = new signalR.HubConnectionBuilder().withUrl("/commentHub").build();
    let itemId = document.getElementById("itemId").value;

    connection.on("NewComment_" + itemId, function (commentId) {
        console.log(commentId);

    });

    connection.start();

})();