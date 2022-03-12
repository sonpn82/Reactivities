import { makeAutoObservable, reaction } from "mobx";
import { ServerError } from "../models/serverError"; // interface with detail of error

// store for error state, login token, appLoaded
export default class CommonStore {
  // error state 
  error: ServerError | null = null;  // initial val is null
  // login token - get from local storage, can be null or a string  
  // initial val can be different ==> need react to the value change of this state
  token: string | null = window.localStorage.getItem('jwt');    
  appLoaded = false;  // initial val is false

  constructor() {
    makeAutoObservable(this);

    // use mobx reaction for token initial value change
    // reaction run when there is changed to the token state
    reaction( 
      () => this.token,
      token => {
        if (token) {
          window.localStorage.setItem('jwt', token)
        } else {
          window.localStorage.removeItem('jwt')
        }
      })
  }

  // action to set the error state
  setServerError = (error: ServerError) => {
    this.error = error;
  }
 
  // and set the token state to that token
  setToken = (token: string | null) => {    
    this.token = token;
  }

  // set the AppLoaded state to true
  setAppLoaded = () => {
    this.appLoaded = true;
  }
}