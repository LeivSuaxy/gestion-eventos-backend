def generate_code(length=6):
    import random
    import string

    return ''.join(random.choices(string.ascii_uppercase + string.digits, k=length))