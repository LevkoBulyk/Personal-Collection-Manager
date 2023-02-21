// set up global variables for tracking pagination
let currentPage = 1;
let isEndOfList = false;

// load the first page of items
loadNexItems();
/*window.onload(function () {
    while ($(document).height() <= $(window).height()) {
        // if the page is not filled (user cannot scroll), load the next page of items
        if (!isEndOfList) {
            loadNexItems();
        }
    }
});*/

let timeout;
// add a scroll listener to the window
$(window).scroll(function () {
    clearTimeout(timeout);
    timeout = setTimeout(function () {
        // check if the user has scrolled to the bottom of the page
        if ($(window).scrollTop() + 1 >= $(document).height() - $(window).height()) {
            // if we haven't reached the end of the list yet, load the next page of items
            if (!isEndOfList) {
                loadNexItems();
            }
        }
    }, 50);
});

function loadNexItems() {
    let modelId = document.getElementById('modelId').value;
    $.ajax({
        url: '/Item/GetItemsList',
        type: 'GET',
        data: { collectionId: modelId, pageNumber: currentPage },
        success: function (result) {
            // append the items to the table
            $('#bodyOfItemsTable').append(result);
            currentPage++;
            // if the result is empty, we've reached the end of the list
            if (result.trim() == '') {
                isEndOfList = true;
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
