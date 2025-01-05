from rest_framework.permissions import BasePermission

'''
This design pattern is used to create permission classes dynamically.
Example:
IsAdmin = create_role_permission(Role.ADMIN.value)
Role is an enum that contains the previously defined application roles.
'''

class RolePermission(BasePermission):
    def __init__(self, role):
        self.required_role = role

    def has_permission(self, request, view):
        return request.user.is_authenticated and request.user.role == self.required_role


def create_role_permission(role):
    """
    Factory to create permission classes based on a specific role.
    """

    class DynamicRolePermission(RolePermission):
        def __init__(self):
            super().__init__(role)

    DynamicRolePermission.__name__ = f"Is{role.capitalize()}"  # Asigna un nombre dinámico a la clase.
    return DynamicRolePermission