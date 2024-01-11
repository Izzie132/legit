import { useJsonApiRequest } from "@/api/useJsonApiRequest";
import { QueryParameters } from "@/api/makeApiRequest.ts";
import { useCallback } from "react";

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
) => {
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
