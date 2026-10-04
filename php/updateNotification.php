<?php
// Configuración de la base de datos
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

// Token compartido para validación simple
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

// Verificar si se recibió el username
if (!isset($_POST['username'])) {
    echo json_encode(array("status" => "error", "message" => "Falta el nombre de usuario"));
    exit();
}

$username = $_POST['username'];

// 1. Obtener el ID del usuario
$stmt = $conn->prepare("SELECT id FROM usuarios WHERE usuario = ?");
$stmt->bind_param("s", $username);
$stmt->execute();
$result = $stmt->get_result();

// Verificar si el usuario existe
if ($result->num_rows > 0) {
    $row = $result->fetch_assoc();
    $usuario_id = $row['id'];
} else {
    echo json_encode(array("status" => "error", "message" => "Usuario no encontrado"));
    exit();
}

$stmt->close();

// 2. Actualizamos la tabla de archivos con el id para que sea true el leido.
$stmt = $conn->prepare("UPDATE archivos SET leido = 1 WHERE destinatario_id = ?");
$stmt->bind_param("i", $usuario_id);
if ($stmt->execute()) {
    echo json_encode(array("status" => "success", "message" => "Notificaciones marcadas como leídas"));
} else {
    echo json_encode(array("status" => "error", "message" => "Error al actualizar: " . $stmt->error));
}

$stmt->close();
$conn->close();
?>