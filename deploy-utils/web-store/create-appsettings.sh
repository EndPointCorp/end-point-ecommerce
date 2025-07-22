#!/bin/bash

cat <<EOF >/usr/share/nginx/html/appsettings.json
{
  "EndPointCommerceApiUrl": "$EndPointCommerceApiUrl",
  "AuthNetEnvironment": "$AuthNetEnvironment",
  "AuthNetLoginId": "$AuthNetLoginId",
  "AuthNetClientKey": "$AuthNetClientKey"
}
EOF
