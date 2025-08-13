#!/bin/bash
export ConnectionStrings__EndPointEcommerceDbContext=$(cat /run/secrets/end-point-ecommerce-db-connection-string)
dotnet run --project ./EndPointEcommerce.Jobs -- $@
