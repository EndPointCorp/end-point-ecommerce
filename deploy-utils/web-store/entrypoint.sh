#!/bin/sh
# Copyright 2025 End Point Corporation. Apache License, version 2.0.

/create-appsettings.sh

# proceed with the original nginx image's entrypoint script
exec /docker-entrypoint.sh "$@"
