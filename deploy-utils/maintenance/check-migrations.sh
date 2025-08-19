#!/bin/bash
# Copyright 2025 End Point Corporation. Apache License, version 2.0.

if [ ! -d "./EndPointEcommerce.Infrastructure" ]; then
  echo "Run this from the same directory as end-point-ecommerce.sln."
  exit 1
fi

export ConnectionStrings__EndPointEcommerceDbContext=$(cat /run/secrets/end-point-ecommerce-db-connection-string)
dotnet ef migrations list \
  --startup-project ./EndPointEcommerce.AdminPortal \
  --project ./EndPointEcommerce.Infrastructure
