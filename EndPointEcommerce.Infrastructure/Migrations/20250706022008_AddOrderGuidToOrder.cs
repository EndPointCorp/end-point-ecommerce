// Copyright 2025 End Point Corporation. Apache License, version 2.0.

﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EndPointEcommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderGuidToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "order_guid",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order_guid",
                table: "orders");
        }
    }
}
