#!/bin/bash
export ConnectionStrings__EndPointCommerceDbContext=$(cat /run/secrets/end-point-commerce-db-connection-string)
dotnet run --project ./EndPointCommerce.Jobs -- $@
