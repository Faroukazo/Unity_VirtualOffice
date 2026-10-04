CREATE TABLE usuarios (
    id INT AUTO_INCREMENT PRIMARY KEY, 
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    usuario VARCHAR(100) UNIQUE NOT NULL,  
    correo VARCHAR(200) NOT NULL,
    contrasena_hash VARCHAR(255) NOT NULL,  
    creado_el TIMESTAMP DEFAULT CURRENT_TIMESTAMP  
);

CREATE TABLE archivos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    remitente_id INT NOT NULL,
    destinatario_id INT NOT NULL,
	asunto VARCHAR(255) NOT NULL,
	nombre_original VARCHAR(255),
	nombre_servidor VARCHAR(255),
    url VARCHAR(255),
    fecha_subida TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    leido BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (remitente_id) REFERENCES usuarios(id),
    FOREIGN KEY (destinatario_id) REFERENCES usuarios(id)
);

--Tablas para la gestion del chat de audio.

CREATE TABLE usuariosEnChatDeVoz(
	id INT AUTO_INCREMENT PRIMARY KEY,
	id_usuario INT not null,
	nombre_usuario VARCHAR(100) not null,
	sala VARCHAR(255) not null,
	fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	FOREIGN key (id_usuario) REFERENCES usuarios(id)
);

CREATE TABLE registroDeEntrada(
	id INT AUTO_INCREMENT PRIMARY KEY,
	id_usuario INT not null,
	sala VARCHAR(255) not null,
	fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	FOREIGN key (id_usuario) REFERENCES usuarios(id)
);

CREATE TABLE registroDeSalida(
	id INT AUTO_INCREMENT PRIMARY KEY,
	id_usuario INT not null,
	sala VARCHAR(255) not null,
	fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	FOREIGN key (id_usuario) REFERENCES usuarios(id)
);

CREATE TABLE registroSalasCreadasPorUsuario(
	id INT AUTO_INCREMENT PRIMARY KEY,
	id_usuario INT not null,
	sala VARCHAR(255) not null,
	fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	FOREIGN key (id_usuario) REFERENCES usuarios(id)
);