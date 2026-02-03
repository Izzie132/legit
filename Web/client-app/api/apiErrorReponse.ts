import type { ApiErrorResponse } from "@/api/ApiClient";
import { ApiException } from "@/api/ApiClient";

const emptyApiErrorResponse: ApiErrorResponse = {
  message: undefined,
  stackTrace: undefined,
  userVisibleMessage: undefined,
};

export const isApiErrorResponse = (
  value: unknown,
): value is ApiErrorResponse => {
  if (typeof value !== "object") {
    return false;
  }

  for (const propertyName of Object.getOwnPropertyNames(
    emptyApiErrorResponse,
  )) {
    if (!Object.prototype.hasOwnProperty.call(value, propertyName)) {
      return false;
    }
  }

  return true;
};

export const parseApiException = (error: Error): ApiErrorResponse => {
  if (ApiException.isApiException(error) && isApiErrorResponse(error.result)) {
    return error.result;
  } else {
    return {
      userVisibleMessage: error.message,
    };
  }
};
