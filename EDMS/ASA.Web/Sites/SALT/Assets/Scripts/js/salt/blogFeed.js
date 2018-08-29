/*!
 * SALT Blog Feed Widget
 * 
 * Feed Formats Supported: Atom
 */
/// <reference path="jquery-1.5.1.js" />

define(['module', 'jquery', 'dust'], function (module, $) {
    var blogFeed = {
        _create: function () {
            var self = this,
                templates = $('script[type="text/x-dust-template"]');
					
            templates.each(function () {
                var template = $(this),
                    name = template.data('name');
                if (name) {
					var html = template.html();
					var convertedText = html.replace(/[\n\r]+\-/g, "");
                    dust.loadSource(dust.compile(convertedText, name));
                } else {
                    if (console && $.isFunction(console.warn)) {
                        console.warn('Template found, but no name specified; skipping.', template);
                    }
                }
                
                template.remove();
            });
            
            this.feedUri = "http://blog.saltmoney.org/atom";
            this.feedCount = 10;
            
            this.refresh();
        },
        
        _getFeed: function (callback) {
            var self = this;
            
            $.ajax({
                url: (window.location.protocol === 'file:' ? 'http:' : '') + '//ajax.googleapis.com/ajax/services/feed/load?v=1.0&num=' + this.feedCount + '&output=xml&q=' + encodeURIComponent(this.feedUri) + '&callback=?',
                dataType: 'json',
                success: function (data) {
                    if ($.isFunction(callback)) {
						var jsonstr = JSON.stringify(data);
						var new_jsonstr = jsonstr.replace(/title/g, 'titleTag');
						var new_obj = JSON.parse(new_jsonstr);

                        callback.call(self, $(new_obj.responseData.xmlString));
                    }
                }
            });
        },
        
        refresh: function () {
            var self = this;
            
            this._getFeed(function (feed) {
				
                var data = {
                        feed: {
                            logo: feed.find('asa\\:feedLogo').first().text(),
                            icon: feed.find('asa\\:feedIcon').first().text(),
                            title: feed.find('titleTag[type=text]').first().text(),
                            uri: feed.find('link[rel=alternate][type="text/html"]').first().attr('href')
                        },
                        entries: []
                    },
                    entries = feed.find('entry');

                entries.each(function () {
                    var entry = $(this),
                        author = entry.find('author'),
                        thumbnail = entry.find('asa\\:thumbnail');
                    
                    // TODO: Add support for RSS
                    data.entries.push({						
                        title: $('<textarea />').html(entry.find('titleTag').text()).val(), // evaluate HTML entities
						permalink: entry.find('link[rel=alternate][type="text/html"]').attr('href'),
                        publishedDate: new Date(entry.find('published').text()),
                        author: {
                            name: author.find('name').text(),
                            uri: author.find('uri').text(),
                            title: entry.find('asa\\:authorTitle').text()
                        },
                        thumbnail: {
                            uri: thumbnail.find('asa\\:uri').text(),
                            width: thumbnail.find('asa\\:width').text(),
                            height: thumbnail.find('asa\\:height').text()
                        }
						
                    });
                });
                dust.render('feed', data, function (err, out) {
                    if (err) {
                        if (console && $.isFunction(console.error)) {
                            console.error(err.message);
                        }
                    }
                    
                    $('#blogfeed').html(out);
                });
            });
        }
    };

    return blogFeed._create();
});