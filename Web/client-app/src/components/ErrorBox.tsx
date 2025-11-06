import type { ReactNode } from "react";

export const ErrorBox = ({ children }: { children: ReactNode }) => (
  <div className="w-full rounded-sm border-2 border-red-800 bg-red-200 p-3 text-left text-red-800">
    {children}
  </div>
);
