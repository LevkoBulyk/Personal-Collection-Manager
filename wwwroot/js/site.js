function ConfirmAction(nameOfAction, nameOfObject, event) {
    // TODO: replace with nice-looking window
    if (!confirm("Are you sure, you want to " + nameOfAction + " " + nameOfObject + "?")) {
        event.preventDefault();
    }
}

(function () {
    setTheme = function (theme, elem) {
        document.querySelector('html').setAttribute('data-bs-theme', theme);
        $(document.getElementById('themesDropdown')).find('button').removeClass('active');
        elem.classList.add('active');
        $.ajax({
            type: "POST",
            url: "/Home/SetTheme",
            data: { theme: theme },
            success: function (response) {
                console.log(response);
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

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