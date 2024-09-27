import { NavLink } from "react-router-dom";

type NavBarLinkProps = {
  to: string;
  label: string;
};

export const NavBarLink = (props: NavBarLinkProps) => (
  <NavLink
    to={props.to}
    className={({ isActive }) => (isActive ? "text-l font-semibold" : "text-l")}
  >
    {props.label}
  </NavLink>
);
