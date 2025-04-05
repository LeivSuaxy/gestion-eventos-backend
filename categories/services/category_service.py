from common.patterns.decorators.singleton import singleton
from common.services.base import BaseService

from categories.models import Category

@singleton
class CategoryService(BaseService):
    model = Category