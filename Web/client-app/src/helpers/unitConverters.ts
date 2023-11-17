export type TemperatureUnits = "kelvin" | "celsius" | "fahrenheit";

export const temperatureSuffixByUnit: { [key in TemperatureUnits]: string } = {
  kelvin: "K",
  celsius: "℃",
  fahrenheit: "℉",
};

export const temperatureConverter = (
  input: number,
  fromUnit: TemperatureUnits,
  toUnit: TemperatureUnits,
): number => {
  const toKelvin: { [key in TemperatureUnits]: (temp: number) => number } = {
    kelvin: (value) => value,
    celsius: (value) => value + 273.15,
    fahrenheit: (value) => ((value + 459.67) * 5) / 9,
  };

  const inputKelvin = toKelvin[fromUnit](input);

  const fromKelvin: { [key in TemperatureUnits]: (value: number) => number } = {
    kelvin: (value) => value,
    celsius: (value) => value - 273.15,
    fahrenheit: (value) => (value * 9) / 5 - 459.67,
  };

  return parseFloat(fromKelvin[toUnit](inputKelvin).toFixed(1));
};

export type WindSpeedUnits = "mps" | "kts" | "mph" | "kph";

export const windSpeedSuffixByUnit: { [key in WindSpeedUnits]: string } = {
  mps: "m/s",
  kts: "kts",
  mph: "mph",
  kph: "km/h",
};

export const windSpeedConverter = (
  input: number,
  fromUnit: WindSpeedUnits,
  toUnit: WindSpeedUnits,
): number => {
  const toMps: { [key in WindSpeedUnits]: (value: number) => number } = {
    mps: (value) => value,
    kts: (value) => value * 0.5144444,
    mph: (value) => value * 0.44704,
    kph: (value) => value * 0.2777778,
  };

  const inputMps = toMps[fromUnit](input);

  const fromMps: { [key in WindSpeedUnits]: (value: number) => number } = {
    mps: (value) => value,
    kts: (value) => value * 1.9438445,
    mph: (value) => value * 2.236937,
    kph: (value) => value * 3.6,
  };

  return parseFloat(fromMps[toUnit](inputMps).toFixed(1));
};
