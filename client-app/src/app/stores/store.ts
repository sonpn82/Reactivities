import { createContext, useContext } from "react";
import ActivityStore from "./activityStore";
import CommonStore from "./commonStore";
import ModalStore from "./modalStore";
import ProfileStore from "./profileStore";
import UserStore from "./userStore";

// typescript interface for type definition
interface Store {
  activityStore: ActivityStore
  commonStore: CommonStore
  userStore: UserStore
  modalStore: ModalStore
  profileStore: ProfileStore
}

// store to store all the state in our app
export const store: Store = {
  activityStore: new ActivityStore(),  // the activity store to store states related to acitivity
  commonStore: new CommonStore(),  // common store to store state related to error
  userStore: new UserStore(),  // user store to store state relate to user login, register or current user
  modalStore: new ModalStore(), // store state for login part 
  profileStore: new ProfileStore() // store state for profile - profile state and profileLoading state
}

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}