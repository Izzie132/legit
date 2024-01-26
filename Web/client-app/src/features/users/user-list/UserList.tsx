import { useEffect, useState } from "react";
import { useGetJson } from "@/api/useGetJson.ts";
import { Title } from "@/components/text/Title.tsx";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table.tsx";
import type { User } from "@/features/users/user.ts";

type GetUsersResponse = {
  users: Array<User>;
};

export const UserList = () => {
  const [users, setUsers] = useState<Array<User>>([]);

  const { makeRequest } = useGetJson<undefined, GetUsersResponse>(
    "api/user/GetUsers",
  );

  useEffect(() => {
    void makeRequest({
      onSuccess: (res) => {
        setUsers(res.users);
      },
    });
  }, [makeRequest]);

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
          {users.map((user) => (
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
};
