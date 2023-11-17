import { useJsonApiRequest } from "@/api/useJsonApiRequest";

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
  const apiRequest = useJsonApiRequest<TRequestBody, TResponse>({
    method: "POST",
    endpointUrl,
  });

  const makeRequest = (
    makeRequestParameters?: MakeRequestParameters<TRequestBody, TResponse>,
  ) => {
    const requestBody = makeRequestParameters?.requestBody;
    const onSuccess = makeRequestParameters?.onSuccess;
    const onFailure = makeRequestParameters?.onFailure;

    return apiRequest.makeRequest({
      requestBody,
      onSuccess,
      onFailure,
    });
  };

  return {
    ...apiRequest,
    makeRequest,
  };
};
