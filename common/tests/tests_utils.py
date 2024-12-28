def test_generate_code():
    from common.utils.codegen import generate_code
    code = generate_code()
    assert len(code) == 6
    assert code.isalnum()
    assert code.isupper()

def test_cache_utils(cache):
    from django.core.cache import cache
    from common.utils.cache_utils import delete_cache
    cache.set('test', 'test')
    assert cache.get('test') == 'test'
    delete_cache('test')
    assert cache.get('test') is None


