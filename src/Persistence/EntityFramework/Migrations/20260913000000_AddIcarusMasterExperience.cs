// <copyright file="20260913000000_AddIcarusMasterExperience.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Migrations
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    /// <summary>
    /// Allows maps to grant master experience independently from monster level.
    /// </summary>
    [DbContext(typeof(EntityDataContext))]
    [Migration("20260913000000_AddIcarusMasterExperience")]
    public partial class AddIcarusMasterExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GrantsMasterExperience",
                schema: "config",
                table: "GameMapDefinition",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""UPDATE config."GameMapDefinition" SET "GrantsMasterExperience" = TRUE WHERE "Number" = 10 AND "Discriminator" = 0;""");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrantsMasterExperience",
                schema: "config",
                table: "GameMapDefinition");
        }
    }
}
