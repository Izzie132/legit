import { CancelTokenSource } from "axios";
import {
  ApiResponse,
  cancelledRequestErrorMessage,
} from "@/api/makeApiRequest";
import { useEffect, useRef, useState } from "react";

type CoreMakeRequestArguments<TResponse> = {
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
export const useApiRequest = <
  TMakeRequestArguments extends CoreMakeRequestArguments<TResponse>,
  TApiRequestParameters extends CoreApiRequestParameters,
  TResponse,
>(
  endpointUrl: string,
  makeApiRequest: (
    parameters: TApiRequestParameters,
  ) => Promise<ApiResponse<TResponse>>,
) => {
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

  const makeRequest = ({
    onSuccess,
    onFailure,
    ...makeRequestArguments
  }: TMakeRequestArguments) => {
    cancelRequest();

    setState((previousState) => ({
      ...previousState,
      isLoading: true,
    }));

    return makeApiRequest({
      ...makeRequestArguments,
      endpointUrl,
      cancelTokenSource: abortController.current.signal,
    } as unknown as TApiRequestParameters).then((apiResponse) => {
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
    });
  };

  // Cancel the request when the component unmounts.
  useEffect(() => {
    return () => cancelRequest();
  }, []);

  return {
    makeRequest,
    cancelRequest,
    state: state,
  };
};
