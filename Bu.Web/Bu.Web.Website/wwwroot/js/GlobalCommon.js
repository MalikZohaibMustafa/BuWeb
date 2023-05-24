$('#basic-layout-container').scroll(function (e) {
;   var $el = $('.fixedToTop');
    var isPositionFixed = ($el.css('position') == 'fixed');
    if ($(this).scrollTop() > 100 && !isPositionFixed) {
        $el.css({ 'position': 'fixed', 'top': '0px', 'width': '100%', 'z-index': '10000' });
    }
    if ($(this).scrollTop() < 100 && isPositionFixed) {
        $el.css({ 'position': 'inherit', 'top': '0px' });
    }
});