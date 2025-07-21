#!/bin/bash

cat <<EOF >/app/wwwroot/appsettings.json
{
  "EndPointCommerceApiUrl": "$EndPointCommerceApiUrl",
  "AuthNetEnvironment": "$AuthNetEnvironment",
  "AuthNetLoginId": "$AuthNetLoginId",
  "AuthNetClientKey": "$AuthNetClientKey"
}
EOF
