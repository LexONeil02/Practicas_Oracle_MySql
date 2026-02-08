using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practicas_Oracle.Migrations
{
    /// <inheritdoc />
    public partial class EstructuraFinalOracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "departamentos",
                columns: table => new
                {
                    id_departamento = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    presupuesto = table.Column<decimal>(type: "NUMBER(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    fecha_creacion = table.Column<DateTime>(type: "DATE", nullable: true, defaultValueSql: "CURRENT_DATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DEPARTAMENTOS", x => x.id_departamento);
                });

            migrationBuilder.CreateTable(
                name: "estudiantes",
                columns: table => new
                {
                    id_estudiante = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    fecha_nacimiento = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    activo = table.Column<bool>(type: "NUMBER(1)", nullable: true, defaultValueSql: "'1'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ESTUDIANTES", x => x.id_estudiante);
                });

            migrationBuilder.CreateTable(
                name: "profesores",
                columns: table => new
                {
                    id_profesor = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    nombre = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    apellido = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    id_departamento = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROFESORES", x => x.id_profesor);
                    table.ForeignKey(
                        name: "profesores_ibfk_1",
                        column: x => x.id_departamento,
                        principalTable: "departamentos",
                        principalColumn: "id_departamento");
                });

            migrationBuilder.CreateTable(
                name: "cursos",
                columns: table => new
                {
                    id_curso = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    titulo = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    creditos = table.Column<int>(type: "NUMBER(10)", nullable: true),
                    id_profesor = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CURSOS", x => x.id_curso);
                    table.ForeignKey(
                        name: "cursos_ibfk_1",
                        column: x => x.id_profesor,
                        principalTable: "profesores",
                        principalColumn: "id_profesor");
                });

            migrationBuilder.CreateTable(
                name: "inscripciones",
                columns: table => new
                {
                    id_estudiante = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    id_curso = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    fecha_inscripcion = table.Column<string>(type: "NVARCHAR2(10)", nullable: true),
                    nota_final = table.Column<decimal>(type: "DECIMAL(4,2)", precision: 4, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INSCRIPCIONES", x => new { x.id_estudiante, x.id_curso });
                    table.ForeignKey(
                        name: "inscripciones_ibfk_1",
                        column: x => x.id_estudiante,
                        principalTable: "estudiantes",
                        principalColumn: "id_estudiante");
                    table.ForeignKey(
                        name: "inscripciones_ibfk_2",
                        column: x => x.id_curso,
                        principalTable: "cursos",
                        principalColumn: "id_curso");
                });

            migrationBuilder.CreateIndex(
                name: "id_profesor",
                table: "cursos",
                column: "id_profesor");

            migrationBuilder.CreateIndex(
                name: "id_curso",
                table: "inscripciones",
                column: "id_curso");

            migrationBuilder.CreateIndex(
                name: "email",
                table: "profesores",
                column: "email",
                unique: true,
                filter: "\"email\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "id_departamento",
                table: "profesores",
                column: "id_departamento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inscripciones");

            migrationBuilder.DropTable(
                name: "estudiantes");

            migrationBuilder.DropTable(
                name: "cursos");

            migrationBuilder.DropTable(
                name: "profesores");

            migrationBuilder.DropTable(
                name: "departamentos");
        }
    }
}
