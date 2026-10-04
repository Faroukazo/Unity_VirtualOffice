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

// Verificar si se reciben los parametros
if (isset($_POST['username']) && isset($_POST['password'])) {
    $user = $_POST['username'];
    $pass = $_POST['password'];

    $stmt = $conn->prepare("SELECT id, nombre, contrasena_hash FROM usuarios WHERE usuario=?");
    $stmt->bind_param("s", $user);
    $stmt->execute();
    $result = $stmt->get_result();

    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        
        if (password_verify($pass, $row['contrasena_hash'])) {
            echo json_encode(array("status" => "success", "message" => "Credenciales correctas"));
        } else {
            echo json_encode(array("status" => "error", "message" => "Contraseña incorrecta"));
        }
    } else {
        echo json_encode(array("status" => "error", "message" => "Usuario no encontrado"));
    }

    $stmt->close();
} else {
    echo json_encode(array("status" => "error", "message" => "Faltan parámetros"));
}

$conn->close();
?>
