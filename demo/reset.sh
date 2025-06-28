#!/bin/bash
cd "$(dirname "$0")"

source .env

# Ensure these are running (they might already be, but perhaps not if this is run as part of a deployment)
docker compose up db maintenance -d --force-recreate

# Stop the other containers that use the DB
docker compose down admin-portal web-api web-store

# Remove all existing images, and reset with the original reference demo store images
rm -rf images/*
cp -r reference-images/* images/

# Remove everything from the DB and re-initialize from scratch
docker compose exec db psql -U $END_POINT_COMMERCE_DB_USERNAME -c "drop owned by $END_POINT_COMMERCE_DB_USERNAME cascade;"
docker compose exec maintenance run-migrations.sh

# Add demo store data customizations
docker compose cp demo-data.sql db:/
docker compose exec db psql -U $END_POINT_COMMERCE_DB_USERNAME -f /demo-data.sql

# Add demo store admin user
docker compose exec maintenance run-job.sh create_admin_user -u admin -e ecommerce-demo-admin@endpointdev.com -p Sh0pd3m0!

# Bring up all the containers that we previously stopped
docker compose up admin-portal web-api web-store -d --force-recreate
