import { makeAutoObservable } from "mobx";
import { ServerError } from "../models/serverError"; // interface with detail of error

// store for error state
export default class CommonStore {
  // error state 
  error: ServerError | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  // action to set the error state
  setServerError = (error: ServerError) => {
    this.error = error;
  }
}