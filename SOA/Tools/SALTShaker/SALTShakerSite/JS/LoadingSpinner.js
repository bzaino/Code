/* File Created: April 3, 2014 */
var DBTool = DBTool || {};
//attach click event for Member List tab on document ready so 
//that it pops spinner right from the begnning the user clicks it
$(document).ready(function() {
    $('.table_tabs ul li:nth-child(2)').click(function () {
        DBTool.Spinner.show();
    });
});

DBTool.Spinner = {
    hide: function () {
        $(".modal").remove();
        $(".loading").hide();
    },

    show: function () {
            var modal = $('<div />');
            modal.addClass("modal");
            $('body').append(modal);
            var loading = $(".loading");
            loading.show();
            var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
            var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
            loading.css({ top: top, left: left });
    }
};