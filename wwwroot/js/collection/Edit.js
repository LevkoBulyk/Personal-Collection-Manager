﻿(function () {
    var removeImageButton = document.getElementById("removeImageButton");
    var fileInput = document.getElementById("fileInput");
    var imageUrl = document.getElementById("inputImageUrl");
    var imageDisplay = document.getElementById("fileDisplayArea");
    var markdownDescription = document.getElementById("descriptionMark");
    var scrollPosition = document.getElementById("scrollPosition");
    //var htmlDescription = document.getElementById("descriptionInHtml");

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

    function autosize() {
        var text = $('.autosize');

        text.each(function () {
            $(this).attr('rows', 1);
            resize($(this));
        });

        text.on('input', function () {
            resize($(this));
        });

        function resize($text) {
            $text.css('height', 'auto');
            $text.css('height', $text[0].scrollHeight + 'px');
        }
    }

    window.addEventListener('scroll', function () {
        scrollPosition.value = window.pageYOffset;
    });

    window.addEventListener("load", function () {
        if (fileInput.files.length === 0 && imageUrl.value == "") {
            removeImageButton.classList.add("visually-hidden");
        }
        autosize();
        window.scroll(0, scrollPosition.value);
        var easyMDE = new EasyMDE({
            element: markdownDescription,
            toolbar: ["undo", "redo", "|", "bold", "italic", "link", "|", "ordered-list", "unordered-list", "horizontal-rule", "|",
                "heading-1", "heading-2", "heading-3", "heading", "|",
                "quote", "code", "|", "side-by-side", "fullscreen", "preview", "guide"],
            status: false
        });
    });

})();