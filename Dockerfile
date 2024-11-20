# Usa una imagen base de Python 3.12
FROM python:3.12-slim

# Establece el directorio de trabajo en el contenedor
WORKDIR /app

# Copia el archivo de requerimientos y el código fuente al contenedor
COPY requirements.txt requirements.txt
COPY . .

# Instala las dependencias
RUN pip install --no-cache-dir -r requirements.txt

# Expone el puerto en el que correrá la aplicación (ajusta según sea necesario)
EXPOSE 8000

# Comando para correr la aplicación (ajusta según sea necesario)
CMD ["python", "app.py"]