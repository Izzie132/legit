import { Route, Routes } from "react-router-dom";
import { Home } from "@/features/Home.tsx";
import { Weather } from "@/features/weather/Weather.tsx";
import { Layout } from "@/components/layout/Layout.tsx";
import { Counter } from "@/features/counter/Counter.tsx";
import { UserList } from "@/features/users/user-list/UserList.tsx";
import { SignUp } from "@/features/users/sign-up/SignUp.tsx";

function App() {
  return (
    <>
      <Routes>
        <Route path="/" element={<Layout />}>
          <Route path="/" element={<Home />} />
          <Route path="/weather" element={<Weather />} />
          <Route path="/counter" element={<Counter />} />
          <Route path="/user-list" element={<UserList />} />
          <Route path="/sign-up" element={<SignUp />} />
        </Route>
      </Routes>
    </>
  );
}

export default App;
