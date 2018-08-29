/*
 * tinyLightbox original animation extension 2.4 - Plugin for jQuery
 * 
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Depends:
 *  jquery.js
 *    jquery.tinyLightbox.js
 * 
 *
 *  Copyright (c) 2008 Oleg Slobodskoi (ajaxsoft.de)
 */
;(function($) {

$.fn.tinyLightbox.original = function( inst ) {
    
    return {
        start: function( callback ) {
            inst.$overlay.animate({opacity: 'show' }, inst.s.speed, function(){
                inst.$box.css({
                    visibility: 'visible',
                    left: inst.boxData.left,
                    top: inst.boxData.top
                });
                callback();
            });            
        },
    
        animate: function( callback ) {
            inst.$box.animate({height: inst.boxData.height}, inst.s.speed, function (){
                $(this).animate(
                    { 
                        width: inst.boxData.width, 
                        left: inst.boxData.left
                    }, 
                    inst.s.speed, 
                    function() {
                        // show image
                        inst.$image.css('background-image','url('+inst.path+')')    
                        .fadeIn(inst.s.speed, function(){
                            inst.$bar.css({
                                top: inst.boxData.top+inst.boxData.height+inst.boxData.borderWidth*2,
                                left: inst.boxData.left,
                                width: inst.boxData.width
                            }).slideDown(inst.s.speed, callback);
                        });            
                    }
                );
            });
        },
    
        prepare: function( callback ) {
            inst.$bar.slideUp(inst.s.speed, function(){
                inst.$image.fadeOut(inst.s.speed, callback);
            });
        },
    
        limit: function( callback )    {
            shake(4, inst.s.speed/2, 40, '+', 0);
            function shake (times, speed, distance, dir, timesNow) {
                timesNow++;
                dir = dir=='+' ? '-' : '+';
                inst.$bar.hide();
                inst.$box.animate({left: dir+'='+distance}, speed, function(){
                    timesNow < times ? shake(times, speed, distance, dir, timesNow) : inst.$bar.show() && callback();
                });
            };
        },
                    
        close: function ( callback ) {
            this.prepare(function(){
                inst.$box.fadeOut(inst.s.speed, function(){
                    inst.$overlay.fadeOut(inst.s.speed, callback);
                });    
            });
        }
        
    };
};


})( jQuery );