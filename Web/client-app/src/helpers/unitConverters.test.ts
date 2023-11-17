import {
  temperatureConverter,
  TemperatureUnits,
  windSpeedConverter,
  WindSpeedUnits,
} from "./unitConverters";
import { describe, expect, test } from "vitest";

type TestTemperatureConverterInputs = {
  input: number;
  fromUnit: TemperatureUnits;
  toUnit: TemperatureUnits;
  expectedOutput: number;
};

describe("temperatureConverter", () => {
  test.each<TestTemperatureConverterInputs>([
    {
      input: 20,
      fromUnit: "celsius",
      toUnit: "kelvin",
      expectedOutput: 293.1,
    },
    {
      input: 90,
      fromUnit: "fahrenheit",
      toUnit: "kelvin",
      expectedOutput: 305.4,
    },
    {
      input: 300,
      fromUnit: "kelvin",
      toUnit: "celsius",
      expectedOutput: 26.9,
    },
    {
      input: 300,
      fromUnit: "kelvin",
      toUnit: "fahrenheit",
      expectedOutput: 80.3,
    },
    {
      input: 300,
      fromUnit: "kelvin",
      toUnit: "kelvin",
      expectedOutput: 300,
    },
  ])(
    "should convert from $fromUnit to $toUnit",
    ({ input, fromUnit, toUnit, expectedOutput }) => {
      const output = temperatureConverter(input, fromUnit, toUnit);
      expect(output).toBe(expectedOutput);
    },
  );
});

type TestWindSpeedConverterInputs = {
  input: number;
  fromUnit: WindSpeedUnits;
  toUnit: WindSpeedUnits;
  expectedOutput: number;
};

describe("windSpeedConverter", () => {
  test.each<TestWindSpeedConverterInputs>([
    {
      input: 20,
      fromUnit: "mph",
      toUnit: "mps",
      expectedOutput: 8.9,
    },
    {
      input: 20,
      fromUnit: "kph",
      toUnit: "mps",
      expectedOutput: 5.6,
    },
    {
      input: 20,
      fromUnit: "kts",
      toUnit: "mps",
      expectedOutput: 10.3,
    },
    {
      input: 20,
      fromUnit: "mps",
      toUnit: "mph",
      expectedOutput: 44.7,
    },
    {
      input: 20,
      fromUnit: "mps",
      toUnit: "kph",
      expectedOutput: 72,
    },
    {
      input: 20,
      fromUnit: "mps",
      toUnit: "kts",
      expectedOutput: 38.9,
    },
    {
      input: 20,
      fromUnit: "mps",
      toUnit: "mps",
      expectedOutput: 20,
    },
  ])(
    "should convert from $fromUnit to $toUnit",
    ({ input, fromUnit, toUnit, expectedOutput }) => {
      const output = windSpeedConverter(input, fromUnit, toUnit);
      expect(output).toBe(expectedOutput);
    },
  );
});
