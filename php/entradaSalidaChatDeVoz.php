<?php
// Configuración de la base de datos
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

$token_secreto = getenv("API_SECRET_TOKEN");

// Crear la conexión
$conn = new mysqli($servername, $username, $password, $dbname);

// Verificar la conexión
if ($conn->connect_error) {
    die(json_encode(array("status" => "error", "message" => "Conexión fallida")));
}

// Verificar el token
if (!isset($_POST['token']) || $_POST['token'] !== $token_secreto) {
    echo json_encode(array("status" => "error", "message" => "Acceso no autorizado"));
    exit();
}

// Verificar si se reciben los parámetros
if (!isset($_POST['usuario']) || !isset($_POST['nombreSala']) || !isset($_POST['action'])) {
    echo json_encode(array("status" => "error", "message" => "Faltan parámetros"));
    exit();
}

$usuario = $_POST['usuario'];
$nombreSala = $_POST['nombreSala'];
$action = $_POST['action'];

// 1. Obtener el ID y nombre del usuario
$stmt = $conn->prepare("SELECT id, usuario FROM usuarios WHERE usuario = ?");
$stmt->bind_param("s", $usuario);
$stmt->execute();
$result = $stmt->get_result();

if ($result->num_rows === 0) {
    echo json_encode(array("status" => "error", "message" => "Usuario no encontrado"));
    exit();
}

$row = $result->fetch_assoc();
$id_usuario = $row['id'];
$nombre_usuario = $row['usuario'];  // Obtenemos el nombre del usuario

// 2. Comprobación para registrar la sala creada por el usuario
if ($nombreSala !== "Lobby" && $nombreSala !== "Oficina") {
    $stmt = $conn->prepare("SELECT COUNT(*) as total FROM usuariosEnChatDeVoz WHERE sala = ?");
    $stmt->bind_param("s", $nombreSala);
    $stmt->execute();
    $result = $stmt->get_result();
    $row = $result->fetch_assoc();

    if ($row['total'] == 0) { // La sala no existe en usuariosEnChatDeVoz, registrar en registroSalasCreadasPorUsuario
        $stmt = $conn->prepare("INSERT INTO registroSalasCreadasPorUsuario (id_usuario, sala) VALUES (?, ?)");
        $stmt->bind_param("is", $id_usuario, $nombreSala);
        $stmt->execute();
    }
}

// 3. Registrar la acción (entrar o salir)
if ($action == "entrar") {
    // Registrar en registroDeEntrada
    $stmt = $conn->prepare("INSERT INTO registroDeEntrada (id_usuario, sala) VALUES (?, ?)");
    $stmt->bind_param("is", $id_usuario, $nombreSala);
    $stmt->execute();
    
    // Insertar en usuariosEnChatDeVoz con el nombre del usuario
    $stmt = $conn->prepare("INSERT INTO usuariosEnChatDeVoz (id_usuario, nombre_usuario, sala) VALUES (?, ?, ?)");
    $stmt->bind_param("iss", $id_usuario, $nombre_usuario, $nombreSala);  // Cambié la consulta para incluir nombre_usuario
    $stmt->execute();
    
    echo json_encode(array("status" => "success", "message" => "Usuario añadido al chat de voz"));
} elseif ($action == "salir") {
    // Registrar en registroDeSalida
    $stmt = $conn->prepare("INSERT INTO registroDeSalida (id_usuario, sala) VALUES (?, ?)");
    $stmt->bind_param("is", $id_usuario, $nombreSala);
    $stmt->execute();

    // Eliminar de usuariosEnChatDeVoz
    $stmt = $conn->prepare("DELETE FROM usuariosEnChatDeVoz WHERE id_usuario = ? ");
    $stmt->bind_param("s", $id_usuario);
    $stmt->execute();

    echo json_encode(array("status" => "success", "message" => "Usuario salió del chat de voz"));
}

$conn->close();
?>
