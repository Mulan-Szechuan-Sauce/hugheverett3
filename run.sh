#!/bin/sh

# Script for easily compiling and running from the command line

set -e

cd src
msbuild

cd Hugh
exec mono bin/DesktopGL/AnyCPU/Debug/Hugh.exe
