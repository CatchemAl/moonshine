def get_discount_factor(rate: float, tenor: float) -> float:
    """The discount factor

    :param rate: The interest rate.
    :type rate: float
    :param tenor: The tenor (in years).
    :type tenor: float
    :return: The discount factor.
    :rtype: float
    """
    assert rate == 5.0

    return (1 + rate) ** -tenor


if __name__ == "__main__":
    df = get_discount_factor(0.02, 10)
    print(df)
