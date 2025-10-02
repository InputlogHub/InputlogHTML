@echo off
@rem set GPLEX_LOC="C:\Users\Robin\Documents\Visual Studio 2010\Projects\InputLog\gplex-distro-1.1.2\binaries\gplex.exe"
set GPLEX_LOC="..\..\..\gplex-distro-1.1.2\binaries\gplex.exe"
if not exist %GPLEX_LOC% (goto :notfoundlabel)
set DEFAULT_INPUT="Scanner.lex"

@rem Count arguments
set ARGC=0
for %%x in (%*) do Set /A ARGC+=1

if %ARGC% GTR 1 (goto :printhelplabel) else (goto :programlabel)


@rem Print help
:printhelplabel
echo Please give the input file for GPLEX as a commandline argument.
echo For example: generateScanner.bat foo.lex
goto :eoflabel


@rem GPLEX not found
:notfoundlabel
echo GPLEX not found. Please set the correct location of GPLEX in the script.
goto :eoflabel


@rem Inputfile not found
:inputnotfoundlabel
echo Inputfile not found.
goto :eoflabel


@rem Run program itself
:programlabel

set INPUTFILE="%1"
@rem if not exist %INPUTFILE% (goto :inputnotfoundlabel)
if not exist %INPUTFILE% (echo Using default input %DEFAULT_INPUT%  & set INPUTFILE=%DEFAULT_INPUT%)

@rem Only generate new .cs file if input file was edited after last .cs file was modified
for /f "delims=" %%i in ('dir /b /OD %INPUTFILE% "Scanner.cs"') do set LAST=%%i
if /I "%LAST%" == %INPUTFILE% (goto :producelabel) else (goto :eoflabel)

:producelabel
echo Running GPLEX on %INPUTFILE%

%GPLEX_LOC% %INPUTFILE%

:eoflabel