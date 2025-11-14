import { NavLink } from "react-router-dom";
import { cn } from "@/lib/utils";

type NavBarLinkProps = {
  to: string;
  label: string;
  className?: string;
};

export const NavBarLink = ({ to, label, className }: NavBarLinkProps) => (
  <NavLink
    to={to}
    className={({ isActive }) =>
      cn("text-l", { "font-semibold": isActive }, className)
    }
  >
    {label}
  </NavLink>
);
