DBTool = {};
$(document).ready(function () {
    //write your code here
    $('#backBtn').click(function () {
        DBTool.Redirect();
    });

});

$.extend(DBTool, {
    Redirect: function () {
        window.location.href = "index.aspx";
    }
});

