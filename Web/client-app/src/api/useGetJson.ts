import { useJsonApiRequest } from "@/api/useJsonApiRequest";
import { QueryParameters } from "@/api/makeApiRequest.ts";

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
  const apiRequest = useJsonApiRequest<undefined, TResponse>({
    method: "GET",
    endpointUrl,
  });

  const makeRequest = (
    makeRequestParameters?: MakeRequestParameters<TQueryParameters, TResponse>,
  ) => {
    const onSuccess = makeRequestParameters?.onSuccess;
    const onFailure = makeRequestParameters?.onFailure;

    return apiRequest.makeRequest({
      onSuccess,
      onFailure,
    });
  };

  return {
    ...apiRequest,
    makeRequest,
  };
};
