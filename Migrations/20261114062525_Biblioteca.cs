using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flash_Lector.Migrations;

/// <inheritdoc />
public partial class _20261114062525_Biblioteca : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categorias",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categorias", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Usuarios",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Nombres = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Apellidos = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                ContrasenaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Rol = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Usuarios", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Obras",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Titulo = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                NombreAutor = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Sinopsis = table.Column<string>(type: "nvarchar(1600)", maxLength: 1600, nullable: false),
                Formato = table.Column<int>(type: "int", nullable: false),
                Estado = table.Column<int>(type: "int", nullable: false),
                Paginas = table.Column<int>(type: "int", nullable: false),
                RutaArchivo = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                CategoriaId = table.Column<int>(type: "int", nullable: false),
                CreadorId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Obras", x => x.Id);
                table.ForeignKey(
                    name: "FK_Obras_Categorias_CategoriaId",
                    column: x => x.CategoriaId,
                    principalTable: "Categorias",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Obras_Usuarios_CreadorId",
                    column: x => x.CreadorId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "SolicitudesRol",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<int>(type: "int", nullable: false),
                RolSolicitado = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                Estado = table.Column<int>(type: "int", nullable: false),
                NotaAdministrador = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SolicitudesRol", x => x.Id);
                table.ForeignKey(
                    name: "FK_SolicitudesRol_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Favoritos",
            columns: table => new
            {
                UsuarioId = table.Column<int>(type: "int", nullable: false),
                ObraId = table.Column<int>(type: "int", nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Favoritos", x => new { x.UsuarioId, x.ObraId });
                table.ForeignKey(
                    name: "FK_Favoritos_Obras_ObraId",
                    column: x => x.ObraId,
                    principalTable: "Obras",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Favoritos_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ProgresosLectura",
            columns: table => new
            {
                UsuarioId = table.Column<int>(type: "int", nullable: false),
                ObraId = table.Column<int>(type: "int", nullable: false),
                PaginaActual = table.Column<int>(type: "int", nullable: false),
                FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProgresosLectura", x => new { x.UsuarioId, x.ObraId });
                table.ForeignKey(
                    name: "FK_ProgresosLectura_Obras_ObraId",
                    column: x => x.ObraId,
                    principalTable: "Obras",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_ProgresosLectura_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Resenas",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UsuarioId = table.Column<int>(type: "int", nullable: false),
                ObraId = table.Column<int>(type: "int", nullable: false),
                Calificacion = table.Column<int>(type: "int", nullable: false),
                Comentario = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                EsVisible = table.Column<bool>(type: "bit", nullable: false),
                FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Resenas", x => x.Id);
                table.ForeignKey(
                    name: "FK_Resenas_Obras_ObraId",
                    column: x => x.ObraId,
                    principalTable: "Obras",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Resenas_Usuarios_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "Usuarios",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Favoritos_ObraId",
            table: "Favoritos",
            column: "ObraId");

        migrationBuilder.CreateIndex(
            name: "IX_Obras_CategoriaId",
            table: "Obras",
            column: "CategoriaId");

        migrationBuilder.CreateIndex(
            name: "IX_Obras_CreadorId",
            table: "Obras",
            column: "CreadorId");

        migrationBuilder.CreateIndex(
            name: "IX_ProgresosLectura_ObraId",
            table: "ProgresosLectura",
            column: "ObraId");

        migrationBuilder.CreateIndex(
            name: "IX_Resenas_ObraId",
            table: "Resenas",
            column: "ObraId");

        migrationBuilder.CreateIndex(
            name: "IX_Resenas_UsuarioId",
            table: "Resenas",
            column: "UsuarioId");

        migrationBuilder.CreateIndex(
            name: "IX_SolicitudesRol_UsuarioId",
            table: "SolicitudesRol",
            column: "UsuarioId");

        migrationBuilder.CreateIndex(
            name: "IX_Usuarios_Correo",
            table: "Usuarios",
            column: "Correo",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Favoritos");

        migrationBuilder.DropTable(
            name: "ProgresosLectura");

        migrationBuilder.DropTable(
            name: "Resenas");

        migrationBuilder.DropTable(
            name: "SolicitudesRol");

        migrationBuilder.DropTable(
            name: "Obras");

        migrationBuilder.DropTable(
            name: "Categorias");

        migrationBuilder.DropTable(
            name: "Usuarios");
    }
}
