from rest_framework.views import APIView
from common.strategy.authpermission import IsAdmin
from administration.api.service import AdminEventService

# Create your views here.
# These endpoints are for administration purposes only

class AdminEventView(APIView):
    # permission_classes = [IsAdmin]

    @staticmethod
    def get(request):
        return AdminEventService.get()


    def post(self, request):
        pass

    def put(self, request):
        pass

    def delete(self, request):
        pass

