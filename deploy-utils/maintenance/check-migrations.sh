#!/bin/bash

if [ ! -d "./EndPointCommerce.Infrastructure" ]; then
  echo "Run this from the same directory as end-point-commerce.sln."
  exit 1
fi

export ConnectionStrings__EndPointCommerceDbContext=$(cat /run/secrets/end-point-commerce-db-connection-string)
dotnet ef migrations list \
  --startup-project ./EndPointCommerce.AdminPortal \
  --project ./EndPointCommerce.Infrastructure
