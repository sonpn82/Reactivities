import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";
import CommonStore from "./commonStore";

// typescript interface for type definition
interface Store {
  activityStore: ActivityStore
  commonStore: CommonStore
}

// store to store all the state in our app
export const store: Store = {
  activityStore: new ActivityStore(),  // the activity store to store states related to acitivity
  commonStore: new CommonStore()  // common store to store state related to error
}

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}