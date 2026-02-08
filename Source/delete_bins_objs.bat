rem This script delete all the "obj" and "bin" folders from the current folder (and recursively from all the subfolder).

@echo off
setlocal

rem Set root_folder to the folder where the script is located
set "root_folder=%~dp0"

rem Recursively delete the bin and obj folders
for /d %%d in ("%root_folder%\*") do (
    if /i "%%~nxd"=="bin" (
        echo Deleting folder: %%d
        rmdir /s /q "%%d"
    ) else if /i "%%~nxd"=="obj" (
        echo Deleting folder: %%d
        rmdir /s /q "%%d"
    )
    rem Recursively check the subfolders
    for /d %%s in ("%%d\*") do (
        if /i "%%~ns"=="bin" (
            echo Deleting folder: %%s
            rmdir /s /q "%%s"
        ) else if /i "%%~ns"=="obj" (
            echo Deleting folder: %%s
            rmdir /s /q "%%s"
        )
    )
)

endlocal