$(function () {
    $(document).delegate('#createBtn', 'click',
        function () {
            $('#CreateArticleModal form').submit();
        });
});