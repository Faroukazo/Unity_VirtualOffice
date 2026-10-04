<?php
/**
 * Plantilla de configuración.
 *
 * Copia este archivo como variables de entorno en tu servidor (nunca subas
 * un .env o config.php con valores reales al repositorio).
 *
 * Variables esperadas:
 *   DB_USERNAME        -> usuario de MySQL (por defecto "admin" si no se define)
 *   DB_PASSWORD        -> contraseña de MySQL
 *   API_SECRET_TOKEN   -> token compartido entre cliente (Unity) y servidor (PHP)
 *
 * El host de la base de datos se define directamente en cada script como
 * "YOUR_DB_HOST_HERE" — sustitúyelo por el endpoint real de tu instancia
 * (RDS, servidor propio, etc.) antes de desplegar.
 *
 * Ejemplo de uso en Apache (.htaccess o configuración de VirtualHost):
 *   SetEnv DB_PASSWORD "tu_password_real"
 *   SetEnv API_SECRET_TOKEN "tu_token_real"
 *
 * O en PHP-FPM / docker-compose, como variables de entorno del proceso.
 */
