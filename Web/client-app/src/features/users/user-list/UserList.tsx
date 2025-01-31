import { useQuery } from "@tanstack/react-query";
import { parseApiException } from "@/api/apiErrorReponse";
import { useApiClient } from "@/api/useApiClient";
import { Loading } from "@/components/Loading";
import { Title } from "@/components/text/Title";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

export const UserList = () => {
  const apiClient = useApiClient();

  const getUsersQuery = useQuery({
    queryKey: ["getUsers"],
    queryFn: ({ signal }) => apiClient.getUsers(signal),
  });

  if (getUsersQuery.isLoading) {
    return <Loading />;
  }
  if (getUsersQuery.isError) {
    return (
      <div>{parseApiException(getUsersQuery.error).userVisibleMessage}</div>
    );
  }
  if (getUsersQuery.isSuccess) {
    return (
      <>
        <Title>User List</Title>

        <Table>
          <TableCaption>A list of users.</TableCaption>
          <TableHeader>
            <TableRow>
              <TableHead>Id</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Email</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {getUsersQuery.data.users.map((user) => (
              <TableRow key={user.id}>
                <TableCell>{user.id}</TableCell>
                <TableCell>{user.name}</TableCell>
                <TableCell>{user.email}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </>
    );
  }
};
