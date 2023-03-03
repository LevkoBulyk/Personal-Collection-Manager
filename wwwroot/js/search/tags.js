(function () {
    const tableId = "tableOfItems";

    $(document).ready(function () {
        let tag = document.getElementById('tag').value;

        let columnDefsHome = [
            {
                targets: 0,
                sortable: false,
                render: function (data, type, row) {
                    return `<a href="/Item/Details/${row.id}">${row.title}</a>`;
                },
            },
            {
                targets: 1,
                sortable: false,
                render: function (data, type, row) {
                    var res = "";
                    $.each(row.tags, function (key, value) {
                        if (res.length > 0) {
                            res += ", ";
                        }
                        res += `<a href="/Search/Index/${value}">${value}</a>`;
                    });
                    return res;
                },
            },
            {
                targets: 2,
                sortable: false,
                render: function (data, type, row) {
                    return `<a href="/Collection/Details/${row.collectionId}">${row.collectionTitle}</a>`;
                },
            },
            {
                targets: 3,
                sortable: false,
                render: function (data, type, row) {
                    return `<a href="/Collection/All?userId=${row.authorId}">${row.authorEmail}</a>`;
                },
            },
        ];

        const dataTableOptions = {
            columnDefs: columnDefsHome,
            createdRow: function (row, data, dataIndex) {
                row.classList.add("bg-gray-opaque");
            },
            autoWidth: false,
            searching: false,
            sorting: false,
            processing: true,
            serverSide: true,
            ajax: {
                url: "/Search/GetItems",
                type: "GET",
                data: { tag: tag },
                async: false,
                dataSrc: function (json) {
                    console.log(json);
                    return json.data;
                },
            },
        };

        const initDataTable = function () {
            $("#" + tableId).DataTable(dataTableOptions);
            let selectElement = document.querySelector(
                'select[name="' + tableId + '_length"]'
            );
            selectElement.classList.add("inherit-color");
            $(selectElement)
                .find("option")
                .addClass("inherit-color");
        }

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
})();
