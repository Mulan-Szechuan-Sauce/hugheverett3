#!/bin/sh

# For calling this via Tiled on MacOS
PATH=$PATH:/Library/Frameworks/Mono.framework/Versions/Current/Commands

cd src/Hugh
exec mono bin/DesktopGL/AnyCPU/Debug/Hugh.exe $1
