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

// Verificar si se reciben los parámetros
if (
    isset($_POST['nombre']) &&
    isset($_POST['apellido']) &&
    isset($_POST['usuario']) &&
    isset($_POST['correo'])
) {
    $nombre = $_POST['nombre'];
    $apellido = $_POST['apellido'];
    $usuario = $_POST['usuario'];
    $correo = $_POST['correo'];

    // Buscar al usuario con todos esos datos
    $stmt = $conn->prepare("SELECT id FROM usuarios WHERE nombre=? AND apellido=? AND usuario=? AND correo=?");
    $stmt->bind_param("ssss", $nombre, $apellido, $usuario, $correo);
    $stmt->execute();
    $stmt->store_result();

    if ($stmt->num_rows > 0) {
        echo json_encode(array("status" => "success", "message" => "Datos verificados correctamente"));
    } else {
        echo json_encode(array("status" => "error", "message" => "No se ha encontrado ninguna cuenta con esos datos"));
    }

    $stmt->close();
} else {
    echo json_encode(array("status" => "error", "message" => "Faltan parámetros"));
}

$conn->close();
?>
