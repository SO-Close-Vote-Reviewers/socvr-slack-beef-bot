FROM mono:latest

RUN apt-get update && apt-get install -y \
 sqlite3 \
 wget

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
