from django.contrib.auth import get_user_model
from rest_framework import generics
from rest_framework.permissions import AllowAny

from authentication.api.serializer import UserSerializer

# Create your views here.
User = get_user_model()

class RegisterView(generics.CreateAPIView):
    queryset = User.objects.all()
    serializer_class = UserSerializer
    permission_classes = (AllowAny,)