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

    $(document).ready(function () {
        delay(500).then(() => {
            $(document.getElementsByClassName("editor-toolbar")[0]).find('button').addClass('inherit-color');
            document.getElementsByClassName("CodeMirror-wrap")[0].classList.add('inherit-color');
        });
    });

    function delay(time) {
        return new Promise(resolve => setTimeout(resolve, time));
    }

})();
