#!/bin/bash

set -e

function help() {
    echo "-u : Set username | -d : Set container | -n : Set name | -h : Print help"
}

if [ "$(pwd)" == "$HOME" ]; then
    echo "DON'T FUCKING RUN THIS SCRIPT FROM YOUR FUCKING HOMEDIR"
    echo "Place it in a directory LITERALLY ANYWHERE ELSE where it can't hurt you :)"
    exit 1
fi

uname=$USER # Just trust me on this one.
container='devcade-dotnet'
name='devcade-dotnet'

while getopts ":hu:d:n:" option; do
    case $option in
        u) # change uname
            uname=$OPTARG;;
        d) # choose which container to use
            container=$OPTARG;;
        n) # set container name
            name=$OPTARG;;
        h)
            help
            exit;;
    esac
done

xauth_path=/tmp/xopp-dev-xauth

rm -rf "$xauth_path"
mkdir -p "$xauth_path"
cp "$HOME"/.Xauthority "$xauth_path"
chmod g+rwx "$xauth_path"/.Xauthority

podman run --name="$name" --rm -it                                \
    -e DISPLAY="$DISPLAY"                                              \
    --network=host                                                     \
    --cap-add=SYS_PTRACE                                               \
    --group-add keep-groups                                            \
    --annotation io.crun.keep_original_groups=1                        \
    -v "$xauth_path"/.Xauthority:/root/.Xauthority:Z                   \
    -v ./:/devcade:Z                                                \
    -v /tmp/.X11-unix:/tmp/.X11-unix                                   \
    --env 'PKG_CONFIG_PATH=/usr/local/lib/pkgconfig:$PKG_CONFIG_PATH'  \
    "$container"
rm -rf "$xauth_path"
