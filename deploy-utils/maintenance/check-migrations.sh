#!/bin/bash
export ConnectionStrings__EndPointCommerceDbContext=$(cat /run/secrets/end-point-commerce-db-connection-string)
dotnet ef migrations list \
  --startup-project ./EndPointCommerce.AdminPortal \
  --project ./EndPointCommerce.Infrastructure
