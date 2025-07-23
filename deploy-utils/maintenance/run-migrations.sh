#!/bin/bash
export ConnectionStrings__EndPointCommerceDbContext=$(cat /run/secrets/end-point-commerce-db-connection-string)
dotnet ef database update \
  --startup-project ./EndPointCommerce.AdminPortal \
  --project ./EndPointCommerce.Infrastructure
