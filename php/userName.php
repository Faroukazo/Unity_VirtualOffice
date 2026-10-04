<?php

$SECRET_TOKEN = getenv("API_SECRET_TOKEN"); 

// Verificar que el token es valido
if (!isset($_POST['token']) || $_POST['token'] !== $SECRET_TOKEN) {
    echo json_encode(array("status" => "error", "message" => "Token inválido o faltante"));
    exit;
}

// Configuracion de la base de datos
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

// Crear la conexion
$conn = new mysqli($servername, $username, $password, $dbname);

// Verificar la conexion
if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}    

// Preparar la consulta sin parametros
$stmt = $conn->prepare("SELECT nombre, usuario FROM usuarios");
$stmt->execute();
$result = $stmt->get_result();

$usuarios = array();

if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $usuarios[] = array(
            "nombre" => $row["nombre"],
            "usuario" => $row["usuario"]
        );
    }
}

// Devolver la lista (puede estar vacia si no hay usuarios)
echo json_encode($usuarios);

$stmt->close();
$conn->close();
?>
