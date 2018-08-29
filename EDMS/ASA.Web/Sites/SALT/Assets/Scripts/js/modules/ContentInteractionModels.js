define([
    'backbone',
    'underscore'
], function (Backbone, _) {

    //TODO our API currently has two different ids for different operations, we should decide on contentID or MemberContentInteractionID
    var ContentInteraction  = Backbone.Model.extend({
        idAttribute: 'MemberContentInteractionID',
        defaults : {
            ContentID: '',
            InteractionDate: '\/Date(928164000000-0400)\/',
            MemberContentInteractionID: 0,
            MemberContentInteractionValue: 0,
            MemberID: 0,
            RefContentInteractionTypeID: 1,
            MemberContentComment: ''
        },
        urlRoot: '/api/ContentService/ContentInteraction',
        parse: function (response) {
            return _.where(response, { RefContentInteractionTypeID: 1 })[0];
        }
    });

    var InteractionCollection = Backbone.Collection.extend({
        model: ContentInteraction,
        url: '/api/ContentService/ContentInteraction',
        parse: function (response, options) {
            return response;
        }
    });

    return {
        Interaction: ContentInteraction,
        InteractionCollection: InteractionCollection
    };
});
