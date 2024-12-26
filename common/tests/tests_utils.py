from common.utils.codegen import generate_code

def test_generate_code():
    code = generate_code()
    assert len(code) == 6
    assert code.isalnum()
    assert code.isupper()