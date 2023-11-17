import { Title } from "@/components/text/Title.tsx";
import { User } from "@/features/users/user.ts";
import { useEffect, useState } from "react";
import { useGetJson } from "@/api/useGetJson.ts";
import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table.tsx";

type GetUsersResponse = {
  users: User[];
};

export const UserList = () => {
  const [users, setUsers] = useState<User[]>([]);

  const getUsers = useGetJson<undefined, GetUsersResponse>("api/user/GetUsers");

  useEffect(() => {
    getUsers.makeRequest({
      onSuccess: (res) => {
        setUsers(res.users);
      },
    });
  }, [getUsers]);

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
