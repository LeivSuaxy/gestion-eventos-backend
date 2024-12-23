from rest_framework.permissions import BasePermission
from authentication.enum.roles import Role

class IsAdmin(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.ADMIN.value

class IsOrganizer(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.ORGANIZER.value

class IsUser(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.USER.value