import { render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Counter } from "@/features/counter/Counter.tsx";

describe("Counter", () => {
  it("should render", () => {
    render(<Counter />);

    expect(screen.getByTestId("count-input")).toHaveProperty("value", "0");
    expect(screen.getByText("Counter")).toBeDefined();
  });

  it("should increment", async () => {
    render(<Counter />);

    const increment = screen.getByText("Increment");
    increment.click();

    await waitFor(() => {
      expect(screen.getByTestId("count-input")).toHaveProperty("value", "1");
    });
  });

  it("should decrement", async () => {
    render(<Counter />);

    const decrement = screen.getByText("Decrement");
    decrement.click();

    await waitFor(() => {
      expect(screen.getByTestId("count-input")).toHaveProperty("value", "-1");
    });
  });
});
