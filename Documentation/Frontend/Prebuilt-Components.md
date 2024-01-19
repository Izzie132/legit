# Prebuilt Components

## shadcn/ui

[shadcn/ui](https://ui.shadcn.com/docs) provides a set of prebuilt components built using [Radix UI](https://www.radix-ui.com/) and [Tailwind](https://tailwindcss.com/).
Unlike component libraries, which generally provide a set of black-box components that can be hard to customise, shadcn/ui provides only the code for components, so they can be easily extended.

## Add a component with shadcn

To add a new component from shadcn:

1. Find the component you want to use on the [shadcn/ui docs](https://ui.shadcn.com/docs)
2. Open a terminal in the [`Web/ClientApp`](../../Web/client-app)
3. Run `npx shadcn-ui@latest add <component-name>`
4. Follow any other instructions under the `Installation` section of the component's docs

This should create the component inside the [`components/ui`](../../Web/client-app/src/components/ui) folder.
