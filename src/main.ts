import { NestFactory } from '@nestjs/core';
import { AppModule } from './app.module';
import { DocapiBuilder } from './swagger';

async function bootstrap() {
  const app = await NestFactory.create(AppModule);

  app.enableCors({
    origin: 'http://localhost:3000',
    methods: 'GET,HEAD,PUT,PATCH,POST,DELETE',
    credentials: true,
  });

  DocapiBuilder(app);

  await app.listen(process.env.PORT ?? 3000);

  console.log(`Escuchando en el puerto ${process.env.PORT ?? 3000}`);
}
bootstrap();
