def test_discount_factor():
    df = discount_factor(2)
    assert df == 8

def discount_factor(x: float):
    return 2 + 3 * x


if __name__ == '__main__':
    df = discount_factor(2)
    print(df)