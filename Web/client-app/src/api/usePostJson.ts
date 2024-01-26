import { useCallback } from "react";
import type { ApiRequest } from "@/api/useApiRequest.ts";
import { useJsonApiRequest } from "@/api/useJsonApiRequest.ts";

type MakeRequestParameters<
  TRequestBody extends object | undefined,
  TResponse extends object,
> = {
  requestBody?: TRequestBody;
  onSuccess?: (response: TResponse) => void;
  onFailure?: (error: string) => void;
};

export const usePostJson = <
  TRequestBody extends object | undefined,
  TResponse extends object,
>(
  endpointUrl: string,
): ApiRequest<MakeRequestParameters<TRequestBody, TResponse>, TResponse> => {
  const { makeRequest, cancelRequest, state } = useJsonApiRequest<
    TRequestBody,
    TResponse
  >({
    method: "POST",
    endpointUrl,
  });

  const wrappedMakeRequest = useCallback(
    (
      makeRequestParameters?: MakeRequestParameters<TRequestBody, TResponse>,
    ) => {
      const requestBody = makeRequestParameters?.requestBody;
      const onSuccess = makeRequestParameters?.onSuccess;
      const onFailure = makeRequestParameters?.onFailure;

      return makeRequest({
        requestBody,
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
