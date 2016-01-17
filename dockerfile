FROM mono:4.2.1

# Things to do:
# - nuget
# - git
# - sqlite

# Install things we can install without setup

RUN apt-get update && apt-get install -y \
 nuget \
 nunit \
 git \
 joe \
 nano \
 sudo \
 sqlite3

# copy in the source folder
COPY source/ /tmp/source/

# compile it and copy the output to the /srv/beefbot directory
RUN \
  nuget restore /tmp/source/SOCVR.Slack.BeefBot.sln && \
  xbuild /p:Configuration=Release /tmp/source/SOCVR.Slack.BeefBot.sln && \
  mkdir -p /srv/beefbot && \
  mkdir -p /var/beef-data && \
  cp /tmp/source/SOCVR.Slack.BeefBot/bin/Release/* /srv/beefbot/

CMD ["mono", "/srv/slackbot/SOCVR.Slack.BeefBot.exe"]
