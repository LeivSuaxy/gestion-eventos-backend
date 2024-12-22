from rest_framework.permissions import BasePermission
from authentication.enum.roles import Role

class IsAdmin(BasePermission):
    def has_permission(self, request, view):
        return request.user and request.user == Role.ADMIN

class IsOrganizer(BasePermission):
    def has_permission(self, request, view):
        return request.user and request.user == Role.ORGANIZER

class IsUser(BasePermission):
    def has_permission(self, request, view):
        return request.user and request.user == Role.USER