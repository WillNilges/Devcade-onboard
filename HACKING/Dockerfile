FROM debian:buster

RUN apt-get -y update
RUN apt-get -y install vim wget git fonts-liberation fontconfig-config tmux xterm pulseaudio zip unzip tiled
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb \
     -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-6.0 dotnet-runtime-6.0 libgtk-3-0
RUN dotnet tool install -g dotnet-mgcb; dotnet tool restore
