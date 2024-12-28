def delete_cache(name: str):
    from django.core.cache import cache
    if cache.get(name):
        cache.delete(name)