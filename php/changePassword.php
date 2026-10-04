<?php
// Configuración de la base de datos
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

// Token secreto compartido
$token_secreto = getenv("API_SECRET_TOKEN");

// Crear la conexión
$conn = new mysqli($servername, $username, $password, $dbname);

// Verificar la conexión
if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}

// Verificar token
if (!isset($_POST['token']) || $_POST['token'] !== $token_secreto) {
    echo json_encode(array("status" => "error", "message" => "Acceso no autorizado"));
    exit();
}

// Verificar si se reciben los parámetros necesarios
if (isset($_POST['usuario']) && isset($_POST['nueva_contrasena'])) {
    $usuario = $_POST['usuario'];
    $nueva_contrasena = $_POST['nueva_contrasena'];

    // Hashear la nueva contraseña
    $nueva_hash = password_hash($nueva_contrasena, PASSWORD_BCRYPT);

    // Actualizar la contraseña del usuario
    $stmt = $conn->prepare("UPDATE usuarios SET contrasena_hash=? WHERE usuario=?");
    $stmt->bind_param("ss", $nueva_hash, $usuario);

    if ($stmt->execute()) {
        echo json_encode(array("status" => "success", "message" => "Contraseña actualizada correctamente"));
    } else {
        echo json_encode(array("status" => "error", "message" => "No se pudo actualizar la contraseña"));
    }

    $stmt->close();
} else {
    echo json_encode(array("status" => "error", "message" => "Faltan parámetros"));
}

$conn->close();
?>
