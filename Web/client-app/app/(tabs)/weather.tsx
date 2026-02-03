import { StyleSheet, Text, View } from "react-native";
import { useApiClient } from "@/api/useApiClient";
import { useQuery } from "@tanstack/react-query";
import { QueryResultWrapper } from "@/api/QueryResponseWrapper";
import theme from "@/constants/theme";

export default function WeatherScreen() {
  const apiClient = useApiClient();
  const getWeatherQuery = useQuery({
    queryKey: ["weather"],
    queryFn: async () => await apiClient.getWeather(),
  });

  return (
    <QueryResultWrapper query={getWeatherQuery}>
      {(weatherData) => (
        <View style={styles.container}>
          <Text style={styles.text}>
            Today&#39;s temperature is: {weatherData.temperature}°C
          </Text>
        </View>
      )}
    </QueryResultWrapper>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.Colors.background,
    justifyContent: "center",
    alignItems: "center",
  },
  text: {
    color: theme.Colors.textPrimary,
  },
});
