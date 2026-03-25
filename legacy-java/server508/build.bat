@echo off
title Building AeroScape
setlocal enabledelayedexpansion

echo Compiling Java source files...

:: Create the bin folder if it doesn't exist
if not exist bin mkdir bin
if exist sources.txt del sources.txt

:: Loop through all .java files, double the backslashes, and quote them
for /R "src\main\java" %%F in (*.java) do (
    set "filepath=%%F"
    set "filepath=!filepath:\=\\!"
    echo "!filepath!" >> sources.txt
)

:: Compile the safely formatted list
javac -d bin @sources.txt

:: Clean up
del sources.txt

echo Compile complete!
pause