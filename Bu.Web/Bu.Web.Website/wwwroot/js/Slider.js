$("#mainSlideShow > div:gt(0)").hide();

setInterval(function () {

    $('#mainSlideShow > div').css('right', '0%');
    $('#mainSlideShow > div').css('left', '0%');
    $('#mainSlideShow > div:first')
        .fadeOut(0)
        .next()
        .fadeIn(300)
        .end()
        .appendTo('#mainSlideShow');
}, 4000);

$("#slideshow > div:gt(0)").hide();

setInterval(function () {

    $('#slideshow > div').css('right', '0%');
    $('#slideshow > div').css('left', '0%');
    $('#slideshow > div:first')
        .fadeOut(500)
        .next()
        .fadeIn(500)
        .end()
        .appendTo('#slideshow');
}, 4000);
console.log($('#ActivitiesSlideshow'));
if ($('#ActivitiesSlideshow')) {
    $("#ActivitiesSlideshow > div:gt(0)").hide();

    setInterval(function () {

        $('#ActivitiesSlideshow > div').css('right', '0%');
        $('#ActivitiesSlideshow > div').css('left', '0%');
        $('#ActivitiesSlideshow > div:first')
            .fadeOut(0)
            .next()
            .fadeIn(300)
            .end()
            .appendTo('#ActivitiesSlideshow');
    }, 4000);
}

function nextSlide(id) {
    $('#'+id+' > div').css('right', '0%');
    $('#'+id+' > div').css('left', '0%');
    $('#'+id+' > div:first')
        .fadeOut(0)
        .next()
        .fadeIn(300)
        .end()
        .appendTo('#'+id);
}

function previousSlide(id) {
    $('#'+id+' > div').css('right', '0%');
    $('#'+id+' > div').css('left', '0%');
    $('#'+id+' > div:first')
        .fadeOut(0).end();
    $('#'+id+' > div:last')
        .fadeIn(300).prependTo('#'+id);

}