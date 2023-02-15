(function () {
    /*var itemId = document.getElementById("itemId");
    var collectionId = document.getElementById("collectionId");
    var title = document.getElementById("title");
    var tags = document.getElementsByClassName("tags");

    var model = {
        id: itemId.value,
        collectionId: collectionId.value,
        title: title.value,
        tags: [],
        fields: []
    };

    $.each(tags, function (tag) {
        model.tags.push(tag.value);
    });

    document.getElementById("addTag").addEventListener("click", function () {
        document.getElementById("tagsPlace").innerHTML +=
            "<input class='tags' class='form-control' />";
    });*/

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
