#!/usr/bin/env node

var fs = require('fs'),
    path = require('path'),
    dust = require('dustjs-linkedin'),
    rmdir = require('rimraf'),
    mkdirp = require('mkdirp'),
    inpDir = __dirname + process.argv[2],
    outDir = __dirname + process.argv[3];

//If optional argument passed, initialize path and clear file
if (process.argv[4]) {
    var destFile = __dirname + process.argv[4];
    if (fs.existsSync(destFile)) {
        fs.unlinkSync(destFile);
    }
}

function createPathMap(dirPath, pathMap) {
    var files = fs.readdirSync(dirPath);
    if (files.length) {
        files.forEach(function (filename) {
            var filePath = dirPath + '/' + filename;
            if (fs.statSync(filePath).isFile()) {
                pathMap[path.basename(filename, '.dust')] = filePath.substring(filePath.indexOf('templates') + 10).slice(0, -5);
            } else {
                if (filename.toLowerCase() !== 'compiled') {
                    createPathMap(filePath, pathMap);
                }
            }
        });
    }
    return pathMap;
}

function traceDependencies(filename, out) {
    // creates a list of file names and their paths
    var pathMap = createPathMap(inpDir, {});
    //Compile file with filename of template, cutting off dust extension
    var data = dust.compile(out, path.basename(filename, '.dust'));
    var indexes = ['dust'];
    var pos = 0;
    // check if the compiled dust file is pulling in any other partials, and add their names to "indexes" array
    while (data.indexOf('.partial("', pos) > -1) {
        if (data.indexOf('.partial("', pos) === pos) {
            var closingQuoteIndex = data.indexOf('"', pos + 10);
            var currentName = data.substring(pos + 10, closingQuoteIndex);
            if (currentName.slice(-5) === '.dust') {
                currentName = currentName.slice(0, -5);
            }
            // return the path associated with this file name
            if (currentName.indexOf('/')) {
                currentName = currentName.substr(currentName.indexOf('/') + 1);
            }
            var partial = 'Compiled' + pathMap[currentName];
            indexes.push(partial);
        }
        pos++;
    }

    if (filename === 'ToolBody.dust') {
        var tools = fs.readdirSync(inpDir + '/Tools');
        tools.forEach(function (tool) {
            indexes.push('Compiled/Tools/' + path.basename(tool, '.dust'));
        });
    }
    return indexes;
}

//Loop through input directory, for each file compile dust and write to identical file in output directory
//If optional destination file was passed, append compiled output of each file to it

function walkInputDir(target, folderName) {
    fs.readdir(target, function (err, files) {
        if (err) {
            console.log(err);
            throw err;
        }
        files.forEach(function (filename) {
            var stats = fs.lstatSync(target + filename);
            if (!stats.isDirectory()) {
                var out = fs.readFileSync(target + filename, 'utf-8'),
                    indexes = traceDependencies(filename, out),
                    nameToRegister = path.basename(filename, '.dust'),
                    parentDirectory = path.dirname(target + filename).substr(path.dirname(target + filename).lastIndexOf('/') + 1);

                if (parentDirectory !== 'templates') {
                    nameToRegister = parentDirectory + '/' + nameToRegister;
                }

                var output = 'define([' + '"' + indexes.join('", "') + '"' + '], function(dust) { ' + dust.compile(out, nameToRegister) + ' });';

                if (folderName && folderName.toLowerCase() !== 'compiled') {
                    mkdirp(outDir + folderName, function (error) {
                        if (error) {
                            console.log(error);
                            throw error;
                        }
                        fs.writeFile(outDir + folderName + '/' + path.basename(filename, '.dust') + '.js', output);
                    });
                } else {
                    fs.writeFile(outDir + path.basename(filename, '.dust') + '.js', output);
                }
                if (destFile) {
                    fs.appendFileSync(destFile, output);
                }
            } else {
                if (filename.toLowerCase() !== 'compiled') {
                    walkInputDir(target + filename + '/', filename);
                }
            }
        });
    });
}

rmdir(outDir, function (err) {
    if (err) {
        console.log(err);
        throw err;
    }
    // creates a folder with the value passed in outDir
    mkdirp(outDir, function (failure) {
        if (failure) {
            console.log(failure);
            throw failure;
        }
        walkInputDir(inpDir);
    });
});
