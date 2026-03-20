import { UseQueryResult } from "@tanstack/react-query";
import { Text, View } from "react-native";
import { parseApiException } from "@/api/apiErrorReponse";
import { ReactNode } from "react";

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
      <View className="flex-1 items-center justify-center bg-brand-background">
        <Text className="text-white">Loading...</Text>
      </View>
    );
  }

  if (isError) {
    return (
      errorContent ?? (
        <View className="flex-1 items-center justify-center bg-brand-background">
          <Text className="text-white">An error occurred: </Text>
          <Text className="text-white">
            {parseApiException(error).userVisibleMessage}
          </Text>
        </View>
      )
    );
  }

  return children(data);
};
