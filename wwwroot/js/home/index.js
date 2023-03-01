(function () {
    let theBiggestCollections = document.getElementById("theBiggestCollections");
    const tableId = "tableOfRecentItems";

    let columnDefsHome = [
        {
            targets: 0,
            sortable: false,
            render: function (data, type, row) {
                return `<a href="/Item/Details/${row.id}">${row.title}</a>`;
            }
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
                })
                return res;
            }
        },
        {
            targets: 2,
            sortable: false,
            render: function (data, type, row) {
                return `<a href="/Collection/Details/${row.collectionId}">${row.collectionTitle}</a>`;
            }
        },
        {
            targets: 3,
            sortable: false,
            render: function (data, type, row) {
                return `<a href="/Collection/All?userId=${row.authorId}">${row.authorEmail}</a>`;
            }
        }
    ];
    let dataTableOptionsHome = {
        columnDefs: columnDefsHome,
        createdRow: function (row, data, dataIndex) {
            row.classList.add('bg-gray-opaque');
        },
        autoWidth: false,
        searching: false,
        sorting: false,
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Home/HereGetResentItems',
            type: 'POST',
            async: false,
            dataSrc: function (json) {
                return json.data;
            }
        }
    };

    $(document).ready(function () {
        $.ajax({
            type: "GET",
            url: "/Home/GetBiggestCollections",
            success: function (responce) {
                theBiggestCollections.innerHTML = responce;
            },
            error: function (error) {
                console.log(error);
            }
        });

        $("#" + tableId).DataTable(dataTableOptionsHome);
        let selectElement = document.querySelector('select[name="' + tableId + '_length"]');
        selectElement.classList.add('inherit-color');
        $(selectElement).find('option').addClass('inherit-color');
    });

})();