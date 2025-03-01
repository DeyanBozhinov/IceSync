import { Outlet } from "react-router-dom";
import Navbar from "../UI/Navbar";

const Root = () => {
  return (
    <>
        <Navbar/>
        <main>
            <Outlet />
        </main>
    </>
  );
};

export default Root;