import { ReactNode } from "react";

export const Title = ({ children }: { children: ReactNode }) => (
  <h1 className="mb-6 text-4xl">{children}</h1>
);
