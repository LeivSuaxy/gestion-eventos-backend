from django.db import models

class BaseService:
    model: models.Model = None

    def get_all(self):
        return self.model.objects.all()

    def get_by_id(self, id):
        return self.model.objects.get(id=id)

    def create(self, data):
        return self.model.objects.create(**data)

    def update(self, id, data):
        return self.model.objects.filter(id=id).update(**data)

    def delete(self, id):
        return self.model.objects.filter(id=id).delete()


