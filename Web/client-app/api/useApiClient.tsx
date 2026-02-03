import type { ReactNode } from "react";
import { createContext, useContext } from "react";
import { ApiClient } from "@/api/ApiClient";
import Constants from "expo-constants";

export const ApiClientContext = createContext<ApiClient | undefined>(undefined);

export const ApiClientContextProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const apiBaseUrl =
    Constants.expoConfig?.extra?.apiBaseUrl ?? "https://localhost:5000";
  const apiClient = new ApiClient(apiBaseUrl);

  return (
    <ApiClientContext.Provider value={apiClient}>
      {children}
    </ApiClientContext.Provider>
  );
};

export const useApiClient = () => {
  const context = useContext(ApiClientContext);

  if (!context) {
    throw new Error("Context does not exist");
  }

  return context;
};
