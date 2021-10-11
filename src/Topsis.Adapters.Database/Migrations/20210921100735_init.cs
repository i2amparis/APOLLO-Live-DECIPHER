using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Topsis.Adapters.Database.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WsCountries",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    A3Code = table.Column<string>(nullable: true),
                    A2Code = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WsJobCategories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsJobCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WsQuestionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DefaultCriterionWeight = table.Column<double>(nullable: false),
                    SettingsJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsQuestionnaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    CountryId = table.Column<string>(nullable: true),
                    JobCategoryId = table.Column<int>(nullable: true),
                    Created_UserId = table.Column<string>(nullable: true),
                    Created_Time = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_WsCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "WsCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_WsJobCategories_JobCategoryId",
                        column: x => x.JobCategoryId,
                        principalTable: "WsJobCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WsAlternatives",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Order = table.Column<short>(nullable: false),
                    QuestionnaireId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsAlternatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsAlternatives_WsQuestionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "WsQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WsCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    Order = table.Column<short>(nullable: false),
                    QuestionnaireId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsCriteria_WsQuestionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "WsQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WsWorkspaces",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(nullable: true),
                    QuestionnaireId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(maxLength: 255, nullable: true),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    CurrentStatus = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsWorkspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsWorkspaces_WsWorkspaces_ParentId",
                        column: x => x.ParentId,
                        principalTable: "WsWorkspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WsWorkspaces_WsQuestionnaires_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalTable: "WsQuestionnaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WsStakeholderVotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkspaceId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsStakeholderVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsStakeholderVotes_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WsStakeholderVotes_WsWorkspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "WsWorkspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WsWorkspacesReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkspaceId = table.Column<int>(nullable: false),
                    Algorithm = table.Column<short>(nullable: false),
                    VotesCount = table.Column<int>(nullable: false),
                    Criteria = table.Column<string>(nullable: true),
                    Alternatives = table.Column<string>(nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(nullable: true),
                    AnalysisResultJson = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsWorkspacesReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsWorkspacesReports_WsWorkspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalTable: "WsWorkspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WsStakeholderAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VoteId = table.Column<int>(nullable: false),
                    AlternativeId = table.Column<int>(nullable: false),
                    CriterionId = table.Column<int>(nullable: false),
                    Value = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsStakeholderAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsStakeholderAnswers_WsAlternatives_AlternativeId",
                        column: x => x.AlternativeId,
                        principalTable: "WsAlternatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WsStakeholderAnswers_WsCriteria_CriterionId",
                        column: x => x.CriterionId,
                        principalTable: "WsCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WsStakeholderAnswers_WsStakeholderVotes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "WsStakeholderVotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WsStakeholderCriteriaImportance",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VoteId = table.Column<int>(nullable: false),
                    CriterionId = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WsStakeholderCriteriaImportance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WsStakeholderCriteriaImportance_WsCriteria_CriterionId",
                        column: x => x.CriterionId,
                        principalTable: "WsCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WsStakeholderCriteriaImportance_WsStakeholderVotes_VoteId",
                        column: x => x.VoteId,
                        principalTable: "WsStakeholderVotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7A", "b04fcd06-06e7-4a76-b71a-585418121efb", "admin", "ADMIN" },
                    { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B", "605db19c-0dc8-45bf-9228-d1591f505a3e", "moderator", "MODERATOR" },
                    { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C", "4b449d81-5beb-4d20-9086-e384e1b3bc96", "stakeholder", "STAKEHOLDER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CountryId", "Email", "EmailConfirmed", "FirstName", "JobCategoryId", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70", 0, "5f150202-08ca-4ab9-9570-7284d9886209", null, "a.soursos@gmail.com", true, null, null, null, false, null, "a.soursos@gmail.com", "a.soursos@gmail.com", "AQAAAAEAACcQAAAAEJ0km/zrlMTQRDO1HE1WKhsWPfZs2GbkmWyap5gF/1yTNAjRQ6YyeSf5X4sEV7/sFA==", "", true, "e8778294-ab9e-42d0-9c7c-21200ab7eb57", false, "a.soursos@gmail.com" });

            migrationBuilder.InsertData(
                table: "WsCountries",
                columns: new[] { "Id", "A2Code", "A3Code", "Title" },
                values: new object[,]
                {
                    { "591", "PA", "PAN", "Panama" },
                    { "275", "PS", "PSE", "Palestine, State of" },
                    { "585", "PW", "PLW", "Palau" },
                    { "586", "PK", "PAK", "Pakistan" },
                    { "512", "OM", "OMN", "Oman" },
                    { "580", "MP", "MNP", "Northern Mariana Islands (the)" },
                    { "598", "PG", "PNG", "Papua New Guinea" },
                    { "574", "NF", "NFK", "Norfolk Island" },
                    { "570", "NU", "NIU", "Niue" },
                    { "566", "NG", "NGA", "Nigeria" },
                    { "562", "NE", "NER", "Niger (the)" },
                    { "578", "NO", "NOR", "Norway" },
                    { "600", "PY", "PRY", "Paraguay" },
                    { "608", "PH", "PHL", "Philippines (the)" },
                    { "558", "NI", "NIC", "Nicaragua" },
                    { "612", "PN", "PCN", "Pitcairn" },
                    { "616", "PL", "POL", "Poland" },
                    { "620", "PT", "PRT", "Portugal" },
                    { "630", "PR", "PRI", "Puerto Rico" },
                    { "634", "QA", "QAT", "Qatar" },
                    { "807", "MK", "MKD", "Republic of North Macedonia" },
                    { "642", "RO", "ROU", "Romania" },
                    { "643", "RU", "RUS", "Russian Federation (the)" },
                    { "646", "RW", "RWA", "Rwanda" },
                    { "638", "RE", "REU", "Réunion" },
                    { "652", "BL", "BLM", "Saint Barthélemy" },
                    { "654", "SH", "SHN", "Saint Helena, Ascension and Tristan da Cunha" },
                    { "604", "PE", "PER", "Peru" },
                    { "554", "NZ", "NZL", "New Zealand" },
                    { "528", "NL", "NLD", "Netherlands (the)" },
                    { "659", "KN", "KNA", "Saint Kitts and Nevis" },
                    { "434", "LY", "LBY", "Libya" },
                    { "440", "LT", "LTU", "Lithuania" },
                    { "442", "LU", "LUX", "Luxembourg" },
                    { "446", "MO", "MAC", "Macao" },
                    { "450", "MG", "MDG", "Madagascar" },
                    { "454", "MW", "MWI", "Malawi" },
                    { "458", "MY", "MYS", "Malaysia" },
                    { "462", "MV", "MDV", "Maldives" },
                    { "466", "ML", "MLI", "Mali" },
                    { "470", "MT", "MLT", "Malta" },
                    { "584", "MH", "MHL", "Marshall Islands (the)" },
                    { "474", "MQ", "MTQ", "Martinique" },
                    { "478", "MR", "MRT", "Mauritania" },
                    { "540", "NC", "NCL", "New Caledonia" },
                    { "480", "MU", "MUS", "Mauritius" },
                    { "484", "MX", "MEX", "Mexico" },
                    { "583", "FM", "FSM", "Micronesia (Federated States of)" },
                    { "498", "MD", "MDA", "Moldova (the Republic of)" },
                    { "492", "MC", "MCO", "Monaco" },
                    { "496", "MN", "MNG", "Mongolia" },
                    { "499", "ME", "MNE", "Montenegro" },
                    { "500", "MS", "MSR", "Montserrat" },
                    { "504", "MA", "MAR", "Morocco" },
                    { "508", "MZ", "MOZ", "Mozambique" },
                    { "104", "MM", "MMR", "Myanmar" },
                    { "516", "NA", "NAM", "Namibia" },
                    { "520", "NR", "NRU", "Nauru" },
                    { "524", "NP", "NPL", "Nepal" },
                    { "175", "YT", "MYT", "Mayotte" },
                    { "662", "LC", "LCA", "Saint Lucia" },
                    { "666", "PM", "SPM", "Saint Pierre and Miquelon" },
                    { "430", "LR", "LBR", "Liberia" },
                    { "772", "TK", "TKL", "Tokelau" },
                    { "776", "TO", "TON", "Tonga" },
                    { "780", "TT", "TTO", "Trinidad and Tobago" },
                    { "788", "TN", "TUN", "Tunisia" },
                    { "792", "TR", "TUR", "Turkey" },
                    { "795", "TM", "TKM", "Turkmenistan" },
                    { "796", "TC", "TCA", "Turks and Caicos Islands (the)" },
                    { "798", "TV", "TUV", "Tuvalu" },
                    { "800", "UG", "UGA", "Uganda" },
                    { "804", "UA", "UKR", "Ukraine" },
                    { "784", "AE", "ARE", "United Arab Emirates (the)" },
                    { "826", "GB", "GBR", "United Kingdom of Great Britain and Northern Ireland (the)" },
                    { "581", "UM", "UMI", "United States Minor Outlying Islands (the)" },
                    { "840", "US", "USA", "United States of America (the)" },
                    { "858", "UY", "URY", "Uruguay" },
                    { "860", "UZ", "UZB", "Uzbekistan" },
                    { "548", "VU", "VUT", "Vanuatu" },
                    { "862", "VE", "VEN", "Venezuela (Bolivarian Republic of)" },
                    { "704", "VN", "VNM", "Viet Nam" },
                    { "092", "VG", "VGB", "Virgin Islands (British)" },
                    { "850", "VI", "VIR", "Virgin Islands (U.S.)" },
                    { "876", "WF", "WLF", "Wallis and Futuna" },
                    { "732", "EH", "ESH", "Western Sahara" },
                    { "887", "YE", "YEM", "Yemen" },
                    { "894", "ZM", "ZMB", "Zambia" },
                    { "716", "ZW", "ZWE", "Zimbabwe" },
                    { "248", "AX", "ALA", "Åland Islands" },
                    { "768", "TG", "TGO", "Togo" },
                    { "626", "TL", "TLS", "Timor-Leste" },
                    { "764", "TH", "THA", "Thailand" },
                    { "834", "TZ", "TZA", "Tanzania, United Republic of" },
                    { "670", "VC", "VCT", "Saint Vincent and the Grenadines" },
                    { "882", "WS", "WSM", "Samoa" },
                    { "674", "SM", "SMR", "San Marino" },
                    { "678", "ST", "STP", "Sao Tome and Principe" },
                    { "682", "SA", "SAU", "Saudi Arabia" },
                    { "686", "SN", "SEN", "Senegal" },
                    { "688", "RS", "SRB", "Serbia" },
                    { "690", "SC", "SYC", "Seychelles" },
                    { "694", "SL", "SLE", "Sierra Leone" },
                    { "702", "SG", "SGP", "Singapore" },
                    { "534", "SX", "SXM", "Sint Maarten (Dutch part)" },
                    { "703", "SK", "SVK", "Slovakia" },
                    { "705", "SI", "SVN", "Slovenia" },
                    { "663", "MF", "MAF", "Saint Martin (French part)" },
                    { "090", "SB", "SLB", "Solomon Islands" },
                    { "710", "ZA", "ZAF", "South Africa" },
                    { "239", "GS", "SGS", "South Georgia and the South Sandwich Islands" },
                    { "728", "SS", "SSD", "South Sudan" },
                    { "724", "ES", "ESP", "Spain" },
                    { "144", "LK", "LKA", "Sri Lanka" },
                    { "729", "SD", "SDN", "Sudan (the)" },
                    { "740", "SR", "SUR", "Suriname" },
                    { "744", "SJ", "SJM", "Svalbard and Jan Mayen" },
                    { "752", "SE", "SWE", "Sweden" },
                    { "756", "CH", "CHE", "Switzerland" },
                    { "760", "SY", "SYR", "Syrian Arab Republic" },
                    { "158", "TW", "TWN", "Taiwan (Province of China)" },
                    { "762", "TJ", "TJK", "Tajikistan" },
                    { "706", "SO", "SOM", "Somalia" },
                    { "426", "LS", "LSO", "Lesotho" },
                    { "438", "LI", "LIE", "Liechtenstein" },
                    { "428", "LV", "LVA", "Latvia" },
                    { "096", "BN", "BRN", "Brunei Darussalam" },
                    { "100", "BG", "BGR", "Bulgaria" },
                    { "854", "BF", "BFA", "Burkina Faso" },
                    { "108", "BI", "BDI", "Burundi" },
                    { "132", "CV", "CPV", "Cabo Verde" },
                    { "116", "KH", "KHM", "Cambodia" },
                    { "120", "CM", "CMR", "Cameroon" },
                    { "124", "CA", "CAN", "Canada" },
                    { "136", "KY", "CYM", "Cayman Islands (the)" },
                    { "140", "CF", "CAF", "Central African Republic (the)" },
                    { "148", "TD", "TCD", "Chad" },
                    { "152", "CL", "CHL", "Chile" },
                    { "156", "CN", "CHN", "China" },
                    { "162", "CX", "CXR", "Christmas Island" },
                    { "166", "CC", "CCK", "Cocos (Keeling) Islands (the)" },
                    { "170", "CO", "COL", "Colombia" },
                    { "174", "KM", "COM", "Comoros (the)" },
                    { "180", "CD", "COD", "Congo (the Democratic Republic of the)" },
                    { "178", "CG", "COG", "Congo (the)" },
                    { "184", "CK", "COK", "Cook Islands (the)" },
                    { "188", "CR", "CRI", "Costa Rica" },
                    { "191", "HR", "HRV", "Croatia" },
                    { "192", "CU", "CUB", "Cuba" },
                    { "531", "CW", "CUW", "Curaçao" },
                    { "196", "CY", "CYP", "Cyprus" },
                    { "422", "LB", "LBN", "Lebanon" },
                    { "384", "CI", "CIV", "Côte d'Ivoire" },
                    { "086", "IO", "IOT", "British Indian Ocean Territory (the)" },
                    { "076", "BR", "BRA", "Brazil" },
                    { "074", "BV", "BVT", "Bouvet Island" },
                    { "072", "BW", "BWA", "Botswana" },
                    { "004", "AF", "AFG", "Afghanistan" },
                    { "008", "AL", "ALB", "Albania" },
                    { "012", "DZ", "DZA", "Algeria" },
                    { "016", "AS", "ASM", "American Samoa" },
                    { "020", "AD", "AND", "Andorra" },
                    { "024", "AO", "AGO", "Angola" },
                    { "660", "AI", "AIA", "Anguilla" },
                    { "010", "AQ", "ATA", "Antarctica" },
                    { "028", "AG", "ATG", "Antigua and Barbuda" },
                    { "032", "AR", "ARG", "Argentina" },
                    { "051", "AM", "ARM", "Armenia" },
                    { "533", "AW", "ABW", "Aruba" },
                    { "036", "AU", "AUS", "Australia" },
                    { "208", "DK", "DNK", "Denmark" },
                    { "040", "AT", "AUT", "Austria" },
                    { "044", "BS", "BHS", "Bahamas (the)" },
                    { "048", "BH", "BHR", "Bahrain" },
                    { "050", "BD", "BGD", "Bangladesh" },
                    { "052", "BB", "BRB", "Barbados" },
                    { "112", "BY", "BLR", "Belarus" },
                    { "056", "BE", "BEL", "Belgium" },
                    { "084", "BZ", "BLZ", "Belize" },
                    { "204", "BJ", "BEN", "Benin" },
                    { "060", "BM", "BMU", "Bermuda" },
                    { "064", "BT", "BTN", "Bhutan" },
                    { "068", "BO", "BOL", "Bolivia (Plurinational State of)" },
                    { "535", "BQ", "BES", "Bonaire, Sint Eustatius and Saba" },
                    { "070", "BA", "BIH", "Bosnia and Herzegovina" },
                    { "031", "AZ", "AZE", "Azerbaijan" },
                    { "262", "DJ", "DJI", "Djibouti" },
                    { "203", "CZ", "CZE", "Czechia" },
                    { "214", "DO", "DOM", "Dominican Republic (the)" },
                    { "332", "HT", "HTI", "Haiti" },
                    { "334", "HM", "HMD", "Heard Island and McDonald Islands" },
                    { "212", "DM", "DMA", "Dominica" },
                    { "340", "HN", "HND", "Honduras" },
                    { "344", "HK", "HKG", "Hong Kong" },
                    { "348", "HU", "HUN", "Hungary" },
                    { "352", "IS", "ISL", "Iceland" },
                    { "356", "IN", "IND", "India" },
                    { "360", "ID", "IDN", "Indonesia" },
                    { "364", "IR", "IRN", "Iran (Islamic Republic of)" },
                    { "368", "IQ", "IRQ", "Iraq" },
                    { "372", "IE", "IRL", "Ireland" },
                    { "833", "IM", "IMN", "Isle of Man" },
                    { "376", "IL", "ISR", "Israel" },
                    { "380", "IT", "ITA", "Italy" },
                    { "388", "JM", "JAM", "Jamaica" },
                    { "392", "JP", "JPN", "Japan" },
                    { "832", "JE", "JEY", "Jersey" },
                    { "400", "JO", "JOR", "Jordan" },
                    { "398", "KZ", "KAZ", "Kazakhstan" },
                    { "404", "KE", "KEN", "Kenya" },
                    { "296", "KI", "KIR", "Kiribati" },
                    { "408", "KP", "PRK", "Korea (the Democratic People's Republic of)" },
                    { "410", "KR", "KOR", "Korea (the Republic of)" },
                    { "414", "KW", "KWT", "Kuwait" },
                    { "417", "KG", "KGZ", "Kyrgyzstan" },
                    { "418", "LA", "LAO", "Lao People's Democratic Republic (the)" },
                    { "328", "GY", "GUY", "Guyana" },
                    { "624", "GW", "GNB", "Guinea-Bissau" },
                    { "336", "VA", "VAT", "Holy See (the)" },
                    { "831", "GG", "GGY", "Guernsey" },
                    { "218", "EC", "ECU", "Ecuador" },
                    { "818", "EG", "EGY", "Egypt" },
                    { "222", "SV", "SLV", "El Salvador" },
                    { "226", "GQ", "GNQ", "Equatorial Guinea" },
                    { "232", "ER", "ERI", "Eritrea" },
                    { "233", "EE", "EST", "Estonia" },
                    { "748", "SZ", "SWZ", "Eswatini" },
                    { "231", "ET", "ETH", "Ethiopia" },
                    { "238", "FK", "FLK", "Falkland Islands (the) [Malvinas]" },
                    { "324", "GN", "GIN", "Guinea" },
                    { "242", "FJ", "FJI", "Fiji" },
                    { "246", "FI", "FIN", "Finland" },
                    { "250", "FR", "FRA", "France" },
                    { "254", "GF", "GUF", "French Guiana" },
                    { "234", "FO", "FRO", "Faroe Islands (the)" },
                    { "260", "TF", "ATF", "French Southern Territories (the)" },
                    { "320", "GT", "GTM", "Guatemala" },
                    { "258", "PF", "PYF", "French Polynesia" },
                    { "312", "GP", "GLP", "Guadeloupe" },
                    { "308", "GD", "GRD", "Grenada" },
                    { "304", "GL", "GRL", "Greenland" },
                    { "300", "GR", "GRC", "Greece" },
                    { "316", "GU", "GUM", "Guam" },
                    { "288", "GH", "GHA", "Ghana" },
                    { "276", "DE", "DEU", "Germany" },
                    { "268", "GE", "GEO", "Georgia" },
                    { "270", "GM", "GMB", "Gambia (the)" },
                    { "266", "GA", "GAB", "Gabon" },
                    { "292", "GI", "GIB", "Gibraltar" }
                });

            migrationBuilder.InsertData(
                table: "WsJobCategories",
                columns: new[] { "Id", "Title" },
                values: new object[,]
                {
                    { 9, "Civil Society" },
                    { 1, "Academia/Researcher" },
                    { 2, "Private Sector/Industry" },
                    { 3, "International Institution" },
                    { 4, "National Government" },
                    { 5, "Regional/Local Government" },
                    { 6, "Policymaker" },
                    { 7, "NGO" },
                    { 8, "Press" },
                    { 100, "Other" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7A" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7B" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA70", "4E59CEA1-FC55-49F5-BF30-4BC46A0DDA7C" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CountryId",
                table: "AspNetUsers",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_JobCategoryId",
                table: "AspNetUsers",
                column: "JobCategoryId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WsAlternatives_QuestionnaireId",
                table: "WsAlternatives",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_WsCriteria_QuestionnaireId",
                table: "WsCriteria",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderAnswers_AlternativeId",
                table: "WsStakeholderAnswers",
                column: "AlternativeId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderAnswers_CriterionId",
                table: "WsStakeholderAnswers",
                column: "CriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderAnswers_VoteId",
                table: "WsStakeholderAnswers",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderCriteriaImportance_CriterionId",
                table: "WsStakeholderCriteriaImportance",
                column: "CriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderCriteriaImportance_VoteId",
                table: "WsStakeholderCriteriaImportance",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_WsStakeholderVotes_ApplicationUserId",
                table: "WsStakeholderVotes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_STAKEHOLDERVOTE_WORKSPACEID",
                table: "WsStakeholderVotes",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WsWorkspaces_ParentId",
                table: "WsWorkspaces",
                column: "ParentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WsWorkspaces_QuestionnaireId",
                table: "WsWorkspaces",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_WsWorkspacesReports_WorkspaceId_Algorithm",
                table: "WsWorkspacesReports",
                columns: new[] { "WorkspaceId", "Algorithm" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "WsStakeholderAnswers");

            migrationBuilder.DropTable(
                name: "WsStakeholderCriteriaImportance");

            migrationBuilder.DropTable(
                name: "WsWorkspacesReports");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "WsAlternatives");

            migrationBuilder.DropTable(
                name: "WsCriteria");

            migrationBuilder.DropTable(
                name: "WsStakeholderVotes");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "WsWorkspaces");

            migrationBuilder.DropTable(
                name: "WsCountries");

            migrationBuilder.DropTable(
                name: "WsJobCategories");

            migrationBuilder.DropTable(
                name: "WsQuestionnaires");
        }
    }
}
