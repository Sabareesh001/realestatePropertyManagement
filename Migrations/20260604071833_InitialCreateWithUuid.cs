using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace propertyManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithUuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "charge_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charge_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "charge_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charge_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_priorities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("complaint_priorities_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("complaint_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "complaint_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("complaint_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("countries_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("currencies_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "document_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("document_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lease_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lease_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "owner_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("owner_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_methods",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_methods_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payment_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payment_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "profile_types",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("profile_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "property_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("property_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proposal_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("proposal_statuses_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "charges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    desc = table.Column<string>(type: "text", nullable: true),
                    charge_type_id = table.Column<int>(type: "integer", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    due_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charges_pkey", x => x.id);
                    table.ForeignKey(
                        name: "charges_charge_type_id_fkey",
                        column: x => x.charge_type_id,
                        principalTable: "charge_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "charges_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "charge_statuses",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    country_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("states_pkey", x => x.id);
                    table.ForeignKey(
                        name: "states_country_id_fkey",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type_id = table.Column<int>(type: "integer", nullable: true),
                    document_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    document_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("documents_pkey", x => x.id);
                    table.ForeignKey(
                        name: "documents_document_type_id_fkey",
                        column: x => x.document_type_id,
                        principalTable: "document_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_ref = table.Column<string>(type: "character varying", nullable: true),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    currency_id = table.Column<int>(type: "integer", nullable: true),
                    paid_by = table.Column<Guid>(type: "uuid", nullable: true),
                    payment_method_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    paid_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("payments_pkey", x => x.id);
                    table.ForeignKey(
                        name: "payments_currency_id_fkey",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "payments_paid_by_fkey",
                        column: x => x.paid_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "payments_payment_method_id_fkey",
                        column: x => x.payment_method_id,
                        principalTable: "payment_methods",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "payments_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "payment_statuses",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_type_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_profiles_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "user_profiles_profile_type_id_fkey",
                        column: x => x.profile_type_id,
                        principalTable: "profile_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_profiles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    role_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_roles_pkey", x => x.id);
                    table.ForeignKey(
                        name: "user_roles_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_roles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "districts",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    state_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("districts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "districts_state_id_fkey",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_documents",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_documents_pkey", x => new { x.user_id, x.document_id });
                    table.ForeignKey(
                        name: "user_documents_document_id_fkey",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_documents_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "charge_payments",
                columns: table => new
                {
                    charge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    payment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_applied = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("charge_payments_pkey", x => new { x.charge_id, x.payment_id });
                    table.ForeignKey(
                        name: "charge_payments_charge_id_fkey",
                        column: x => x.charge_id,
                        principalTable: "charges",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "charge_payments_payment_id_fkey",
                        column: x => x.payment_id,
                        principalTable: "payments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "owner_profiles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    owner_type_id = table.Column<int>(type: "integer", nullable: true),
                    organization_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    gstin = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("owner_profiles_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "owner_profiles_owner_type_id_fkey",
                        column: x => x.owner_type_id,
                        principalTable: "owner_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "owner_profiles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "tenant_profiles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monthly_income = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    occupation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emergency_contact_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    emergency_contact_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tenant_profiles_pkey", x => x.user_id);
                    table.ForeignKey(
                        name: "tenant_profiles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    district_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "character varying", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cities_pkey", x => x.id);
                    table.ForeignKey(
                        name: "cities_district_id_fkey",
                        column: x => x.district_id,
                        principalTable: "districts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "properties",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    city_id = table.Column<int>(type: "integer", nullable: true),
                    address_line = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    latitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(10,7)", precision: 10, scale: 7, nullable: true),
                    thumbnail_img_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    monthly_rent = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    upfront_payment = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    security_deposit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("properties_pkey", x => x.id);
                    table.ForeignKey(
                        name: "properties_city_id_fkey",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "properties_owner_id_fkey",
                        column: x => x.owner_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "properties_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "property_statuses",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "complaints",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    property_id = table.Column<int>(type: "integer", nullable: true),
                    complaint_type_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    priority_id = table.Column<int>(type: "integer", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    resolved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    resolved_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("complaints_pkey", x => x.id);
                    table.ForeignKey(
                        name: "complaints_complaint_type_id_fkey",
                        column: x => x.complaint_type_id,
                        principalTable: "complaint_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "complaints_priority_id_fkey",
                        column: x => x.priority_id,
                        principalTable: "complaint_priorities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "complaints_property_id_fkey",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "complaints_resolved_by_fkey",
                        column: x => x.resolved_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "complaints_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "complaint_statuses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "complaints_tenant_id_fkey",
                        column: x => x.tenant_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lease_proposals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    property_id = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    monthly_rent = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    upfront_payment = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    security_deposit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    lease_type_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_proposals_pkey", x => x.id);
                    table.ForeignKey(
                        name: "lease_proposals_lease_type_id_fkey",
                        column: x => x.lease_type_id,
                        principalTable: "lease_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_proposals_property_id_fkey",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_proposals_reviewed_by_fkey",
                        column: x => x.reviewed_by,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_proposals_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "proposal_statuses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_proposals_tenant_id_fkey",
                        column: x => x.tenant_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "property_documents",
                columns: table => new
                {
                    property_id = table.Column<int>(type: "integer", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("property_documents_pkey", x => new { x.property_id, x.document_id });
                    table.ForeignKey(
                        name: "property_documents_document_id_fkey",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "property_documents_property_id_fkey",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "leases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    property_id = table.Column<int>(type: "integer", nullable: true),
                    proposal_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    monthly_rent = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    upfront_payment = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    security_deposit = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    lease_type_id = table.Column<int>(type: "integer", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("leases_pkey", x => x.id);
                    table.ForeignKey(
                        name: "leases_lease_type_id_fkey",
                        column: x => x.lease_type_id,
                        principalTable: "lease_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "leases_property_id_fkey",
                        column: x => x.property_id,
                        principalTable: "properties",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "leases_proposal_id_fkey",
                        column: x => x.proposal_id,
                        principalTable: "lease_proposals",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "leases_status_id_fkey",
                        column: x => x.status_id,
                        principalTable: "lease_statuses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "leases_tenant_id_fkey",
                        column: x => x.tenant_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lease_charges",
                columns: table => new
                {
                    charge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_charges_pkey", x => new { x.charge_id, x.lease_id });
                    table.ForeignKey(
                        name: "lease_charges_charge_id_fkey",
                        column: x => x.charge_id,
                        principalTable: "charges",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_charges_lease_id_fkey",
                        column: x => x.lease_id,
                        principalTable: "leases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "lease_documents",
                columns: table => new
                {
                    lease_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lease_documents_pkey", x => new { x.lease_id, x.document_id });
                    table.ForeignKey(
                        name: "lease_documents_document_id_fkey",
                        column: x => x.document_id,
                        principalTable: "documents",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lease_documents_lease_id_fkey",
                        column: x => x.lease_id,
                        principalTable: "leases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_charges",
                columns: table => new
                {
                    charge_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_charges_pkey", x => new { x.charge_id, x.user_id });
                    table.ForeignKey(
                        name: "user_charges_charge_id_fkey",
                        column: x => x.charge_id,
                        principalTable: "charges",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "user_charges_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "leases",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_charge_payments_payment_id",
                table: "charge_payments",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "charge_statuses_name_key",
                table: "charge_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "charge_types_name_key",
                table: "charge_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_charges_charge_type_id",
                table: "charges",
                column: "charge_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_charges_status_id",
                table: "charges",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_cities_district_id",
                table: "cities",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "complaint_priorities_name_key",
                table: "complaint_priorities",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "complaint_statuses_name_key",
                table: "complaint_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "complaint_types_name_key",
                table: "complaint_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_complaints_complaint_type_id",
                table: "complaints",
                column: "complaint_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_priority_id",
                table: "complaints",
                column: "priority_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_property_id",
                table: "complaints",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_resolved_by",
                table: "complaints",
                column: "resolved_by");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_status_id",
                table: "complaints",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_complaints_tenant_id",
                table: "complaints",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "countries_name_key",
                table: "countries",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "currencies_code_key",
                table: "currencies",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_districts_state_id",
                table: "districts",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "document_types_name_key",
                table: "document_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_documents_document_type_id",
                table: "documents",
                column: "document_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_charges_lease_id",
                table: "lease_charges",
                column: "lease_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_documents_document_id",
                table: "lease_documents",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_lease_type_id",
                table: "lease_proposals",
                column: "lease_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_property_id",
                table: "lease_proposals",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_reviewed_by",
                table: "lease_proposals",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_status_id",
                table: "lease_proposals",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_lease_proposals_tenant_id",
                table: "lease_proposals",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "lease_statuses_name_key",
                table: "lease_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "lease_types_name_key",
                table: "lease_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_leases_lease_type_id",
                table: "leases",
                column: "lease_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_property_id",
                table: "leases",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_proposal_id",
                table: "leases",
                column: "proposal_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_status_id",
                table: "leases",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_leases_tenant_id",
                table: "leases",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_owner_profiles_owner_type_id",
                table: "owner_profiles",
                column: "owner_type_id");

            migrationBuilder.CreateIndex(
                name: "owner_types_name_key",
                table: "owner_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "payment_methods_name_key",
                table: "payment_methods",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "payment_statuses_name_key",
                table: "payment_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_currency_id",
                table: "payments",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_paid_by",
                table: "payments",
                column: "paid_by");

            migrationBuilder.CreateIndex(
                name: "IX_payments_payment_method_id",
                table: "payments",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_status_id",
                table: "payments",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "payments_transaction_ref_key",
                table: "payments",
                column: "transaction_ref",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "profile_types_name_key",
                table: "profile_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_properties_city_id",
                table: "properties",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_properties_owner_id",
                table: "properties",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_properties_status_id",
                table: "properties",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_property_documents_document_id",
                table: "property_documents",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "property_statuses_name_key",
                table: "property_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "proposal_statuses_name_key",
                table: "proposal_statuses",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "roles_name_key",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_states_country_id",
                table: "states",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_charges_user_id",
                table: "user_charges",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_documents_document_id",
                table: "user_documents",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_profile_type_id",
                table: "user_profiles",
                column: "profile_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "user_roles_user_id_role_id_idx",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "charge_payments");

            migrationBuilder.DropTable(
                name: "complaints");

            migrationBuilder.DropTable(
                name: "lease_charges");

            migrationBuilder.DropTable(
                name: "lease_documents");

            migrationBuilder.DropTable(
                name: "owner_profiles");

            migrationBuilder.DropTable(
                name: "property_documents");

            migrationBuilder.DropTable(
                name: "tenant_profiles");

            migrationBuilder.DropTable(
                name: "user_charges");

            migrationBuilder.DropTable(
                name: "user_documents");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "complaint_types");

            migrationBuilder.DropTable(
                name: "complaint_priorities");

            migrationBuilder.DropTable(
                name: "complaint_statuses");

            migrationBuilder.DropTable(
                name: "owner_types");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "charges");

            migrationBuilder.DropTable(
                name: "leases");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "payment_methods");

            migrationBuilder.DropTable(
                name: "payment_statuses");

            migrationBuilder.DropTable(
                name: "profile_types");

            migrationBuilder.DropTable(
                name: "charge_types");

            migrationBuilder.DropTable(
                name: "charge_statuses");

            migrationBuilder.DropTable(
                name: "lease_proposals");

            migrationBuilder.DropTable(
                name: "lease_statuses");

            migrationBuilder.DropTable(
                name: "document_types");

            migrationBuilder.DropTable(
                name: "lease_types");

            migrationBuilder.DropTable(
                name: "properties");

            migrationBuilder.DropTable(
                name: "proposal_statuses");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "property_statuses");

            migrationBuilder.DropTable(
                name: "districts");

            migrationBuilder.DropTable(
                name: "states");

            migrationBuilder.DropTable(
                name: "countries");
        }
    }
}
