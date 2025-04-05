from common.patterns.decorators.singleton import singleton
from common.services.base import BaseService
from categories.models import SubCategory

@singleton
class SubCategoryService(BaseService):
    model = SubCategory