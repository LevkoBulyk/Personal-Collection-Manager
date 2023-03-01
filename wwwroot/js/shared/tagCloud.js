$(document).ready(function () {
    let itemId = null;
    let itemIdInput = document.getElementById('itemId');
    if (itemIdInput != undefined) {
        itemId = itemIdInput.value;
    }
    $.ajax({
        type: "GET",
        url: "/Tag/GetTagsForCloud",
        data: { itemId: itemId },
        success: function (words) {
            $('#tagCloud').jQCloud(words, {
                /*width: 500,*/
                height: 350,
                autoResize: true,
                margin: 0
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
});