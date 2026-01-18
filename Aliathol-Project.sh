#!/bin/sh
printf '\033c\033]0;%s\a' Aliathol-Project
base_path="$(dirname "$(realpath "$0")")"
"$base_path/Aliathol-Project.x86_64" "$@"
