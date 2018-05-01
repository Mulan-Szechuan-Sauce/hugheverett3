#!/bin/sh

# For calling this via Tiled on MacOS
PATH=$PATH:/Library/Frameworks/Mono.framework/Versions/Current/Commands

cd src/HughFor
exec mono bin/DesktopGL/AnyCPU/Debug/HughFor.exe $1
