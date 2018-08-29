@echo off
cd EDMS/Tools/JSTools
node r.js -o buildMain.js optimize=none
node_modules\.bin\grunt.cmd minjs