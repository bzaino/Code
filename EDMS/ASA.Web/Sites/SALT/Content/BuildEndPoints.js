var fs = require('fs'),
    xml2js = require('xml2js'),
    inDir = __dirname + process.argv[2],
    outDir = __dirname + process.argv[3];

var parser = new xml2js.Parser();
// Read xml from Web.config file
fs.readFile(inDir + '/web.config', function (err, data) {
    parser.parseString(data, function (err, result) {
        //parse appSettings section to JSON
        var output = JSON.stringify(result.configuration.appSettings);
        // build the JS to be used in the client-side code
        var outputFile = 'define([], function() { var endPoints = ' + output + '; return endPoints; });';
        fs.writeFileSync(outDir + 'EndPoints.js', outputFile);
    });
});