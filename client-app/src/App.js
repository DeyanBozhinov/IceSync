import React from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Root from "./components/pages/RootPage";
import WorkflowPage from "./components/pages/WorkflowPage";

const routes = createBrowserRouter([
  {
    path: "",
    element: <Root />,
    children: [
      {
        index: true,
        element: <WorkflowPage />,
      },
    ],
  },
]);

const App = () => {
  return <RouterProvider router={routes}></RouterProvider>;
};

export default App;