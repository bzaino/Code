$(document).foundation();
$.stellar({
// Set scrolling to be in either one or both directions
horizontalScrolling: true,
verticalScrolling: true,

// Set the global alignment offsets
horizontalOffset: 0,
verticalOffset: 0,

// Refreshes parallax content on window load and resize
responsive: false,

// Select which property is used to calculate scroll.
// Choose 'scroll', 'position', 'margin' or 'transform',
// or write your own 'scrollProperty' plugin.
scrollProperty: 'scroll',

// Select which property is used to position elements.
// Choose between 'position' or 'transform',
// or write your own 'positionProperty' plugin.
positionProperty: 'position',

// Enable or disable the two types of parallax
parallaxBackgrounds: true,
parallaxElements: true,

// Hide parallax elements that move outside the viewport
hideDistantElements: true,

// Customise how elements are shown and hidden
hideElement: function($elem) { $elem.hide(); },
showElement: function($elem) { $elem.show(); }
});

$.stellar.positionProperty.scale = {
sized: function($element, newSize, originalTop) {
$element.css('scale', newSize);
}
};
