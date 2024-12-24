from django.contrib.auth import get_user_model
from rest_framework import generics, status
from rest_framework.permissions import AllowAny, IsAuthenticated
from rest_framework.response import Response
from authentication.api.serializer import UserSerializer
from authentication.models import EventUser
from authentication.services.mails import send_verification_email
from django.core.cache import cache
from common.utils.codegen import generate_code
from rest_framework.decorators import api_view, permission_classes
from common.strategy.authpermission import IsUser, IsAdmin, IsOrganizer

# Create your views here.
User: EventUser = get_user_model()

class RegisterView(generics.CreateAPIView):
    queryset = User.objects.all()
    serializer_class = UserSerializer
    permission_classes = (AllowAny,)

    def create(self, request, *args, **kwargs):
        email = request.data.get('email')
        if User.objects.filter(email=email).exists():
            return Response({'detail': 'User with this email already exists.'}, status=status.HTTP_400_BAD_REQUEST)
        serializer = self.get_serializer(data=request.data)
        serializer.is_valid(raise_exception=True)
        user = serializer.save(is_active=False)
        code = generate_code()
        # This is while server is in dev mode
        print(code)
        send_verification_email(user, code)
        return Response({'detail': 'Verification email sent.'}, status=status.HTTP_200_OK)

class VerifyEmailView(generics.GenericAPIView):
    permission_classes = (AllowAny,)

    def post(self, request, *args, **kwargs):
        code = request.data.get('code')
        user_email = request.data.get('email')
        stored_code = cache.get(user_email)
        if not stored_code:
            return Response({'detail': 'Verification code expired.'}, status=status.HTTP_400_BAD_REQUEST)

        if code != stored_code:
            return Response({'detail': 'Invalid verification code.'}, status=status.HTTP_400_BAD_REQUEST)

        user = User.objects.get(email=user_email)
        user.is_active = True
        user.save()
        cache.delete(user_email)
        return Response({'detail': 'Register success.'}, status=status.HTTP_201_CREATED)