import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { HandleQueryResult } from "@/api/HandleQueryResult";
import { useApiClient } from "@/api/useApiClient";
import TemperatureIcon from "@/assets/icons/temperature-half-solid.svg?react";
import WindIcon from "@/assets/icons/wind-solid.svg?react";
import { Title } from "@/components/text/Title";
import {
  type TemperatureUnits,
  type WindSpeedUnits,
  temperatureConverter,
  temperatureSuffixByUnit,
  windSpeedConverter,
  windSpeedSuffixByUnit,
} from "@/helpers/unitConverters";

export const Weather = () => {
  const [temperatureUnits, setTemperatureUnits] =
    useState<TemperatureUnits>("celsius");
  const [windSpeedUnits, setWindSpeedUnits] = useState<WindSpeedUnits>("mph");

  const apiClient = useApiClient();

  const getWeather = useQuery({
    queryKey: ["getWeather"],
    queryFn: ({ signal }) => apiClient.getWeather(signal),
  });

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

  return (
    <div className="flex flex-col items-center">
      <Title>Weather</Title>
      <HandleQueryResult query={getWeather}>
        {(getWeatherResponse) => (
          <>
            <h2 className="mb-2 text-xl">{getWeatherResponse.description}</h2>
            <div className="mb-2 flex items-center">
              <button
                className="mr-2 flex h-[20px] w-[20px] justify-center"
                onClick={cycleTemperatureUnits}
              >
                <TemperatureIcon />
              </button>
              <p>
                {temperatureConverter(
                  getWeatherResponse.temperature,
                  "celsius",
                  temperatureUnits,
                )}{" "}
                {temperatureSuffixByUnit[temperatureUnits]}
              </p>
            </div>
            <div className="mb-2 flex items-center">
              <button
                className="mr-2 flex h-[20px] w-[20px] justify-center"
                onClick={cycleWindSpeedUnits}
              >
                <WindIcon />
              </button>
              <p>
                {windSpeedConverter(
                  getWeatherResponse.windSpeed,
                  "mph",
                  windSpeedUnits,
                )}{" "}
                {windSpeedSuffixByUnit[windSpeedUnits]}
              </p>
            </div>
          </>
        )}
      </HandleQueryResult>
      <p className="text-sm text-gray-600">
        Hint: Click on the icons to change the units!
      </p>
    </div>
  );
};
