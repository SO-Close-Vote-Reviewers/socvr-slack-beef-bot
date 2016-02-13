FROM ubuntu:15.10

# Things to do:
# - git
# - sqlite

# Install things we can install without setup

RUN apt-get update && apt-get install -y \
 python-software-properties \
 software-properties-common \
 nunit \
 git \
 nano \
 sudo \
 sqlite3 \
 wget
 
# Install Mono and CA keys
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb http://download.mono-project.com/repo/debian wheezy main" | tee /etc/apt/sources.list.d/mono-xamarin.list

# Do the final installs now that all the setup is done
RUN apt-get update && apt-get install -y \
 mono-complete \
 ca-certificates-mono

# copy in the source folder
COPY source/ /tmp/source/

# compile it and copy the output to the /srv/beefbot directory
RUN \
  wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe && \
  mono /nuget.exe restore /tmp/source/SOCVR.Slack.BeefBot.sln && \
  touch /tmp/source/SOCVR.Slack.BeefBot/settings.json && \
  xbuild /p:Configuration=Release /tmp/source/SOCVR.Slack.BeefBot.sln && \
  mkdir -p /srv/beefbot && \
  mkdir -p /var/beef-data && \
  cp -r /tmp/source/SOCVR.Slack.BeefBot/bin/Release/* /srv/beefbot/

CMD ["mono", "/srv/beefbot/SOCVR.Slack.BeefBot.exe"]
