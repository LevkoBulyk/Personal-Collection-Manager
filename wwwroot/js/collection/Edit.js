(function () {
    var removeImageButton = document.getElementById("removeImageButton");
    var fileInput = document.getElementById("fileInput");
    var imageUrl = document.getElementById("inputImageUrl");

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
            document.getElementById("fileDisplayArea").innerHTML =
                "<img class='collectionImg' src='" + e.target.result + "' />";
        };
        reader.readAsDataURL(file);
    };

    removeImageButton.addEventListener("click", function () {
        fileInput.value = "";
        document.getElementById("fileDisplayArea").innerHTML = "";
        removeImageButton.classList.add("visually-hidden");
        imageUrl.value = "";
    });

    window.addEventListener("load", function () {
        if (fileInput.files.length === 0 && imageUrl.value == "") {
            removeImageButton.classList.add("visually-hidden");
        }
    });
})();