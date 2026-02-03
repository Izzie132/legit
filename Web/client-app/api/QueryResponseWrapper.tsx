import { UseQueryResult } from "@tanstack/react-query";
import { StyleSheet, Text, View } from "react-native";
import { parseApiException } from "@/api/apiErrorReponse";
import { ReactNode } from "react";
import theme from "@/constants/theme";

type QueryResultWrapperProps<T> = {
  query: UseQueryResult<T>;
  children: (data: T) => ReactNode;
  errorContent?: ReactNode;
};

export const QueryResultWrapper = <QueryType,>({
  query,
  children,
  errorContent,
}: QueryResultWrapperProps<QueryType>) => {
  const { isPending, isError, error, data } = query;
  if (isPending) {
    return (
      <View style={styles.container}>
        <Text style={styles.text}>Loading...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      errorContent ?? (
        <View style={styles.container}>
          <Text style={styles.text}>An error occurred: </Text>
          <Text style={styles.text}>
            {parseApiException(error).userVisibleMessage}
          </Text>
        </View>
      )
    );
  }

  return children(data);
};

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
