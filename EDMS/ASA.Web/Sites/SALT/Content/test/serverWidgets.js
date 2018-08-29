var assert = require("assert"),
    serverWidgets = require('./../serverWidgets'),
    sinon = require('sinon'),
    http = require('https'),
    dialogflow = require('dialogflow');

//globals
var currentYear = new Date().getFullYear();

describe('#ClassifyUser', function(){
    // Lower Classmen Segment Tests - Start
    it(':TC1: should classify user as Lower Classmen when EnrollmentStatus is FullTime, Max ExpectedGraduationYear is more than 4 years in the future"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 4)}, { ExpectedGraduationYear: (currentYear - 3)} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-LowerClass', userClassification);
    });

    it(':TC2: should classify user as Lower Classmen when EnrollmentStatus = "H", ExpectedGraduationYear = "' + (currentYear + 6) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 6)}];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-LowerClass', userClassification);
    });

    it(':TC3: should classify user as Lower Classmen when EnrollmentStatus = "H", Max ExpectedGraduationYear = "' + (currentYear + 6) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 6)}, { ExpectedGraduationYear: (currentYear - 2)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-LowerClass', userClassification);
    });

    it(':TC4: should classify user as Lower Classmen when EnrollmentStatus = "L", Max ExpectedGraduationYear = "' + (currentYear + 6) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 6)}, { ExpectedGraduationYear: (currentYear - 2)} ];
        var requestAttributes = {
            EnrollmentStatus: 'L',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-LowerClass', userClassification);
    });    
    // Lower Classmen Segment Tests - End

    // Upper Classmen Segment Tests - Start
    it(':TC5: should classify user as Upper Classmen when EnrollmentStatus = "F", ExpectedGraduationYear = "' + (currentYear) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: currentYear} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });

    it(':TC6: should classify user as Upper Classmen when EnrollmentStatus = "H", ExpectedGraduationYear = "' + (currentYear + 2) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 2)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });

    it(':TC7: should classify user as Upper Classmen when EnrollmentStatus = "F", Max ExpectedGraduationYear = "' + (currentYear ) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear)}, { ExpectedGraduationYear: (currentYear -3)} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });

    it(':TC8: should classify user as Upper Classmen when EnrollmentStatus = "H", Max ExpectedGraduationYear = "' + (currentYear) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear)}, { ExpectedGraduationYear: (currentYear -3)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });

    it(':TC9: should classify user as Upper Classmen when EnrollmentStatus = "L", Max ExpectedGraduationYear = "' + (currentYear) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear)}, { ExpectedGraduationYear: (currentYear -3)} ];
        var requestAttributes = {
            EnrollmentStatus: 'L',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });    
    it(':TC33: should classify user as Upper Classmen when EnrollmentStatus = null, ExpectedGraduationYear = "' + (currentYear + 3) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear + 3)} ];
        var requestAttributes = {
            EnrollmentStatus: '',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClass', userClassification);
    });        
    // Upper Classmen Segment Tests - End

    // Upper Classmen/Alumni Mix Segment Tests - Start
    it(':TC10: should classify user as Upper Classmen/Alumni Mix when EnrollmentStatus = "F", ExpectedGraduationYear = "' + (currentYear -3) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 3)} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClassAlumniMix', userClassification);
    });

    it(':TC11: should classify user as Upper Classmen/Alumni Mix when EnrollmentStatus = "H", ExpectedGraduationYear = "' + (currentYear -7) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 7)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClassAlumniMix', userClassification);
    });

    it(':TC12: should classify user as Upper Classmen/Alumni Mix when EnrollmentStatus = "L", ExpectedGraduationYear = "' + (currentYear - 7) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 7)} ];
        var requestAttributes = {
            EnrollmentStatus: 'L',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClassAlumniMix', userClassification);
    });

    it(':TC13: should classify user as Upper Classmen/Alumni Mix when EnrollmentStatus = "H", Max ExpectedGraduationYear = "' + (currentYear -7) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 14)}, { ExpectedGraduationYear: (currentYear - 7)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClassAlumniMix', userClassification);
    });

    it(':TC14: should classify user as Upper Classmen/Alumni Mix when EnrollmentStatus = "A", ExpectedGraduationYear = "' + (currentYear) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear)} ];
        var requestAttributes = {
            EnrollmentStatus: 'A',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-UpperClassAlumniMix', userClassification);
    });
    // Upper Classmen/Alumni Mix Segment Tests - End


    // Alumni Segment Tests - Start
    it(':TC15: should classify user as Alumni when EnrollmentStatus = "F", ExpectedGraduationYear = "' + (currentYear - 11) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 11)} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC16: should classify user as Alumni when EnrollmentStatus = "H", ExpectedGraduationYear = "' + (currentYear - 17) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 17)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC17: should classify user as Alumni when EnrollmentStatus = "F", Max ExpectedGraduationYear = "' + (currentYear - 9) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 9)}, { ExpectedGraduationYear: (currentYear - 16)} ];
        var requestAttributes = {
            EnrollmentStatus: 'F',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC18: should classify user as Alumni when EnrollmentStatus = "H", ExpectedGraduationYear = "' + (currentYear - 12) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 12)}, { ExpectedGraduationYear: (currentYear - 16)} ];
        var requestAttributes = {
            EnrollmentStatus: 'H',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC19: should classify user as Alumni when EnrollmentStatus = "L", ExpectedGraduationYear = "' + (currentYear - 12) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 12)}, { ExpectedGraduationYear: (currentYear - 16)} ];
        var requestAttributes = {
            EnrollmentStatus: 'L',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC20: should classify user as Alumni when EnrollmentStatus = "G", Max ExpectedGraduationYear = "' + (currentYear - 15) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 15)}, { ExpectedGraduationYear: (currentYear - 14)} ];
        var requestAttributes = {
            EnrollmentStatus: 'G',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC21: should classify user as Alumni when EnrollmentStatus = "G", Max ExpectedGraduationYear = "' + (currentYear - 5) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 5)}, { ExpectedGraduationYear: (currentYear - 16)} ];
        var requestAttributes = {
            EnrollmentStatus: 'G',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });

    it(':TC22: should classify user as Alumni when EnrollmentStatus = "null", max ExpectedGraduationYear = "' + (currentYear - 8) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear - 15)}, { ExpectedGraduationYear: (currentYear -8)} ];
        var requestAttributes = {
            EnrollmentStatus: null,
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });    

    it(':TC23: should classify user as Alumni when EnrollmentStatus = "Z", ExpectedGraduationYear = "' + (currentYear - 7) +'"', function(){
        var orgArray = [ { ExpectedGraduationYear: (currentYear -7)} ];
        var requestAttributes = {
            EnrollmentStatus: 'Z',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Alumni', userClassification);
    });    
    // Alumni Segment Tests - End

     // Unknown Segment Tests - Start
    it(':TC24: should classify user as Unknown when EnrollmentStatus = "", ExpectedGraduationYear = "null"', function(){
        var orgArray = [ { ExpectedGraduationYear: null} ];
        var requestAttributes = {
            EnrollmentStatus: '',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Default', userClassification);
    });

    it(':TC25: should classify user as Unknown when EnrollmentStatus = "N", ExpectedGraduationYear = "null"', function(){
        var orgArray = [ ];
        var requestAttributes = {
            EnrollmentStatus: 'N',
            Organizations: orgArray
        },
        userClassification = serverWidgets.ClassifyUser(requestAttributes);
        assert.equal('UserClass-Default', userClassification);
    });
    // Unknown Segment Tests - End
});

