#!/bin/bash
cd "$(dirname "$0")"

source .env

# Ensure everything is stopped, and fully remove all persistent volumes as well to blow away the DB
docker compose down -v

# Bring up the database and run the migrations
docker compose up db maintenance -d
docker compose exec maintenance run-migrations.sh

# Add demo store data customizations
docker compose cp demo-data.sql db:/
docker compose exec db psql -U $END_POINT_COMMERCE_DB_USERNAME -f /demo-data.sql

# Add demo store admin user
docker compose exec maintenance run-job.sh create_admin_user -u admin -e ecommerce-demo-admin@endpointdev.com -p Sh0pd3m0!

# Remove all existing images, and reset with the original reference demo store images
rm -rf images/*
cp -r reference-images/* images/

# Now we can bring up all the containers
docker compose up -d
