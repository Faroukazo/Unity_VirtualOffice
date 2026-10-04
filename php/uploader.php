<?php
require 'vendor/autoload.php'; 

use Aws\S3\S3Client;
use Aws\Exception\AwsException;

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

$token_secreto = getenv("API_SECRET_TOKEN");

$s3Client = new S3Client([
    'region'  => 'eu-west-3',     
    'version' => 'latest',
]);

$bucket = 'cloudoficinavirtual';  

if (!isset($_POST['token']) || $_POST['token'] !== $token_secreto) {
    echo json_encode(array("status" => "error", "message" => "Acceso no autorizado"));
    exit();
}

if (!isset($_FILES['file']) || $_FILES['file']['error'] !== UPLOAD_ERR_OK) {
    http_response_code(400);
    echo json_encode(['error' => 'No se recibió el archivo o hubo un error al subirlo.']);
    exit;
}

$file = $_FILES['file'];
$tmpFile = $file['tmp_name'];
$nombreOriginal = $file['name'];


$nombreUnico = uniqid() . '_' . preg_replace('/[^a-zA-Z0-9_\.-]/', '_', $nombreOriginal);

try {
    $result = $s3Client->putObject([
        'Bucket'     => $bucket,
        'Key'        => $nombreUnico,  
        'SourceFile' => $tmpFile,
        'ACL'        => 'private',
    ]);

    echo json_encode([
        'message' => 'Archivo subido correctamente.',
        'url' => $result['ObjectURL'],
        'nombreOriginal' => $nombreOriginal,
        'nombreServidor' => $nombreUnico
    ]);
    

} catch (AwsException $e) {
    http_response_code(500);
    echo json_encode(['error' => $e->getMessage()]);
}
?>
