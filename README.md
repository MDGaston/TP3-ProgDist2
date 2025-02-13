# TP1-ProgDist3

# Instrucciones Trabajo práctico inicial

Primero hay que abrir la consola y situarse en el directorio

```bash
cd .../Tp2TrackingApi
```

Una vez situados correr el docker compose

```bash
docker compose up
```

# Para probar el proyecto entrar importar la coleccion de Postman

Es importante primero ejecutar el endpoint de login, para realizar las pruebas iniciales loguear con el siguiente usuario y contraseña

```bash
{
  "username": "admin",
  "password": "admin"
}
```

# Una vez ejecutado esto se generara el token que debe de ser utilizado en las request a la app de usuarios.

Se puede guardar el Token obtenido en la variable en postman Token(principal sugerencia) o agregarlo directamente en cada pedido en la seccion de "Bearer Token" en la parte de Authorization

# Updates

En esta version del tp se agregaron los endpoints correspondientes para poder probar la app de track de usuarios y la app de la Blacklist, ademas se agrego una aplicacion de consola TrackingConsumer que sera el encargado de logear los mensajes de las colas y de indicar si es de estado critico en caso de que el usuario ingrese a una URL blacklisteada. Estos logs se pueden encontrar en:

\Tp3 Replication\Logs

Para las implementaciones correspondientes a la tercera entrega se implemento postgreSQL en reemplazo de SQL server y se implemento un nuevo contenedor para replicar la base de datos principal. Para realizar los test correspondientes en verificacion de la replicacion ingresar los siguientes comandos :

```bash
# Listar contenedores en ejecución

docker ps
# Acceder al contenedor de PostgreSQL

docker exec -it postgres_replica bash

# Conectarse alguna de las bases de datos usando psql Utilizar la password del docker compose "Admin123456"

psql -U postgres -d UserDB

psql -U postgres -d TrackingDB

# Listar tablas

\dt
# Consultar datos de la tabla TrackingEvents

SELECT * FROM "TrackingEvents"

# Salir de psql

\q

# Salir del contenedor
exit
```
Documentacion de la tercera entrega :
https://docs.google.com/document/d/12Qjn0zf4aDgDflYNdqWPr5vZWrHTULTj8ycOeMDmLls/edit?usp=sharing
