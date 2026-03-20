import { Text, View } from "react-native";
import { useApiClient } from "@/api/useApiClient";
import { useQuery } from "@tanstack/react-query";
import { QueryResultWrapper } from "@/api/QueryResponseWrapper";

export default function WeatherScreen() {
  const apiClient = useApiClient();
  const getWeatherQuery = useQuery({
    queryKey: ["weather"],
    queryFn: async () => await apiClient.getWeather(),
  });

  return (
    <QueryResultWrapper query={getWeatherQuery}>
      {(weatherData) => (
        <View className="flex-1 items-center justify-center bg-brand-background">
          <Text className="text-white">
            Today&#39;s temperature is: {weatherData.temperature}°C
          </Text>
        </View>
      )}
    </QueryResultWrapper>
  );
}
