/*global module*/
/*
 * grunt
 * http://gruntjs.com/
 *
 * Copyright (c) 2013 "Cowboy" Ben Alman
 * Licensed under the MIT license.
 * https://github.com/gruntjs/grunt/blob/master/LICENSE-MIT
 */

'use strict';

module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    watch: {
      scripts: {
        files: ['<%= jshint.all %>'],
        tasks: ['jshint', 'buildMain', 'test']
      },
        sass: {
            files: ['<%= sass.all %>'],
            tasks: ['sassLocal']
        },
      dust: {
        files: ['../../ASA.Web/Sites/SALT/Assets/templates/**/*.dust'],
        tasks: ['DustCompile', 'buildMain']
      },
      testSpecs: {
        files: [
          '../../ASA.Web/Sites/SALT/JSTestFramework/Specs/*.js',
          '../../ASA.Web/Sites/SALT/Content/test/*.js'
        ],
        tasks: ['test']
      }
    },
    jshint: {
      all: [
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/ASA/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/modules/**/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/salt/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/salt/analytics/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/salt/global/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/salt/models/*.js',
        '../../ASA.Web/Sites/SALT/Assets/Scripts/js/tools/*.js',
        '../../ASA.Web/Sites/SALT/Content/*.js'
      ],
      options: {
          curly: true,
          eqeqeq: true,
          immed: true,
          latedef: true,
          newcap: true,
          noarg: true,
          sub: true,
          undef: false,
          boss: true,
          eqnull: true,
          browser: true,
          evil: true,
          white: true,
          maxdepth: 5,
          maxstatements: 25,
          maxerr: 15,
          ignores: ['../../ASA.Web/Sites/SALT/Assets/Scripts/js/require.js', '../../ASA.Web/Sites/SALT/Assets/Scripts/js/json.js', '../../ASA.Web/Sites/SALT/Assets/Scripts/js/modules/StarRatingWidget.js']
      }
    },
    concat: {
        foundationPlugins: {
            src: [
                '../../ASA.Web/Sites/SALT/Assets/Scripts/js/libs/foundation5/ReferencePlugins/*.js'
            ],
            dest: '../../ASA.Web/Sites/SALT/Assets/Scripts/js/libs/foundation5/foundation.plugins.js'
        }
    },
    exec: {
      compileDust: {
        cmd: function() {
          return 'node ../../ASA.Web/Sites/SALT/Content/DustjsCompile.js /../Assets/templates/ /../Assets/templates/Compiled/';
        }
      },
      testMocha: {
        cmd: function() {
          return 'mocha-phantomjs -R spec -s webSecurityEnabled=false -p phantomjs.exe ../../ASA.Web/Sites/SALT/JSTestFramework/BuildSpec.html';
        }
      },
      testServerJavaScript: {
        cmd: function () {
          return '"../../ASA.Web/Sites/SALT/Content/node_modules/.bin/mocha" ../../ASA.Web/Sites/SALT/Content/test -R tap';
        }
      },
      coverageServerJavaScript: {
        cmd: function () {
          return '"../../ASA.Web/Sites/SALT/Content/node_modules/.bin/mocha" ../../ASA.Web/Sites/SALT/Content/test --require blanket -R html-cov > ../../ASA.Web/Sites/SALT/Content/coverage.html';
        }
      }
    },
    sass: {
        all: ['../../ASA.Web/Sites/SALT/Assets/scss/**/*.scss'],
        local: {
            options: {
                noCache: false,
                style: 'expanded',
                compass: false
            },
            files: [{
                expand: true,
                cwd: '../../ASA.Web/Sites/SALT/Assets/scss',
                src: ['**/*.scss'],
                dest: '../../ASA.Web/Sites/SALT/Assets/css',
                // ext: '.css',
                // extDot works with grunt 0.4.3 and above Extensions in filenames begin after the last dot
                //once we upgrade then rename can be removed and ext and extDot used.
                // extDot: 'last',
                //rename handlea multiple dots in file name until ext & extDot are available
                rename  : function (dest, src) {
                    var _new_ext = 'css';
                    //Get src filename
                    src = src.split('/');
                    var filename = src.pop();
                    //Apply new extension to filename
                    var arr  = filename.split('.');
                    arr.pop();
                    arr.push(_new_ext);
                    filename = '/' + arr.join('.');
                    if (src.length > 0) {
                        dest = dest + '/';
                    }
                    return dest + src.join('/') + filename;
                }
            }],
        },
        prod: {
            options: {
                noCache: true,
                style: 'compressed',
                'sourcemap=none': '',
                compass: false
            },
            files: [{
                expand: true,
                cwd: '../../ASA.Web/Sites/SALT/Assets/scss',
                src: ['**/*.scss'],
                dest: '../../ASA.Web/Sites/SALT/Assets/css',
                // ext: '.css',
                // extDot works with grunt 0.4.3 and above Extensions in filenames begin after the last dot
                //once we upgrade then rename can be removed and ext and extDot used.
                // extDot: 'last',
                //rename handlea multiple dots in file name until ext & extDot are available
                rename  : function (dest, src) {
                    var _new_ext = 'css';
                    //Get src filename
                    src = src.split('/');
                    var filename = src.pop();
                    //Apply new extension to filename
                    var arr  = filename.split('.');
                    arr.pop();
                    arr.push(_new_ext);
                    filename = '/' + arr.join('.');
                    if (src.length > 0) {
                        dest = dest + '/';
                    }
                    return dest + src.join('/') + filename;
                }
            }]
        }
    },
    requirejs: {
      options: {
        appDir: '../../ASA.Web/Sites/SALT/assets/scripts/',
          baseUrl: './js',
          mainConfigFile: '../../ASA.Web/Sites/SALT/assets/scripts/js/require-config.js',
          dir: '../../ASA.Web/Sites/SALT/assets/scripts/OptimizedJS',
          paths: {
              'compiled': '../../templates/compiled/'
          },
          modules: [
              {
                  name: 'main',
                  include: ['libs/ios-orientationchange-fix', 'modules/SaltDustHelpers', 'modules/SearchAutoComplete', 'libs/fastclick', 'backbone.wreqr', 'jquery.dotdotdot', 'json'],
                  exclude: ['configuration']
              },
              {
                  name: 'modules/FinancialStatus/Application',
                  exclude: ['main', 'configuration', 'dust-helpers', 'modules/SaltDustHelpers', 'ASA/ASAUtilities']
              }
          ]
      },
      local: {
        options: {
          optimize: 'none'
        }
      },
      prod: {
        options: {
          optimize: 'uglify'
        }
      }
    }
  });

  // These plugins provide necessary tasks.
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-contrib-concat');
  grunt.loadNpmTasks('grunt-css');
  grunt.loadNpmTasks('grunt-contrib-watch');
  grunt.loadNpmTasks('grunt-exec');
  grunt.loadNpmTasks('grunt-contrib-sass');
  grunt.loadNpmTasks('grunt-contrib-requirejs');

  // "npm test" runs these tasks
  grunt.registerTask('test', ['exec:testMocha', 'exec:testServerJavaScript', 'exec:coverageServerJavaScript']);
  grunt.registerTask('DustCompile', 'exec:compileDust');
    // "buildMain" task is only being used by the watch task. For local environments, don't minify JS or scss
    grunt.registerTask('buildMain', ['concat:foundationPlugins', 'requirejs:local']);
    grunt.registerTask('hint', 'jshint');
    grunt.registerTask('sassLocal', 'sass:local');
    grunt.registerTask('build', ['jshint', 'sass:prod', 'concat:foundationPlugins', 'requirejs:prod']);

  // Run sub-grunt files, because right now, testing tasks is a pain.
  grunt.registerMultiTask('subgrunt', 'Run a sub-gruntfile.', function() {
    var path = require('path');
    grunt.util.async.forEachSeries(this.filesSrc, function(gruntfile, next) {
      grunt.util.spawn({
        grunt: true,
        args: ['--gruntfile', path.resolve(gruntfile)],
      }, function(error, result) {
        if (error) {
          grunt.log.error(result.stdout).writeln();
          next(new Error('Error running sub-gruntfile "' + gruntfile + '".'));
        } else {
          grunt.verbose.ok(result.stdout);
          next();
        }
      });
    }, this.async());
  });
};
