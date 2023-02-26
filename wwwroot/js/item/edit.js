(function () {
    var tagsInputs = document.getElementsByClassName("tags");

    $.each(tagsInputs, function (key, tagInput) {
        tagInput.addEventListener('input', function () {
            $.ajax({
                type: "GET",
                url: "/Tag/List",
                data: { prefix: this.value },
                success: function (response) {
                    datalistOptions = document.getElementById("list_" + tagInput.id);
                    datalistOptions.innerHTML = "";
                    for (var i = 0; i < response.length; i++) {
                        var option = document.createElement("option");
                        option.value = response[i];
                        datalistOptions.appendChild(option);
                    }
                    console.log(datalistOptions);
                },
                error: function (error) {
                    console.log(error);
                }
            });
        });
    });

})();
