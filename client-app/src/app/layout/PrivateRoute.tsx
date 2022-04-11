import { Navigate } from "react-router-dom";
import { useStore } from "../stores/store";

interface Props {
  component: React.ComponentType
  path?: string
}

export const PrivateRoute: React.FC<Props> = ({component: RouteComponent}) => {
  const {userStore: {isLoggedIn}} = useStore();

  if (isLoggedIn ) {
    return <RouteComponent />
  }

  return <Navigate to="/" />
}