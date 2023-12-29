import axios, {
  AxiosRequestHeaders,
  AxiosResponse,
  CancelTokenSource,
  Method,
  RawAxiosRequestHeaders,
  ResponseType,
} from "axios";
import { isApiErrorResponse } from "@/api/ApiErrorResponse";
import queryString from "query-string";

export type HttpMethod = Method;

export type QueryParameters = Record<string, unknown>;

const defaultRequestTimeoutInSeconds = 10;

export type MakeApiRequestParameters<TRequestBody> = {
  method: HttpMethod;
  endpointUrl: string;
  cancelTokenSource?: CancelTokenSource;
  requestBody?: TRequestBody;
  queryParameters?: QueryParameters;
  timeoutInSeconds?: number;
  responseType?: ResponseType;
  headers?: AxiosRequestHeaders;
};

export const makeApiRequest = async <TRequestBody, TResponse>({
  method,
  endpointUrl,
  cancelTokenSource,
  requestBody,
  queryParameters,
  timeoutInSeconds,
  responseType,
  headers,
}: MakeApiRequestParameters<TRequestBody>): Promise<ApiResponse<TResponse>> => {
  const defaultHeaders: RawAxiosRequestHeaders = {
    "X-Requested-With": "XMLHttpRequest",
  };

  const url =
    queryParameters == null
      ? endpointUrl
      : `${endpointUrl}?${queryString.stringify(queryParameters)}`;

  const request = axios({
    method: method,
    url: url,
    cancelToken: cancelTokenSource?.token,
    data: requestBody,
    responseType: responseType ?? "json",
    timeout: (timeoutInSeconds ?? defaultRequestTimeoutInSeconds) * 1_000,
    headers: { ...defaultHeaders, ...(headers ?? {}) },
  });

  return request
    .then(handleSuccessfulApiResponse)
    .catch(handleFailedApiResponse);
};

export type ApiResponse<TResponse> =
  | SuccessfulApiResponse<TResponse>
  | FailedApiResponse;

type SuccessfulApiResponse<TResponse> = {
  success: true;
  error: null;
  statusCode: number;
  response: TResponse;
};

type FailedApiResponse = {
  success: false;
  error: string;
  statusCode: number | null;
  response: null;
};

const handleSuccessfulApiResponse = <TResponse>(
  response: AxiosResponse<TResponse>,
): SuccessfulApiResponse<TResponse> => ({
  success: true,
  error: null,
  statusCode: response.status,
  response: response.data,
});

const handleFailedApiResponse = (error: unknown): FailedApiResponse => {
  if (isCancelledError(error)) {
    console.warn(cancelledRequestErrorMessage);
    return buildErrorResponse({
      error: cancelledRequestErrorMessage,
      statusCode: null,
    });
  }

  console.error(error);

  if (isHttpStatusCodeError(error) && error.response != null) {
    const responseData = error.response.data;

    const errorMessage =
      (isApiErrorResponse(responseData)
        ? responseData.userVisibleMessage || responseData.message
        : null) || "An unexpected error has occurred";

    return buildErrorResponse({
      error: errorMessage,
      statusCode: error.response.status,
    });
  }

  if (isNoResponseReceivedError(error) && error.request != null) {
    return buildErrorResponse({
      error: "The request was sent but no response was received",
      statusCode: null,
    });
  }

  return buildErrorResponse({
    error: "Something went wrong while making the request",
    statusCode: null,
  });
};

export const cancelledRequestErrorMessage = "The request was cancelled";

const isCancelledError = (error: unknown): boolean => axios.isCancel(error);

const isHttpStatusCodeError = (
  error: unknown,
): error is { response: AxiosResponse } =>
  typeof error === "object" && error != null && "response" in error;

const isNoResponseReceivedError = (
  error: unknown,
): error is { request: XMLHttpRequest } =>
  typeof error === "object" && error != null && "request" in error;

const buildErrorResponse = ({
  error,
  statusCode,
}: {
  error: string;
  statusCode: number | null;
}): FailedApiResponse => ({
  success: false,
  error,
  statusCode,
  response: null,
});
