import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { User, UserFormValues } from "../models/user";
import { store } from "./store";
import { history } from "../..";

// userStore to store state related to user login, current user
export default class UserStore {
  user: User | null = null;  // user state, initial val = null

  constructor() {
    makeAutoObservable(this)
  }

  // !! to convert an object into a bool value
  // check if is log in or not
  get isLoggedIn() {
    return !!this.user;
  }

  // use agent login function to login
  login = async (creds:UserFormValues) => {
    try {
      // try to login using agent.Account.login
      const user = await agent.Account.login(creds);
      // if ok store the token to local storage and set the token state to that token
      store.commonStore.setToken(user.token)
      // set the user state here to that user also
      // need to user runInAction here - because need to wait from await agent...
      runInAction(() => this.user = user);
      // move user to activities page after login success
      history.push('/activities')
      // close the modal page
      store.modalStore.closeModal();
    } catch (error) {
      throw error;
    }
  }

  // logout function
  logout = () => {
    // set the token state to null
    store.commonStore.setToken(null);
    // remove jwt token from local storage
    window.localStorage.removeItem('jwt');
    // set the user state to null
    this.user = null;
    // move user to homepage
    history.push('/');
  }

  getUser =async () => {
    try {
      // get the current user using agent.Account.current
      const user = await agent.Account.current();
      // set the user state to this user
      // need to user runInAction to change state after await
      runInAction(() => this.user = user);
    } catch (error) {
      console.log(error);
    }
  }

  register =async (creds:UserFormValues) => {
    try {
      // try to register using agent.Account.register
      const user = await agent.Account.register(creds);
      // if ok store the token to local storage and set the token state to that token
      store.commonStore.setToken(user.token)
      // set the user state here to that user also
      // need to user runInAction here - because need to wait from await agent...
      runInAction(() => this.user = user);
      // move user to activities page after login success
      history.push('/activities')
      // close the modal page
      store.modalStore.closeModal();
    } catch (error) {
      throw error;
    }   
  }

  // set image state for user
  setImage = (image: string) => {
    if (this.user) this.user.image = image;    
  }

  // set displayName for user
  setDisplayName = (name: string) => {
    if (this.user) this.user.displayName = name;
  }
}