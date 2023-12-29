export type ApiErrorResponse = {
  message: string | null;
  stackTrace: number | null;
  userVisibleMessage: string | null;
};

const emptyApiErrorResponse: ApiErrorResponse = {
  message: null,
  stackTrace: null,
  userVisibleMessage: null,
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
