$(function () {
    $(document).delegate('#EditArticleModal #editBtn', 'click',
        function () {
            $('#EditArticleModal form').submit();
        });

    $('#createMessage').click(function () {
        setTimeout(function () {
            Location.reload();
        }, 200);
    });
});