INSTALLATION 
============

1. NodeJS
http://nodejs.org/

IN TERMINAL
2. Grunt
npm install -g grunt

3. BBB
npm install -g bbb

4. Start Server 
cd ../ASA_LessonsAndTools/site
bbb server

5. VMware
change this line in grunt.js - replace "127.0.0.1" with your IP address
host: "127.0.0.1", port: 8000

![Boilerplate](https://github.com/tbranyen/backbone-boilerplate/raw/assets/header.png)

Backbone Boilerplate
====================

This boilerplate is the product of much research and frustration.  Existing
boilerplates freely modify Backbone core, lack a build process, and are very
prescriptive; Backbone Boilerplate changes that.

Organize your application in a logical filesystem, develop your
Models/Collections/Views/Routers inside modules, and build knowing you have
efficient code that will not bottleneck your users.

Thanks to our
[Contributors](https://github.com/tbranyen/backbone-boilerplate/contributors)!

Special Thanks to: [cowboy](http://github.com/cowboy),
[iros](http://github.com/iros), [nimbupani](http://github.com/nimbupani),
[wookiehangover](http://github.com/wookiehangover), and
[jugglinmike](http://github.com/jugglinmike) for helping me create this project.

Extra Special Thanks to: [Paul Guinan](http://bigredhair.com/work/paul.html)
for giving me usage rights to his fantastic Boilerplate character.

## Documentation ##

View the Backbone Boilerplate documentation here:

[GitHub Wiki](https://github.com/tbranyen/backbone-boilerplate/wiki)

## Build process ##

To use the new and improved build process, please visit the 
[grunt-bbb](https://github.com/backbone-boilerplate/grunt-bbb)
plugin repo and follow the instructions to install.  Basing your project off
this repo will allow the `bbb` commands to work out-of-the-box.
