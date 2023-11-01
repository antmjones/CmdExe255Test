This is a .NET 4 based console application to reproduce a bug in ```cmd.exe```.

# Background

When publishing a Blazor client application, the ```EmccCompile``` MSBuild task launches 
```emcc.bat```. I encountered an intermittent issue where the build would exit with an error
due to ```cmd.exe``` returning an error code of 255.

Futher investigation showed that the underlying issue is related to the fact that
```emcc.bat``` calls Python with stdin redirected as follows:

```@%CMD% %* < NUL```

This itself is a workaround for a Python bug, see https://github.com/emscripten-core/emscripten/blob/e8535a814a96039279c33017449805b0ddb64578/em%2B%2B.bat#L87,
for further details.

Under certain cirumstances the redirection of stdin using ```< NUL``` appears to trigger a
bug in ```cmd.exe```, whereby:

- The proceeding lines in the batch file are echoed to stdout regardless of the ```@``` prefix.
- If exiting with ```exit /b ...```, the exit code is always 255 rather than the specified exit code.

See also https://github.com/emscripten-core/emscripten/issues/20583 for some further discussion.

# Reproduction

To reproduce the issue, I have found the following works reliably for me:

- Have a batch file that redirects stdin at some point using ```... < NUL``` (as ```emcc.bat``` does).
- Launch that batch file from a process that redirects stdout and stderr, but that has stdin closed.
- Run from "Windows Terminal" rather than a "standard" command prompt (using a standard command 
  prompt window the issue will only occasionaly occur, whereas using Windows Terminal it will occur
  almost every time for me).

The .NET Console app in this repo demonstrates this.

## Expected output:

```
STDOUT: Testing...
Exit code: 0
```

## Actual output

```
STDOUT: Testing...
STDOUT: C:\...\net48>@exit /b 0
STDOUT: C:\...\net48>
Exit code: 255
```
