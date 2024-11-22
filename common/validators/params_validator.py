def params_validator(data: dict, to_validate: list):
    validated: list = to_validate.copy()

    for var in to_validate:
        if var in data:
            validated.remove(var)

    if len(validated) != 0:
        return False, f'Missing required fields: {[element for element in validated]}'

    return True, 'Success'
