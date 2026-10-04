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

// 2. Consultar las notificaciones del usuario (archivos enviados o recibidos)
$stmt = $conn->prepare("SELECT u.nombre AS remitente_nombre, a.asunto, a.url, a.leido 
                        FROM archivos a
                        JOIN usuarios u ON a.remitente_id = u.id
                        WHERE a.destinatario_id = ?");
$stmt->bind_param("i", $usuario_id);
$stmt->execute();

$result = $stmt->get_result();

$notificaciones = array();

while ($row = $result->fetch_assoc()) {
    $notificaciones[] = array(
        "nombre" => $row['remitente_nombre'],
        "asunto" => $row['asunto'],
        "url" => $row['url'],
        "leido" => $row['leido']
    );
}

// 3. Devolver el resultado como JSON
echo json_encode($notificaciones);

$stmt->close();
$conn->close();
?>