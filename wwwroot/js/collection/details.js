(function () {
    let modelId = document.getElementById('modelId').value;
    let itemsList = [];
    let columnDefs = [
        {
            targets: 1,
            sortable: false
        },
        {
            targets: -1,
            sortable: false
        }
    ];
    let dataTableOptions = {
        columnDefs: columnDefs,
        processing: true,
        createdRow: function (row, data, dataIndex) {
            row.classList.add('bg-gray-opaque');
        },
        autoWidth: false,
        processing: true,
        data: itemsList
    };

    const initDataTable = function () {
        $.ajax({
            url: '/Item/GetAllItems',
            type: 'POST',
            async: false,
            data: {
                collectionId: modelId
            },
            success: function (response) {
                $.each(response, function (key, value) {
                    itemsList.push(mapItem(value));
                })
                $('#tableOfItems').DataTable(dataTableOptions);
                let selectElement = document.querySelector('select[name="tableOfItems_length"]');
                selectElement.classList.add('inherit-color');
                let inputElement = document.querySelector('input[aria-controls="tableOfItems"]');
                inputElement.classList.add('inherit-color');
                $(selectElement).find('option').addClass('inherit-color');
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

    $(document).ready(function () {
        if (culture == 'uk-UA') {
            fetch("https://cdn.datatables.net/plug-ins/1.13.3/i18n/uk.json")
                .then((response) => response.json())
                .then((data) => {
                    dataTableOptions.language = data;
                    initDataTable();
                });
        } else {
            initDataTable();
        }
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


