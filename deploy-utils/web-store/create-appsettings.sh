#!/bin/bash

cat <<EOF >/usr/share/nginx/html/appsettings.json
{
  "EndPointEcommerceApiUrl": "$EndPointEcommerceApiUrl",
  "AuthNetEnvironment": "$AuthNetEnvironment",
  "AuthNetLoginId": "$AuthNetLoginId",
  "AuthNetClientKey": "$AuthNetClientKey"
}
EOF
