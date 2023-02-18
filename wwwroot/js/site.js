function ConfirmAction(nameOfAction, nameOfObject, event) {
    // TODO: replace with nice-looking window
    if (!confirm("Are you sure, you want to " + nameOfAction + " " + nameOfObject + "?")) {
        event.preventDefault();
    }
}

(function () {
    window.setTimeout(function () {
        $("#alert").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 10000);

    window.addEventListener("load", function () {
        autosize();
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