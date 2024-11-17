from rest_framework.authtoken.views import ObtainAuthToken
from rest_framework.response import Response
from rest_framework.authtoken.models import Token
from rest_framework import status

# Create your views here.

class Login(ObtainAuthToken):

    def post(self, request, *args, **kwargs):
        login_serializer = self.serializer_class(data=request.data, context={'request': request})

        if login_serializer.is_valid():
            user = login_serializer.validated_data['user']
            token, created = Token.objects.get_or_create(user=user)
            if created:
                print(token)
                print(token.key)
                return Response({
                    'token': token.key
                }, status = status.HTTP_201_CREATED)

        return Response(login_serializer.errors, status=status.HTTP_400_BAD_REQUEST)
