$(function (){
    $(".editMessage").click(function () {
        $(".sentMessage").off("click");
        $(this).closest('tr').prev('tr').prev('tr').children('td:eq(1)').html("<input class = 'editInput'>");
        $(this).html("送出修改");
        $(".editMessage").off("click");
        $(this).removeClass("editMessage");
        $(this).addClass("sentMessage");

        $(".sentMessage").click(function () {
            var $editInput = $(this).closest('tr').prev('tr').prev('tr').children('td:eq(1)').children('input').val();
            console.log($editInput);

            var parameter = $(".sentMessage").next('button').attr('onclick');
            parameter = parameter.replace("DeleteMessage", "UpdateMessage");
            parameter = parameter.replace("';return false;", "&Content=" + $editInput + "';return false;");
            $(this).parent("td").append("<button class='ImFaker' style = 'display:none'>我是假的</button>");
            $(this).next('button').next('button').attr("onclick", parameter);
            $(".ImFaker").click();
        })
    })
})