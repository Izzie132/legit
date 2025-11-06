import type { UseQueryResult } from "@tanstack/react-query";
import type { ReactNode } from "react";
import { parseApiException } from "@/api/apiErrorReponse";
import { ErrorBox } from "@/components/ErrorBox";
import { Loading } from "@/components/Loading";

type HandleQueryResultProps<TResponse> = {
  query: UseQueryResult<TResponse>;
  children: (data: TResponse) => ReactNode;
};

export const HandleQueryResult = <TResponse,>({
  query,
  children,
}: HandleQueryResultProps<TResponse>) => {
  if (query.isLoading) {
    return <Loading />;
  }

  if (query.isError || !query.isSuccess) {
    return (
      <ErrorBox>
        <div>An error occurred while fetching the data.</div>
        {query.isError && (
          <div>{parseApiException(query.error).userVisibleMessage}</div>
        )}
      </ErrorBox>
    );
  }

  return children(query.data);
};
