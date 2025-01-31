import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Route, Routes } from "react-router-dom";
import { ApiClientContextProvider } from "@/api/useApiClient";
import { Layout } from "@/components/layout/Layout";
import { Counter } from "@/features/counter/Counter";
import { Home } from "@/features/Home";
import { SignUp } from "@/features/users/sign-up/SignUp";
import { UserList } from "@/features/users/user-list/UserList";
import { Weather } from "@/features/weather/Weather";

const queryClient = new QueryClient();

export const App = () => (
  <>
    <ApiClientContextProvider>
      <QueryClientProvider client={queryClient}>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route path="/" element={<Home />} />
            <Route path="/weather" element={<Weather />} />
            <Route path="/counter" element={<Counter />} />
            <Route path="/user-list" element={<UserList />} />
            <Route path="/sign-up" element={<SignUp />} />
          </Route>
        </Routes>
      </QueryClientProvider>
    </ApiClientContextProvider>
  </>
);
