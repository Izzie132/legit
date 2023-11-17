import { NavLink } from "react-router-dom";

type NavBarLinkProps = {
  to: string;
  label: string;
};

export const NavBarLink = (props: NavBarLinkProps) => {
  return (
    <>
      <div className="mr-8">
        <NavLink
          to={props.to}
          className={({ isActive }) =>
            isActive ? "text-l font-semibold" : "text-l"
          }
        >
          {props.label}
        </NavLink>
      </div>
    </>
  );
};
