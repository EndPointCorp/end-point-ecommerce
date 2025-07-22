#!/bin/sh

/create-appsettings.sh

# proceed with the original nginx image's entrypoint script
exec /docker-entrypoint.sh "$@"
