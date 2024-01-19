# React Frontend

The frontend of this application is a Single Page App (SPA) built using React. It is configured to use the following core:

- [React](https://react.dev/) as the frontend framework.
- [Tailwind CSS](https://tailwindcss.com/) for styling.
- [React Router](https://reactrouter.com/) for routing.
- [shadcn/ui](https://ui.shadcn.com/) for common components.
- [Axios](https://axios-http.com/) for making HTTP requests.
- [React Hook Form](https://react-hook-form.com/) and [Zod](https://zod.dev/) for forms and validation.
- [Vite.js](https://vitejs.dev/) for frontend tooling.
- [Prettier](https://prettier.io/) and [ESLint](https://eslint.org/) for code formatting and linting.
- [Vitest](https://vitest.dev/) and [React Testing Library](https://testing-library.com/docs/react-testing-library/intro/) for testing.

Similar to the backend, the frontend is setup in a vertical slices architecture, where each page will have a single file.
This file will contain the page logic, any required components, and any required styles. Where possible, the page logic
will be contained within a single file, however, if there is a lot of shared logic, it might make sense to split this out
into shared components or helper functions.
