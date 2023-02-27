(function () {
    let theBiggestCollections = document.getElementById("theBiggestCollections");

    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Home/GetBiggestCollections",
            success: function (responce) {
                theBiggestCollections.innerHTML = responce;
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
})();