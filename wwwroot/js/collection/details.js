let currentPage = 1;
let isEndOfList = false;

$(document).ready(function () {
    loadItems();
});

function loadItems() {
    let modelId = document.getElementById('modelId').value;
    let listOfItems = [];
    $.ajax({
        url: '/Item/GetItems',
        type: 'GET',
        data: {
            collectionId: modelId,
            pageNumber: currentPage
        },
        async: false,
        success: function (data) {
            $.each(data, function (key, value) {
                listOfItems.push(mapItem(value));
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
    $('#tableOfItems').DataTable({
        data: listOfItems,
        columnDefs: [
            {
                targets: 1,
                sortable: false
            },
            {
                targets: -1,
                sortable: false
            }
        ]
    });
    $('#tableOfItems tbody tr').addClass('table-secondary');
}

function mapItem(input) {
    console.log(input);
    var item = [];
    item.push(input.title);
    let tagsInfo = "";
    $.each(input.tags, function (key, value) {
        if (tagsInfo.length > 0) {
            tagsInfo += ", "
        }
        tagsInfo += "<a href='" + window.location.origin + "/Search/Index/" + value + "'>" + value + "</a>";
    });
    item.push(tagsInfo);
    $.each(input.values, function (key, value) {
        let val = value === "true" ? "<i class='bi bi-check2'></i>" :
            value === "false" ? "<i class='bi bi-x-lg'></i>" :
                "<div class='multiline-div'>" + value + "</div>";
        item.push(val);
    });
    $.ajax({
        url: '/Item/GetItemActions',
        type: 'GET',
        data: {
            userId: input.userId,
            id: input.id
        },
        async: false,
        success: function (actions) {
            item.push(actions);
        },
        error: function (error) {
            console.log(error);
        }
    });
    return item;
};
