import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { User, UserFormValues } from "../models/user";
import { store } from "./store";
import { history } from "../..";

// userStore to store state related to user login, current user
export default class UserStore {
  user: User | null = null;  // user state, initial val = null
  fbAccessToken: string | null = null; // for facebook login - save the token
  fbLoading = false;  // for facebook login
  refreshTokenTimeout: any;  // after app finish, for refresh token

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

      this.startRefreshTokenTimer(user);  // add after app finish - to refresh token

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
      store.commonStore.setToken(user.token);  // set after app finish - for refresh token
      // set the user state to this user
      // need to user runInAction to change state after await
      runInAction(() => this.user = user);

      this.startRefreshTokenTimer(user);  // add after app finish - to refresh token
    } catch (error) {
      console.log(error);
    }
  }

  register =async (creds:UserFormValues) => {
    try {
      // remove below code when use email verf
      // try to register using agent.Account.register 
      // const user = await agent.Account.register(creds);
      // if ok store the token to local storage and set the token state to that token
      //store.commonStore.setToken(user.token)           
      //this.startRefreshTokenTimer(user);  // add after app finish - to refresh token

      // set the user state here to that user also
       // need to user runInAction here - because need to wait from await agent...
      //runInAction(() => this.user = user);
      // move user to activities page after login success

      // add below
      await agent.Account.register(creds);
      history.push(`/account/registerSuccess?email=${creds.email}`);

      // remove this when use email verify
      //history.push('/activities')
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

  // set the fbAccessToken
  // check if user facebook is login or not
  getFacebookLoginStatus = async () => {
    window.FB.getLoginStatus(response => {
      if (response.status === 'connected') {
        this.fbAccessToken = response.authResponse.accessToken;
      }
    })
  }

  // for facebook login
  facebookLogin = () => {
    this.fbLoading = true;
    const apiLogin = (accessToken: string) => {
      agent.Account.fbLogin(accessToken).then(user => {
        store.commonStore.setToken(user.token);
        this.startRefreshTokenTimer(user);  // add after app finish - to refresh token
        runInAction(() => {
          this.user = user;
          this.fbLoading = false;
        })
        history.push('/activities');
      }).catch(error => {
        console.log(error);
        runInAction(() => this.fbLoading = false);
      })
    }

    if (this.fbAccessToken) {
      apiLogin(this.fbAccessToken);      
    } else {
      window.FB.login(response => {
        apiLogin(response.authResponse.accessToken);
      }, {scope: 'public_profile, email'})
    }
  }

  // after app finish - to get the refresh token from api
  refreshToken = async () => {
    this.stopRefreshTokenTimer();
    try {
      const user = await agent.Account.refreshToken();
      runInAction(() => this.user = user);
      store.commonStore.setToken(user.token);
      this.startRefreshTokenTimer(user);
    } catch (error) {
      console.log(error);
    }
  }

  private startRefreshTokenTimer(user: User) {
    const jwtToken = JSON.parse(atob(user.token.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1000);
    const timeout = expires.getTime() - Date.now() - (60*1000); // 60 secs
    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout)
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}