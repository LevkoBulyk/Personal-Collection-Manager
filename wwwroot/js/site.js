function ConfirmAction(nameOfAction, nameOfObject, event) {
    // TODO: replace with nice-looking window
    if (!confirm("Are you sure, you want to " + nameOfAction + " " + nameOfObject + "?")) {
        event.preventDefault();
    }
};

(function () {
    window.setTimeout(function () {
        $("#alert").fadeTo(500, 0).slideUp(500, function () {
            $(this).remove();
        });
    }, 10000);
}) ();