<?php
require 'vendor/autoload.php'; 
use Aws\S3\S3Client;
use Aws\Exception\AwsException;

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

$token_secreto = getenv("API_SECRET_TOKEN");

if (!isset($_GET['token']) || $_GET['token'] !== $token_secreto) {
    echo json_encode(["status" => "error", "message" => "Acceso no autorizado"]);
    exit();
}

if (!isset($_GET['file']) || empty($_GET['file'])) {
    echo json_encode(['error' => 'No se proporcionó el archivo para descargar.']);
    exit();
}

$nombreServidor = urldecode($_GET['file']);

$servername = "YOUR_DB_HOST_HERE";
$username = getenv("DB_USERNAME") ?: "admin";
$password = getenv("DB_PASSWORD");
$dbname = "oficinaVirtual";

$conn = new mysqli($servername, $username, $password, $dbname);
if ($conn->connect_error) {
    die("Conexión fallida: " . $conn->connect_error);
}

$sql = "SELECT nombre_original FROM archivos WHERE url LIKE ?";
$stmt = $conn->prepare($sql);
$likeUrl = "%/$nombreServidor";
$stmt->bind_param("s", $likeUrl);
$stmt->execute();
$stmt->bind_result($nombreOriginal);
$stmt->fetch();
$stmt->close();
$conn->close();

if (empty($nombreOriginal)) {
    $nombreOriginal = basename($nombreServidor);
}

$s3Client = new S3Client([
    'region'  => 'eu-west-3',     
    'version' => 'latest',
]);

$bucket = 'cloudoficinavirtual';  

try {
    $result = $s3Client->getObject([
        'Bucket' => $bucket,
        'Key'    => $nombreServidor,
    ]);

    header('Content-Type: ' . $result['ContentType']);
    header('Content-Disposition: attachment; filename="' . basename($nombreOriginal) . '"');
    header('Content-Length: ' . $result['ContentLength']);

    echo $result['Body'];
} catch (AwsException $e) {
    echo json_encode(['error' => 'Error al descargar el archivo: ' . $e->getMessage()]);
}
