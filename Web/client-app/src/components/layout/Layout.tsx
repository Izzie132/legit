import { Outlet } from "react-router-dom";
import { NavBarLink } from "@/components/layout/NavBarLink.tsx";
import { Toaster } from "@/components/ui/toaster.tsx";

export const Layout = () => (
  <div className="flex flex-col items-center">
    <div className="mb-10 flex h-16 w-full items-center px-16 shadow">
      <h1 className="mr-20 text-2xl font-semibold">Project Name</h1>
      <NavBarLink to="/" label="Home" />
      <NavBarLink to="/weather" label="Weather" />
      <NavBarLink to="/counter" label="Counter" />
      <NavBarLink to="/user-list" label="Users" />
      <NavBarLink to="/sign-up" label="Sign Up" />
    </div>

    <div className="flex w-full flex-col items-center lg:w-1/2">
      <Outlet />
    </div>

    <Toaster />
  </div>
);
