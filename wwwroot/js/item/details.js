(function () {
    let commentText = document.getElementById('commentText');
    let sendCommentBtn = document.getElementById('sendCommentBtn');
    let itemId = document.getElementById('itemId').value;
    let comments = document.getElementById('comments');
    let thumbUpBtn = document.getElementById('thumbUpBtn');
    let thumbDownBtn = document.getElementById('thumbDownBtn');
    let userId = document.getElementById('userId') != null ? document.getElementById('userId').value : null;

    window.onload = function () {
        loadComments();
        userGaveLike();

        if (thumbDownBtn != null) {
            thumbDownBtn.onclick = function () {
                thumbUpOrDown(false);
            }
        }

        if (thumbUpBtn != null) {
            thumbUpBtn.onclick = function () {
                thumbUpOrDown(true);
            }
        }

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

    const userGaveLike = function () {
        if (userId != null) {
            $.ajax({
                type: "GET",
                url: "/Item/FindLike",
                data: {
                    itemId: itemId,
                    userId: userId
                },
                success: function (response) {
                    console.log(response);
                    if (response == true) {
                        thumbUpBtn.classList.add('active');
                        thumbDownBtn.classList.remove('active');
                    } else if (response == false) {
                        thumbDownBtn.classList.add('active');
                        thumbUpBtn.classList.remove('active');
                    } else {
                        thumbDownBtn.classList.remove('active');
                        thumbUpBtn.classList.remove('active');
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    }

    const thumbUpOrDown = function (isUp) {
        let urlToCall = "/Item/" + (isUp ? "ThumbUp" : "ThumbDown");
        $.ajax({
            type: "POST",
            url: urlToCall,
            async: false,
            data: {
                itemId: itemId,
                userId: userId
            },
            success: function (response) {
                document.getElementById('likes').innerHTML = "Likes: " + response;
                userGaveLike();
            },
            error: function (error) {
                console.log(error);
            }
        });
    }

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

    // commentHub area
    (function () {
        "use strict";
        var connection = new signalR.HubConnectionBuilder().withUrl("/commentHub").build();
        //let itemId = document.getElementById("itemId").value;

        connection.on("NewComment_" + itemId, function () {
            loadComments();
        });
        connection.on("DeleteComment", function (commentId) {
            let toDelete = document.getElementById("delete_" + commentId);
            toDelete.remove();
        });
        connection.on("EditComment", function (commentId, newText) {
            console.log(document.getElementById("textOfComment_" + commentId).textContent);
            document.getElementById("textOfComment_" + commentId).textContent = newText;
        });

        connection.start();

    })();

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
