CREATE DATABASE IF NOT EXISTS AcademiaDB;
USE AcademiaDB;

-- 1. Tabla de Departamentos
CREATE TABLE Departamentos (
    id_departamento INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    presupuesto DECIMAL(12, 2) DEFAULT 0.00,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP
);

-- 2. Tabla de Profesores
CREATE TABLE Profesores (
    id_profesor INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    apellido VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE,
    id_departamento INT,
    FOREIGN KEY (id_departamento) REFERENCES Departamentos(id_departamento)
);

-- 3. Tabla de Cursos
CREATE TABLE Cursos (
    id_curso INT AUTO_INCREMENT PRIMARY KEY,
    titulo VARCHAR(150) NOT NULL,
    creditos INT CHECK (creditos > 0),
    id_profesor INT,
    FOREIGN KEY (id_profesor) REFERENCES Profesores(id_profesor)
);

-- 4. Tabla de Estudiantes
CREATE TABLE Estudiantes (
    id_estudiante INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL,
    fecha_nacimiento DATE,
    activo BOOLEAN DEFAULT TRUE
);

-- 5. Tabla de Inscripciones (Muchos a Muchos entre Estudiantes y Cursos)
CREATE TABLE Inscripciones (
    id_estudiante INT,
    id_curso INT,
    fecha_inscripcion DATE,
    nota_final DECIMAL(4, 2),
    PRIMARY KEY (id_estudiante, id_curso),
    FOREIGN KEY (id_estudiante) REFERENCES Estudiantes(id_estudiante),
    FOREIGN KEY (id_curso) REFERENCES Cursos(id_curso)
);

-- Insertar datos de prueba básicos
INSERT INTO Departamentos (nombre, presupuesto) VALUES ('Informática', 50000.00), ('Matemáticas', 30000.00);
INSERT INTO Profesores (nombre, apellido, email, id_departamento) VALUES ('Ana', 'García', 'ana@academia.com', 1);
INSERT INTO Cursos (titulo, creditos, id_profesor) VALUES ('Bases de Datos', 6, 1), ('Programación C#', 8, 1);
INSERT INTO Estudiantes (nombre, fecha_nacimiento) VALUES ('Juan Pérez', '2002-05-15');
INSERT INTO Inscripciones (id_estudiante, id_curso, fecha_inscripcion) VALUES (1, 1, CURDATE());