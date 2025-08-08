#!/bin/bash

if [ ! -d "./EndPointEcommerce.Infrastructure" ]; then
  echo "Run this from the same directory as end-point-ecommerce.sln."
  exit 1
fi

export ConnectionStrings__EndPointEcommerceDbContext=$(cat /run/secrets/end-point-ecommerce-db-connection-string)
dotnet ef database update \
  --startup-project ./EndPointEcommerce.AdminPortal \
  --project ./EndPointEcommerce.Infrastructure
