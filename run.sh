#!/bin/sh

# Script for easily compiling and running from the command line

set -e

cd src
msbuild

exec mono Hugh/bin/DesktopGL/AnyCPU/Debug/Hugh.exe
