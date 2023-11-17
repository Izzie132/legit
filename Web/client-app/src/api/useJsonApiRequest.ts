import {
  HttpMethod,
  makeApiRequest,
  MakeApiRequestParameters,
  QueryParameters,
} from "@/api/makeApiRequest";
import { useApiRequest } from "@/api/useApiRequest";

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
}: UseJsonApiRequestParameters) =>
  useApiRequest<
    MakeJsonRequestArguments<TRequestBody, TResponse>,
    MakeApiRequestParameters<TRequestBody>,
    TResponse
  >(endpointUrl, (parameters) => makeApiRequest({ ...parameters, method }));
