def singleton(cls):
    """
    Decorator that ensures a class is only instantiated once.

    Usage:
        @singleton
        class MyService(BaseService):
            model = MyModel
    """
    instances = {}

    def get_instance(*args, **kwargs):
        if cls not in instances:
            instances[cls] = cls(*args, **kwargs)
        return instances[cls]

    return get_instance