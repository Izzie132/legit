import axios, {
  type AxiosRequestHeaders,
  type AxiosResponse,
  type CancelTokenSource,
  type Method,
  type RawAxiosRequestHeaders,
  type ResponseType,
} from "axios";
import queryString from "query-string";
import { isApiErrorResponse } from "@/api/ApiErrorResponse.ts";

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

export const makeApiRequest = <TRequestBody, TResponse>({
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
    method,
    url,
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
    // eslint-disable-next-line no-console -- We want to know about this if it happens
    console.warn(cancelledRequestErrorMessage);
    return buildErrorResponse({
      error: cancelledRequestErrorMessage,
      statusCode: null,
    });
  }

  // eslint-disable-next-line no-console -- If we've got a failed API response, this will help with debugging
  console.error(error);

  if (isHttpStatusCodeError(error)) {
    const errorMessage =
      (isApiErrorResponse(error.response.data)
        ? error.response.data.userVisibleMessage ?? error.response.data.message
        : null) ?? "An unexpected error has occurred";

    return buildErrorResponse({
      error: errorMessage,
      statusCode: error.response.status,
    });
  }

  if (isNoResponseReceivedError(error)) {
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
  typeof error === "object" &&
  error != null &&
  "response" in error &&
  error.response != null;

const isNoResponseReceivedError = (
  error: unknown,
): error is { request: XMLHttpRequest } =>
  typeof error === "object" &&
  error != null &&
  "request" in error &&
  error.request != null;

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
