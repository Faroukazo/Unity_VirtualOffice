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

// Obtener todos los usuarios sin filtrar por sala
$sql = "SELECT nombre_usuario, sala FROM usuariosEnChatDeVoz";
$result = $conn->query($sql);

$usuarios = array(); // Siempre inicializamos el array

if ($result && $result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $usuarios[] = array(
            'nombre_usuario' => $row['nombre_usuario'],
            'sala' => $row['sala']
        );
    }
}

// Siempre respondemos con "success", aunque la lista esté vacía
echo json_encode(array("status" => "success", "usuarios" => $usuarios));

$conn->close();
?>
