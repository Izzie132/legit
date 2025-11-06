import { useQuery } from "@tanstack/react-query";
import { HandleQueryResult } from "@/api/HandleQueryResult";
import { useApiClient } from "@/api/useApiClient";
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

  return (
    <HandleQueryResult query={getUsersQuery}>
      {(getUsersResponse) => (
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
              {getUsersResponse.users.map((user) => (
                <TableRow key={user.id}>
                  <TableCell>{user.id}</TableCell>
                  <TableCell>{user.name}</TableCell>
                  <TableCell>{user.email}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </>
      )}
    </HandleQueryResult>
  );
};
