#!/bin/sh
# Script for easily compiling and running from the command line

# For calling this via Tiled on MacOS
PATH=$PATH:/Library/Frameworks/Mono.framework/Versions/Current/Commands

set -e

cd src
msbuild

cd HughFor
exec mono bin/DesktopGL/AnyCPU/Debug/HughFor.exe $1
