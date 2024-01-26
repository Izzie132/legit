import type { CancelTokenSource } from "axios";
import { useCallback, useEffect, useRef, useState } from "react";
import {
  type ApiResponse,
  cancelledRequestErrorMessage,
} from "@/api/makeApiRequest.ts";

export type CoreMakeRequestArguments<TResponse> = {
  onSuccess?: (response: TResponse) => void;
  onFailure?: (error: string) => void;
};

type CoreApiRequestParameters = {
  endpointUrl: string;
  cancelTokenSource?: CancelTokenSource;
};

type State = {
  isLoading: boolean;
  error: string | null;
};

export type ApiRequest<
  TMakeRequestArguments extends CoreMakeRequestArguments<TResponse>,
  TResponse,
> = {
  makeRequest: (
    args: TMakeRequestArguments,
  ) => Promise<ApiResponse<TResponse> | undefined>;
  cancelRequest: () => void;
  state: State;
};

export const useApiRequest = <
  TMakeRequestArguments extends CoreMakeRequestArguments<TResponse>,
  TApiRequestParameters extends CoreApiRequestParameters,
  TResponse,
>(
  endpointUrl: string,
  makeApiRequest: (
    parameters: TApiRequestParameters,
  ) => Promise<ApiResponse<TResponse>>,
): ApiRequest<TMakeRequestArguments, TResponse> => {
  const [state, setState] = useState<State>({
    isLoading: false,
    error: null,
  });

  const abortController = useRef(new AbortController());
  const cancelRequest = () => {
    abortController.current.abort();
    setState((previousState) => ({
      ...previousState,
      isLoading: false,
    }));
  };

  const makeRequest = useCallback(
    async ({
      onSuccess,
      onFailure,
      ...makeRequestArguments
    }: TMakeRequestArguments) => {
      cancelRequest();

      setState((previousState) => ({
        ...previousState,
        isLoading: true,
      }));

      const apiResponse = await makeApiRequest({
        ...makeRequestArguments,
        endpointUrl,
        cancelTokenSource: abortController.current.signal,
      } as unknown as TApiRequestParameters);

      if (apiResponse.success) {
        setState((previousState) => ({
          ...previousState,
          isLoading: false,
        }));

        onSuccess?.(apiResponse.response);
      } else {
        if (apiResponse.error === cancelledRequestErrorMessage) {
          // If the request has been cancelled then we ignore the error as it is expected.
          return;
        }

        setState((previousState) => ({
          ...previousState,
          isLoading: false,
          error: apiResponse.error,
        }));

        onFailure?.(apiResponse.error);
      }
      return apiResponse;
    },
    [endpointUrl, makeApiRequest],
  );

  // Cancel the request when the component unmounts.
  useEffect(() => () => cancelRequest(), []);

  return {
    makeRequest,
    cancelRequest,
    state,
  };
};
