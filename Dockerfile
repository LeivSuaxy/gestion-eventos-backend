# Usa una imagen base de Python 3.12
FROM python:3.12.8-alpine3.21

# Establece el directorio de trabajo en el contenedor
WORKDIR /app

RUN mkdir -p /root/.cache/pip

# Copia el archivo de requerimientos y el código fuente al contenedor
COPY requirements.txt requirements.txt
COPY . .

# Instala las dependencias
RUN pip install --cache-dir /root/.cache/pip -r requirements.txt

# Expone el puerto en el que correrá la aplicación (ajusta según sea necesario)
EXPOSE 8000

# Comando para correr la aplicación (ajusta según sea necesario)
CMD ["python", "manage.py", "runserver", "localhost:8000"]