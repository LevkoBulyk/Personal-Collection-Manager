(function () {
    var markdownDescription = document.getElementById("descriptionMark");

    window.addEventListener("load", function () {
        autosize();
        var easyMDE = new EasyMDE({
            element: markdownDescription,
            toolbar: ["undo", "redo", "|", "bold", "italic", "link", "|", "ordered-list", "unordered-list", "horizontal-rule", "|",
                "heading-1", "heading-2", "heading-3", "heading", "|",
                "quote", "code", "|", "side-by-side", "fullscreen", "preview", "guide"],
            status: false
        });
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
    };
})();