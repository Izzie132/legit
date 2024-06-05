import { useState } from "react";
import { Title } from "@/components/text/Title.tsx";
import { Button } from "@/components/ui/button.tsx";
import { Input } from "@/components/ui/input.tsx";

export const Counter = () => {
  const [count, setCount] = useState<number>(0);

  return (
    <>
      <Title>Counter</Title>
      <div className="flex">
        <Button className="mr-5" onClick={() => setCount(count - 1)}>
          Decrement
        </Button>
        <Input
          id="count"
          value={count}
          type="number"
          onChange={(e) => setCount(+e.target.value)}
          data-testid="count-input"
        />
        <Button className="ml-5" onClick={() => setCount(count + 1)}>
          Increment
        </Button>
      </div>
    </>
  );
};
