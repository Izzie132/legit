import { useJsonApiRequest } from "@/api/useJsonApiRequest";
import { useCallback } from "react";

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
) => {
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
