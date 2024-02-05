import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { parseApiException } from "@/api/apiErrorReponse.ts";
import { useApiClient } from "@/api/useApiClient.tsx";
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

  if (getWeather.isLoading) {
    return <Loading />;
  }

  if (getWeather.isError) {
    return <div>{parseApiException(getWeather.error).userVisibleMessage}</div>;
  }

  return (
    <div className="flex flex-col items-center">
      <Title>Weather</Title>
      {getWeather.isSuccess && (
        <>
          <h2 className="mb-2 text-xl">{getWeather.data.description}</h2>
          <div className="mb-2 flex items-center">
            <button
              className="mr-2 flex h-[20px] w-[20px] justify-center"
              onClick={cycleTemperatureUnits}
            >
              <TemperatureIcon />
            </button>
            <p>
              {temperatureConverter(
                getWeather.data.temperature,
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
                getWeather.data.windSpeed,
                "mph",
                windSpeedUnits,
              )}{" "}
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
