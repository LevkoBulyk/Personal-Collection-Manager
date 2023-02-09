(function () {
    var itemId = document.getElementById("itemId");
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

    console.log(model);

    document.getElementById("addTag").addEventListener("click", function () {
        document.getElementById("tagsPlace").innerHTML +=
            "<input class='tags' class='form-control' />";
    });
})();