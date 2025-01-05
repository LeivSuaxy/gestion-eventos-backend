from common.guards.factory_guard import create_role_permission
from authentication.enum.roles import Role

IsAdmin = create_role_permission(Role.ADMIN.value)
IsOrganizer = create_role_permission(Role.ORGANIZER.value)
IsUser = create_role_permission(Role.USER.value)