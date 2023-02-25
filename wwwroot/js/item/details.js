(function () {
    let commentText = document.getElementById('commentText');
    let sendCommentBtn = document.getElementById('sendCommentBtn');
    let itemId = document.getElementById('itemId').value;
    let comments = document.getElementById('comments');


    window.onload = function () {
        loadComments();

        if (commentText != null) {
            commentText.oninput = function () {
                if (commentText.value.length > 0) {
                    sendCommentBtn.classList.remove('disabled');
                }
                else {
                    sendCommentBtn.classList.add('disabled');
                }
            };
        }

        if (sendCommentBtn != null) {
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
        }
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
        // TODO: logic should be corrected
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
            },
            error: function (error) {
                console.log(error);
            }
        });
    };

})();


// TODO: async comments loading, when the page is scrolled down to the bottom
/*
 // Global variables to keep track of the comments loaded so far
var loadedComments = 0;
var totalComments = 0;

$(document).ready(function() {
  // Load the first 10 comments when the page loads
  loadComments(10);
  
  // Bind a scroll event to the window
  $(window).scroll(function() {
    // Check if the user has scrolled to the bottom of the page
    if($(window).scrollTop() + $(window).height() == $(document).height()) {
      // Check if there are more comments to load
      if(loadedComments < totalComments) {
        // Load another 10 comments
        loadComments(10);
      }
    }
  });
});

function loadComments(count) {
  // Make an AJAX call to the server-side method to load the comments
  $.ajax({
    type: "POST",
    url: "/GetAllCommentsForItem",
    data: { itemId: itemId, count: count, loadedComments: loadedComments },
    success: function(result) {
      // Update the totalComments and loadedComments variables
      totalComments = result.totalCount;
      loadedComments += result.comments.length;
      
      // Append the new comments to the comments container
      $.each(result.comments, function(index, comment) {
        $("#comments-container").append("<div>" + comment.text + "</div>");
      });
    }
  });
}

 */