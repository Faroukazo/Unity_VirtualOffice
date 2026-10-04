<?php
// Configuracion de la base de datos
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";


$token_secreto = getenv("API_SECRET_TOKEN");

// Crear la conexion
$conn = new mysqli($servername, $username, $password, $dbname);

// Verificar la conexion
if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}

// Verificar el token
if (!isset($_POST['token']) || $_POST['token'] !== $token_secreto) {
    echo json_encode(array("status" => "error", "message" => "Acceso no autorizado"));
    exit();
}

// Verificar si se reciben los parámetros de los campos
if (isset($_POST['nombre']) && isset($_POST['apellido']) && isset($_POST['usuario']) && isset($_POST['correo']) && isset($_POST['contrasena'])) {
    $nombre = $_POST['nombre'];
    $apellido = $_POST['apellido'];
    $usuario = $_POST['usuario'];
    $correo = $_POST['correo'];
    $contrasena = $_POST['contrasena'];

    // Validar que el usuario no exista
    $stmt = $conn->prepare("SELECT id FROM usuarios WHERE usuario=?");
    $stmt->bind_param("s", $usuario);
    $stmt->execute();
    $stmt->store_result();

    if ($stmt->num_rows > 0) {
        echo json_encode(array("status" => "error", "message" => "El nombre de usuario ya está en uso."));
        $stmt->close();
    } else {
        $contrasena_hash = password_hash($contrasena, PASSWORD_BCRYPT);
        $stmt = $conn->prepare("INSERT INTO usuarios (nombre, apellido, usuario, correo, contrasena_hash) VALUES (?, ?, ?, ?, ?)");
        $stmt->bind_param("sssss", $nombre, $apellido, $usuario, $correo, $contrasena_hash);

        if ($stmt->execute()) {
            echo json_encode(array("status" => "success", "message" => "Usuario registrado exitosamente"));
        } else {
            echo json_encode(array("status" => "error", "message" => "Error al registrar el usuario"));
        }
        $stmt->close();
    }
} else {
    echo json_encode(array("status" => "error", "message" => "Faltan parámetros"));
}

$conn->close();
?>
