import { useEffect, useState } from "react";
import { useGetJson } from "@/api/useGetJson.ts";
import TemperatureIcon from "@/assets/icons/temperature-half-solid.svg?react";
import WindIcon from "@/assets/icons/wind-solid.svg?react";
import { Loading } from "@/components/Loading.tsx";
import { Title } from "@/components/text/Title.tsx";
import {
  type TemperatureUnits,
  type WindSpeedUnits,
  temperatureConverter,
  temperatureSuffixByUnit,
  windSpeedConverter,
  windSpeedSuffixByUnit,
} from "@/helpers/unitConverters.ts";

type WeatherInfo = {
  temperature: number;
  windSpeed: number;
  description: string;
};

export const Weather = () => {
  const [weatherInfo, setWeatherInfo] = useState<WeatherInfo | null>(null);
  const [temperatureUnits, setTemperatureUnits] =
    useState<TemperatureUnits>("celsius");
  const [windSpeedUnits, setWindSpeedUnits] = useState<WindSpeedUnits>("mph");

  const { makeRequest, state } = useGetJson<undefined, WeatherInfo>(
    "api/weather/GetWeather",
  );

  useEffect(() => {
    void makeRequest({
      onSuccess: (response) => {
        setWeatherInfo(response);
      },
    });
  }, [makeRequest]);

  const cycleTemperatureUnits = () => {
    switch (temperatureUnits) {
      case "celsius":
        setTemperatureUnits("fahrenheit");
        break;
      case "fahrenheit":
        setTemperatureUnits("kelvin");
        break;
      case "kelvin":
        setTemperatureUnits("celsius");
    }
  };

  const cycleWindSpeedUnits = () => {
    switch (windSpeedUnits) {
      case "mph":
        setWindSpeedUnits("kph");
        break;
      case "kph":
        setWindSpeedUnits("kts");
        break;
      case "kts":
        setWindSpeedUnits("mps");
        break;
      case "mps":
        setWindSpeedUnits("mph");
    }
  };

  if (state.isLoading) {
    return <Loading />;
  }

  return (
    <div className="flex flex-col items-center">
      <Title>Weather</Title>
      {weatherInfo && (
        <>
          <h2 className="mb-2 text-xl">{weatherInfo.description}</h2>
          <div className="mb-2 flex items-center">
            {/* eslint-disable-next-line jsx-a11y/click-events-have-key-events,jsx-a11y/no-static-element-interactions -- TODO make this accessible */}
            <div
              className="mr-2 flex h-[20px] w-[20px] justify-center"
              onClick={cycleTemperatureUnits}
            >
              <TemperatureIcon />
            </div>
            <p>
              {temperatureConverter(
                weatherInfo.temperature,
                "celsius",
                temperatureUnits,
              )}{" "}
              {temperatureSuffixByUnit[temperatureUnits]}
            </p>
          </div>
          <div className="mb-2 flex items-center">
            {/* eslint-disable-next-line jsx-a11y/click-events-have-key-events,jsx-a11y/no-static-element-interactions -- TODO make this accessible */}
            <div
              className="mr-2 flex h-[20px] w-[20px] justify-center"
              onClick={cycleWindSpeedUnits}
            >
              <WindIcon />
            </div>
            <p>
              {windSpeedConverter(weatherInfo.windSpeed, "mph", windSpeedUnits)}{" "}
              {windSpeedSuffixByUnit[windSpeedUnits]}
            </p>
          </div>
        </>
      )}
      <p className="text-sm text-gray-600">
        Hint: Click on the icons to change the units!
      </p>
    </div>
  );
};
