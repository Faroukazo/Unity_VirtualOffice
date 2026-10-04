<?php
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

$token_secreto  = getenv("API_SECRET_TOKEN"); 
// Crear la conexión a la base de datos
$conn = new mysqli($servername, $username, $password, $dbname);

// Verificar la conexión
if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}

// Verificar el token
if (!isset($_POST['token']) || $_POST['token'] !== $token_secreto) {
    echo json_encode(array("status" => "error", "message" => "Acceso no autorizado"));
    exit();
}

// Recibe el nombre del usuario desde la solicitud POST
$nombreUsuario = isset($_POST['nombreUsuario']) ? $_POST['nombreUsuario'] : null;

if (empty($nombreUsuario)) {
    echo json_encode(['error' => 'El nombre de usuario es obligatorio.']);
    exit;
}

// Consultar si el usuario existe en la base de datos
$sql = "SELECT id FROM usuarios WHERE usuario = ?";
$stmt = $conn->prepare($sql);
$stmt->bind_param("s", $nombreUsuario);  // 's' es el tipo de dato para el string

$stmt->execute();
$stmt->store_result();  // Almacenar el resultado

if ($stmt->num_rows > 0) {
    // El usuario existe
    echo json_encode(['status' => 'success', 'message' => 'El usuario existe.']);
} else {
    // El usuario no existe
    echo json_encode(['status' => 'error', 'message' => 'El usuario no existe.']);
}

// Cerrar la conexión a la base de datos
$stmt->close();
$conn->close();
?>
