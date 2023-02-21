(function () {
    let commentText = document.getElementById('commentText');
    let sendCommentBtn = document.getElementById('sendCommentBtn');
    let itemId = document.getElementById('itemId').value;
    let comments = document.getElementById('comments');

    commentText.oninput = function () {
        if (commentText.value.length > 0) {
            sendCommentBtn.classList.remove('disabled');
        }
        else {
            sendCommentBtn.classList.add('disabled');
        }
    };

    window.onload = function () {
        loadComments();
    };

    sendCommentBtn.onclick = function () {
        $.ajax({
            type: "POST",
            url: "/Comment/Add",
            data: {
                itemId: itemId,
                text: commentText.value
            },
            success: function () {
                commentText.value = '';
                sendCommentBtn.classList.add('disabled');
                loadComments();
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

    loadComments = function loadComments() {
        $.ajax({
            type: "GET",
            url: "/Comment/GetAllCommentsForItem",
            data: {
                itemId: itemId
            },
            success: function (response) {
                comments.innerHTML = response;
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

    editComment = function editComment(commentId) {
        $.ajax({
            type: "GET",
            url: "/Comment/EditComment",
            data: {
                commentId: commentId,
                text: commentText.value
            },
            success: function (response) {
                if (response) {
                    loadComments();
                }
                // comments.innerHTML = response;
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

    deleteComment = function deleteComment(commentId) {
        $.ajax({
            type: "GET",
            url: "/Comment/DeleteComment",
            data: {
                commentId: commentId
            },
            success: function (response) {
                if (response) {
                    loadComments();
                }
                // comments.innerHTML = response;
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

})();
