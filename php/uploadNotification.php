<?php
$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

$token_secreto = getenv("API_SECRET_TOKEN");

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

// Recibe los datos enviados en el formulario
$nombreUsuario = isset($_POST['nombreUsuario']) ? $_POST['nombreUsuario'] : null;
$nombreDestinatario = isset($_POST['nombreDestinatario']) ? $_POST['nombreDestinatario'] : null;
$asunto = isset($_POST['asunto']) ? $_POST['asunto'] : null;
$url = isset($_POST['url']) ? $_POST['url'] : null;
$nombreOriginal = isset($_POST['nombreOriginal']) ? $_POST['nombreOriginal'] : null;
$nombreServidor = isset($_POST['nombreservidor']) ? $_POST['nombreservidor'] : null;

if (empty($nombreUsuario) || empty($nombreDestinatario) || empty($asunto) || empty($url) || empty($nombreOriginal) || empty($nombreServidor)) {
    echo json_encode(['error' => 'Todos los campos son obligatorios.']);
    exit;
}

// Consultar el ID del remitente (nombreUsuario)
$sqlRemitente = "SELECT id FROM usuarios WHERE usuario = ?";
$stmtRemitente = $conn->prepare($sqlRemitente);
$stmtRemitente->bind_param("s", $nombreUsuario);
$stmtRemitente->execute();
$resultRemitente = $stmtRemitente->get_result();

if ($resultRemitente->num_rows == 0) {
    echo json_encode(['error' => 'El remitente no se encuentra registrado.']);
    exit;
}

$rowRemitente = $resultRemitente->fetch_assoc();
$remitenteId = $rowRemitente['id'];

// Consultar el ID del destinatario (nombreDestinatario)
$sqlDestinatario = "SELECT id FROM usuarios WHERE usuario = ?";
$stmtDestinatario = $conn->prepare($sqlDestinatario);
$stmtDestinatario->bind_param("s", $nombreDestinatario);
$stmtDestinatario->execute();
$resultDestinatario = $stmtDestinatario->get_result();

if ($resultDestinatario->num_rows == 0) {
    echo json_encode(['error' => 'El destinatario no se encuentra registrado.']);
    exit;
}

$rowDestinatario = $resultDestinatario->fetch_assoc();
$destinatarioId = $rowDestinatario['id'];

// Insertar los datos en la tabla 'archivos' incluyendo el asunto y el nuevo campo 'nombreOriginal' y 'nombreServidor'
$stmt = $conn->prepare("INSERT INTO archivos (remitente_id, destinatario_id, asunto, url, nombre_original, nombre_servidor) VALUES (?, ?, ?, ?, ?, ?)");
$stmt->bind_param("iissss", $remitenteId, $destinatarioId, $asunto, $url, $nombreOriginal, $nombreServidor);

if ($stmt->execute()) {
    echo json_encode([
        'message' => 'Archivo registrado correctamente.',
        'remitente_id' => $remitenteId,
        'destinatario_id' => $destinatarioId,
        'asunto' => $asunto,
        'url' => $url,
        'nombre_original' => $nombreOriginal,
        'nombre_servidor' => $nombreServidor
    ]);
} else {
    echo json_encode(['error' => 'Error al insertar el archivo: ' . $conn->error]);
}

// Cerrar la conexión a la base de datos
$stmt->close();
$conn->close();
?>
