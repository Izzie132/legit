import { useCallback } from "react";
import type { QueryParameters } from "@/api/makeApiRequest.ts";
import type { ApiRequest } from "@/api/useApiRequest.ts";
import { useJsonApiRequest } from "@/api/useJsonApiRequest.ts";

type MakeRequestParameters<
  TQueryParameters extends QueryParameters | undefined,
  TResponse extends object,
> = {
  queryParameters?: TQueryParameters | undefined;
  onSuccess?: (response: TResponse) => void;
  onFailure?: (error: string) => void;
};

export const useGetJson = <
  TQueryParameters extends QueryParameters | undefined,
  TResponse extends object,
>(
  endpointUrl: string,
): ApiRequest<
  MakeRequestParameters<TQueryParameters, TResponse>,
  TResponse
> => {
  const { makeRequest, cancelRequest, state } = useJsonApiRequest<
    undefined,
    TResponse
  >({
    method: "GET",
    endpointUrl,
  });

  const wrappedMakeRequest = useCallback(
    (
      makeRequestParameters?: MakeRequestParameters<
        TQueryParameters,
        TResponse
      >,
    ) => {
      const onSuccess = makeRequestParameters?.onSuccess;
      const onFailure = makeRequestParameters?.onFailure;

      return makeRequest({
        ...makeRequestParameters,
        onSuccess,
        onFailure,
      });
    },
    [makeRequest],
  );

  return {
    cancelRequest,
    state,
    makeRequest: wrappedMakeRequest,
  };
};
