define([
    'jquery'
], function ($) {

    var wtTags = {
        // Standard
        ONSITE_PROMOTION_CLICK:            'WT.ac',
        ONSITE_PROMOTION_VIEW:             'WT.ad',
        CONTENT_GROUP:                     'WT.cg_n',
        CONTENT_SUBGROUP:                  'WT.cg_s',
        EXTERNAL_VISITOR_ID:               'WT.dcsvid',
        SERVER_CALL_IDENTIFIER:            'WT.dl',
        PAGE_VIEW_IDENTIFIER:              'WT.dl',
        EVENT_SOURCE:                      'WT.es',
        CAMPAIGN_ID:                       'WT.mc_id',
        ONSITE_SEARCH_WORD:                'WT.oss',
        SEARCH_PAGE_NUMBER:                'WT.oss_p',
        NUM_SEARCH_RESULTS:                'WT.oss_r',
        REGISTERED_VISITOR:                'WT.rv',
        SITE:                              'WT.site',
        SCENARIO_ANALYSIS_CONVERSION_STEP: 'WT.si_cs',
        SCENARIO_ANALYSIS_NAME:            'WT.si_n',
        SCENARIO_ANALYSIS_STEP_NUM:        'WT.si_x',
        PAGE_TITLE:                        'WT.ti',
        VIDEO_NAME:                        'WT.clip_n',
        VIDEO_PERCENT_COMPLETE:            'WT.clip_ev',
        PRODUCT_SKU:                       'WT.pn_sku',
        TRANSACTION_EVENT:                 'WT.tx_e',

        // SmartSource Data Collector
        // Override Parameters
        // Parameter names from: http://kb.webtrends.com/support/solutiondetail.aspx?Id=50140000000a92kAAA
        DCS_REFERRER:               'DCS.dcsref',
        DCS_DOMAIN:                 'DCS.dcssip',
        DCS_URI_STEM:               'DCS.dcsuri',
        DCS_PROTOCOL:               'DCS.dcspro',
        DCS_URI_QUERY:              'DCS.dcsqry',
        DCS_AUTHENTICATED_USERNAME: 'DCS.dcsaut',
        DCS_METHOD:                 'DCS.dcsmet',
        DCS_STATUS:                 'DCS.dcssta',
        DCS_BYTES:                  'DCS.dcsbyt',
        DCS_IP_ADDRESS:             'DCS.dcscip',
        DCS_USER_AGENT:             'DCS.dcsua',

        // Custom
        TYPE:                                'WT.z_type',
        ACTIVITY_NAME:                       'WT.z_actname',
        ACTIVITY_SUB_NAME:                   'WT.z_actsub',
        ACTIVITY_SUB_NAME_2:                 'WT.z_actsub2',
        ACTIVITY_TYPE:                       'WT.z_acttype',
        ACTIVITY_TRANSACTION:                'WT.z_acttx',
        ACTIVITY_ACTION:                     'WT.z_action',
        STATE:                               'WT.z_state',
        CAROUSEL_NAME:                       'WT.z_cn',
        CAROUSEL_ROTATE:                     'WT.z_cr',
        CAROUSEL_SELECT:                     'WT.z_cs',
        LESSON_DATA_RESTORED:                'WT.z_lrstrd',
        SCENARIO_ANALYSIS_STEP_PREPOPULATED: 'WT.z_si_p',
        CONTENT_GROUP_TYPE:                  'WT.z_cg_t',
        CG_TYPE:                             'WT.z_cg_type',
        AUTHOR_NAME:                         'WT.z_author',
        REG_WALL:                            'WT.z_Wall'
    };

    var WT = {

        IIC_FIRED_KEY: 'initialInputChange-fired',

        resetInitialInputChange: function (obj) {
            $(obj).data(this.IIC_FIRED_KEY, false);
        },

        initialInputChange: function (obj, handler) {
            var FIRED_KEY = this.IIC_FIRED_KEY,
                PREV_VAL_KEY = 'initialInputChange-prevVal';

            var onEvent = ($(obj).is('select') ? 'change' : 'keyup');
            var onEventHandler = function (eventObject) {
                var fired = !!$(this).data(FIRED_KEY);
                var prevVal = $(this).data(PREV_VAL_KEY) || '';
                var inputChanged = (prevVal !== $(this).val());

                if (inputChanged) {
                    if (!fired) {
                        if (typeof handler === 'function') {
                            handler.call($(this), eventObject);
                        }

                        $(this).data(FIRED_KEY, true);
                    }

                    $(this).data(PREV_VAL_KEY, $(this).val());
                }
            };

            if (typeof obj === 'string') {
                $(document).on(onEvent, obj, onEventHandler);
            } else {
                $(obj).bind(onEvent, onEventHandler);
            }
        },

        tags : wtTags,

        SERVER_CALL_IDENTIFIERS : {
            STANDARD_PAGE_VIEW: '0',
            DOWNLOAD:           '20',
            MAILTO:             '23',
            OFFSITE_LINK:       '24',
            SALT_CUSTOM_EVENT:  '90',
            NAMED_ANCHOR:       '99'
        },

        USER_STATE_IDENTIFIERS : {
            LOGGED_OUT:          '00',
            LOGGED_IN_UNMATCHED: '01',
            LOGGED_IN_MATCHED:   '11'
        },

        ID_TAGS  : [
            wtTags.DCS_AUTHENTICATED_USERNAME,
            wtTags.EXTERNAL_VISITOR_ID
        ],

        // Noncumulative (aka "Normally Empty") Tags
        NORMALLY_EMPTY_TAGS : [
            wtTags.SCENARIO_ANALYSIS_CONVERSION_STEP,
            wtTags.SCENARIO_ANALYSIS_NAME,
            wtTags.SCENARIO_ANALYSIS_STEP_NUM,
            wtTags.ACTIVITY_NAME,
            wtTags.ACTIVITY_TYPE,
            wtTags.ACTIVITY_TRANSACTION,
            wtTags.TRANSACTION_EVENT,
            wtTags.CONTENT_GROUP_TYPE,
            wtTags.CG_TYPE,
            wtTags.AUTHOR_NAME
        ],

        getTagIndex: function (tags, tagName) {
            var tagIndex = -1;

            for (var i = 0; tagIndex === -1 && i < tags.length; i += 2) {
                if (tags[i] === tagName) {
                    tagIndex = i;
                }
            }

            return tagIndex;
        },

        // adds (or replaces) a tag to a list of tags
        setTag: function (tags, tagName, tagValue) {
            if (!tags) {
                tags = [];
            }

            var tagIndex = this.getTagIndex(tags, tagName);

            if (tagIndex === -1) {
                tags.push(tagName);
                tags.push(tagValue);
            } else {
                tags[tagIndex + 1] = tagValue;
            }

            return tags;
        }
    };

    return WT;
});

