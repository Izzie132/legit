import type { ReactNode } from "react";
import { createContext, useContext } from "react";
import { ApiClient } from "@/api/ApiClient.ts";

export const ApiClientContext = createContext<ApiClient | undefined>(undefined);

export const ApiClientContextProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const apiClient = new ApiClient(window.location.origin);

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
