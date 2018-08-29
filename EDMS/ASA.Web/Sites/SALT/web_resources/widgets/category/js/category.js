$(document).ready(function(){
    $(".perc-category-list .perc-category-list-collapsible").each(function(){
        $(this).dynatree({
                minExpandLevel: 2 //Default expanded levels = 2
        });
    })
});