(function () {
    let modelId = document.getElementById('modelId').value;
    let columnDefs = [
        {
            targets: 0,
            name: "Title"
        },
        {
            targets: 1,
            sortable: false
        },
        {
            targets: -1,
            sortable: false
        }
    ];
    $.each(document.getElementsByClassName('itemValues'), function (key, value) {
        columnDefs.push({
            targets: 2 + parseInt(value.id),
            name: value.id
        });
    });
    console.log(columnDefs);
    let dataTableOptions = {
        columnDefs: columnDefs,
        createdRow: function (row, data, dataIndex) {
            row.classList.add('table-secondary');
        },
        autoWidth: false,
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Item/GetAllItems',
            type: 'POST',
            async: false,
            data: {
                collectionId: modelId
            },
            error: function (error) {
                console.log(error);
            },
            dataSrc: function (json) {
                let listOfItems = [];
                $.each(json.data, function (key, value) {
                    listOfItems.push(mapItem(value));
                });
                return listOfItems;
            }
        }
    };

    $(document).ready(function () {
        $('#tableOfItems').DataTable(dataTableOptions);
    });

    const mapItem = function mapItem(input) {
        var item = [];
        item.push(input.title);
        let tagsInfo = "";
        $.each(input.tags, function (key, value) {
            if (tagsInfo.length > 0) {
                tagsInfo += ", "
            }
            tagsInfo +=
                "<a href='" +
                window.location.origin +
                "/Search/Index/" +
                value +
                "'>" +
                value +
                "</a>";
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
})();


