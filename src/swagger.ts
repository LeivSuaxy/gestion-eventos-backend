import { DocumentBuilder, SwaggerModule } from '@nestjs/swagger';

export const DocapiBuilder = (app) => {
  const config = new DocumentBuilder()
    .setTitle('E-Events API Docs')
    .setDescription('API E-Events DOCS')
    .setVersion('v0')
    .addBearerAuth()
    .build();

  const document = SwaggerModule.createDocument(app, config);

  SwaggerModule.setup('docs', app, document);
};
