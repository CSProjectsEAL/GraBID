#!/bin/sh
echo '>>>---STARTING INGESTION FROM MONO--->' >> /activity.log
crond -b
# Run your dotnet console app
/usr/bin/dotnet /var/app/Ingestion.dll
tail /activity.log -f