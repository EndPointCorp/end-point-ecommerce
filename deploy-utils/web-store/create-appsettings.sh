#!/bin/bash
# Copyright 2025 End Point Corporation. Apache License, version 2.0.

cat <<EOF >/usr/share/nginx/html/appsettings.json
{
  "EndPointEcommerceApiUrl": "$EndPointEcommerceApiUrl",
  "AuthNetEnvironment": "$AuthNetEnvironment",
  "AuthNetLoginId": "$AuthNetLoginId",
  "AuthNetClientKey": "$AuthNetClientKey"
}
EOF
