import Spinner from "@/assets/spinner.svg?react";

export const Loading = () => {
  return (
    <div className="flex h-[100px] w-[100px]">
      <Spinner />
    </div>
  );
};
