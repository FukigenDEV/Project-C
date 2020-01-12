import { standard, email } from './fieldcheck';

test('Fieldcheck: email', () => {
    expect(email("")).toBe("Dit veld moet worden ingevuld!");
    expect(email(undefined)).toBe("Dit veld moet worden ingevuld!");
    expect(email("@gmail.com")).toBe("E-mailadres is incorrect!");
    expect(email("mike@gmail.com")).toBe(false);
});

test('Fieldcheck: standard', () => {
    expect(standard("")).toBe("Dit veld moet worden ingevuld!");
    expect(standard(undefined)).toBe("Dit veld moet worden ingevuld!");
    expect(standard("random string")).toBe(false);
});