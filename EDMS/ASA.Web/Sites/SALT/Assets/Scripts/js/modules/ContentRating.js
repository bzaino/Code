require([
    'jquery',
    'modules/ContentInteractionModels',
    'salt/models/SiteMember',
    'backbone',
    'salt',
    'modules/StarRatingWidget',
    'jquery.serializeObject',
    'asa/ASAWebService'
], function ($, Interactions, SiteMember, Backbone, SALT) {

    var contentId = '',
        rating;

    function generateRatingUI() {
        $('#UserRating').ratings(5, rating.get('MemberContentInteractionValue'), true).on('ratingchanged', function (event, data) {
            rating.set('MemberContentInteractionValue', data.rating);
            //TODO fix this to use model.save rather than backbone sync
            //It is currently using backbone sync because we are defaulting our models to have an id, so it sends save as a put rather than post, override this
            Backbone.sync('create', rating, {
                url: rating.urlRoot,
                success: function (savedRating) {
                    rating.set('MemberContentInteractionID', savedRating.MemberContentInteractionID);
                }
            });

            //show feedback box.
            $('#js-rating-email-send').slideDown();
        });
    }

    //Make sure to fill in the memberID when we get back data from the server
    //We dont want to try to do any posting/fetching until we have member data
    function setupRating() {
        contentId = $('.js-todoContainer').attr('data-primary-key');
        if (contentId) {
            rating = new Interactions.Interaction({ ContentID: contentId });
            SiteMember.done(function (member) {
                if (member.MembershipId) {
                    rating.set('MemberID', parseInt(member.MembershipId, 10));
                    rating.set('FromEmailAddress', member.Emails[0].EmailAddress);
                    rating.fetch({url: rating.urlRoot + '/' + rating.get('MemberID') + '/' + rating.get('ContentID') + '/1', success: generateRatingUI });
                } else {
                    //Not logged in, generate widget with 0 stars filled and bind a click event to open the login overlay
                    $('#UserRating').ratings(5, 0, false).on('click', function () {
                        SALT.trigger('open:customOverlay', 'loginOverlay');
                    });
                }
            });
            bindEventsForFeedBack();
        }
    }

    //email feedback boxes click event binding function
    function bindEventsForFeedBack() {
        $('.js-rating-box-close').click(function (e) {
            e.preventDefault();
            $(e.currentTarget).closest('.js-box-feedback').slideUp();
        });
        $('#js-rating-form').on('valid', function (e) {
            e.preventDefault();
            var formObject = $(e.currentTarget).serializeObject(),
                jsonObject = {
                    emailBody: formObject.feedbackBody,
                    emailSubject: 'Content Feedback: ' + document.title,
                    ratingVal: rating.get('MemberContentInteractionValue'),
                    FromEmailAddress: rating.get('FromEmailAddress'),
                    contentID: $('.js-todoContainer').attr('data-primary-key'),
                    memberID: rating.get('MemberID')
                };
            SALT.services.contentFeedbackEmail(JSON.stringify(jsonObject), feedbackSent, this);

            rating.set('MemberContentComment', formObject.feedbackBody);
            rating.save();

        });
    }

    function feedbackSent() {
        //on success callback, slide up the email box, slide down thanks box.
        $('#js-rating-email-send').slideUp();
        //show thanks box, and using setTimeOut to dismiss automatically after 3 sec.
        $('#js-rating-email-confirm').slideDown(400, function () {
            setTimeout(function () {
                $('#js-rating-email-confirm').slideUp();
            }, 3000);
        });
    }

    $(function () {
        setupRating();
    });
});
