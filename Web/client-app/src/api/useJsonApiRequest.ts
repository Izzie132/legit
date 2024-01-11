import {
  HttpMethod,
  makeApiRequest,
  MakeApiRequestParameters,
  QueryParameters,
} from "@/api/makeApiRequest";
import { useApiRequest } from "@/api/useApiRequest";
import { useCallback } from "react";

type UseJsonApiRequestParameters = {
  method: HttpMethod;
  endpointUrl: string;
};

type MakeJsonRequestArguments<TRequestBody, TResponse> = {
  requestBody?: TRequestBody;
  queryParameters?: QueryParameters;
  onSuccess?: (response: TResponse) => void;
  onFailure?: (error: string) => void;
};

export const useJsonApiRequest = <TRequestBody, TResponse>({
  method,
  endpointUrl,
}: UseJsonApiRequestParameters) => {
  const wrappedMethod = useCallback(
    (parameters: MakeApiRequestParameters<TRequestBody>) =>
      makeApiRequest<TRequestBody, TResponse>({ ...parameters, method }),
    [method],
  );

  return useApiRequest<
    MakeJsonRequestArguments<TRequestBody, TResponse>,
    MakeApiRequestParameters<TRequestBody>,
    TResponse
  >(endpointUrl, wrappedMethod);
};
