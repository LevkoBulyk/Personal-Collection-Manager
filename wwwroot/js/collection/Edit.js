(function () {
    var removeImageButton = document.getElementById("removeImageButton");
    var fileInput = document.getElementById("fileInput");
    var imageUrl = document.getElementById("inputImageUrl");
    var imageDisplay = document.getElementById("fileDisplayArea");
    var exampleDataList = document.getElementById("topic");
    var datalistOptions = document.getElementById("datalistOptions");

    document.addEventListener("drop", function (event) {
        event.preventDefault();
        fileInput.files = event.dataTransfer.files;
        showImage();
        removeImageButton.classList.remove("visually-hidden");
    });

    document.addEventListener("dragover", function (event) {
        event.preventDefault();
    });

    fileInput.addEventListener("change", function () {
        showImage();
        removeImageButton.classList.remove("visually-hidden");
    });

    function showImage() {
        var file = fileInput.files[0];
        var reader = new FileReader();
        reader.onload = function (e) {
            imageDisplay.innerHTML =
                "<img class='collectionImg' src='" + e.target.result + "' />";
        };
        reader.readAsDataURL(file);
    };

    removeImageButton.addEventListener("click", function () {
        fileInput.value = "";
        imageDisplay.innerHTML = "";
        removeImageButton.classList.add("visually-hidden");
        imageUrl.value = "";
    });

    window.addEventListener("load", function () {
        if (fileInput.files.length === 0 && imageUrl.value == "") {
            removeImageButton.classList.add("visually-hidden");
        }
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

    exampleDataList.addEventListener('input', function () {
        $.ajax({
            type: "GET",
            url: "/Collection/Topics",
            data: { prefix: exampleDataList.value },
            success: function (response) {
                datalistOptions.innerHTML = "";
                for (var i = 0; i < response.length; i++) {
                    var option = document.createElement("option");
                    option.value = response[i];
                    datalistOptions.appendChild(option);
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    });

})();
