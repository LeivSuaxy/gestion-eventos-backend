from rest_framework.permissions import BasePermission
from authentication.enum.role import Role

class IsOrganizer(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.ORGANIZER

class IsParticipant(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.PARTICIPANT

class IsAdmin(BasePermission):
    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == Role.ADMIN