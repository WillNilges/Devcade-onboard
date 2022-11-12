# Devcade-onboard
The onboard menu and control software for the Devcade custom arcade system.


## Building

To make a build to run on the Idiot, do the following from `/onboard`:
```
dotnet publish -c Release -r linux-x64 --no-self-contained
```

To put it on the Idiot, compress the `publish` folder located at `\Devcade-onboard\onboard\bin\Release\netcoreapp3.1\linux-x64` and `scp` that to the Idiot.

## The Idiot

### Prereqs

Debian >=10

A user named `devcade`

`apt install xorg` and friends (I dont actually know what all is installed)

### Daemon

_daemons are always watching. They are always with you. So is Willard._

The Devcade Idiot is running Debian 10 with a very _very_ simple Xorg server setup. It has a systemd service configured to launch the onboarding program, along with said xorg server, as the `devcade` user.

You can find everything(tm) you need to set up the Devcade Idiot in `/idiot`. This includes the systemd service, which contains the path you need to install it at.

```
/etc/systemd/system/devcade-onboard.service
```

You'll also need to add/change some lines in your `/etc/X11/Xwrapper.config`

```
needs_root_rights=yes
allowed_users=anybody
```

This should be interactable as a normal systemd service, so `enable`/`disable` it as normal.

_Helpful Tip: Remember to `chmod +x onboard`. You may get weird syntax errors if you don't_

## HACKING

To setup and launch a development environment, you can do the following:

### Env Vars
Create a `.env` file with the following values.

```
AWS_ACCESS_KEY_ID
AWS_SECRET_ACCESS_KEY
AWS_DEFAULT_REGION
DEVCADE_API_DOMAIN
```

### Building and Launching the Container

```
cd HACKING
./build-environment.sh
./launch-environment.sh
```
