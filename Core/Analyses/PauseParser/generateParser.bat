@echo off
@rem set GPPG_LOC="C:\Users\Robin\Documents\Visual Studio 2010\Projects\InputLog\gppg-distro-1.5.0\binaries\gppg.exe"
set GPPG_LOC="..\..\..\gppg-distro-1.5.0\binaries\gppg.exe"
if not exist %GPPG_LOC% (goto :notfoundlabel)
set DEFAULT_INPUT="Parser.y"

@rem Count arguments
set ARGC=0
for %%x in (%*) do Set /A ARGC+=1

if %ARGC% GTR 1 (goto :printhelplabel) else (goto :programlabel)


@rem Print help
:printhelplabel
echo Please give the input file for GPPG as a commandline argument.
echo For example: generateParser.bat foo.y
goto :eoflabel


@rem GPPG not found
:notfoundlabel
echo GPPG not found. Please set the correct location of GGPG in the script.
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
for /f "delims=" %%i in ('dir /b /OD %INPUTFILE% "Parser.cs"') do set LAST=%%i
if /I "%LAST%" == %INPUTFILE% (goto :producelabel) else (goto :eoflabel)

@rem The /gplex option is important here, as it allows the use of the GPLEX scanner.
@rem The /report option is less important: this results in some information about the generated automaton to be printed to a .report.html file.
@rem The .report.html can be viewed with a HTML viewer (e.g. any Internet browser).

:producelabel
echo Running GPPG with options /gplex and /report on %INPUTFILE%
(%GPPG_LOC% %INPUTFILE% /gplex /report)

:eoflabel