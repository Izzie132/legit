import { useCallback } from "react";
import {
  type HttpMethod,
  type MakeApiRequestParameters,
  type QueryParameters,
  makeApiRequest,
} from "@/api/makeApiRequest.ts";
import type { ApiRequest } from "@/api/useApiRequest.ts";
import { useApiRequest } from "@/api/useApiRequest.ts";

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
}: UseJsonApiRequestParameters): ApiRequest<
  MakeJsonRequestArguments<TRequestBody, TResponse>,
  TResponse
> => {
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
