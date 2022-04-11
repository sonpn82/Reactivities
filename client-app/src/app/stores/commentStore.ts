import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { makeAutoObservable, runInAction } from "mobx";
import { ChatComment } from "../models/comment";
import { store } from "./store";

export default class CommentStore {
  comments: ChatComment[] = [];
  hubConnection: HubConnection | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  createHubConnection = (activityId: string) => {
    if (store.activityStore.selectedActivity) {
      // config the connection
      this.hubConnection = new HubConnectionBuilder()
        .withUrl(process.env.REACT_APP_CHAT_URL + '?activityId=' + activityId, { // ? is start of a query string  // react app environtment variable should start with REACT_APP_
          accessTokenFactory: () => store.userStore.user?.token!
        }) 
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      // start the connection
      this.hubConnection.start().catch(
        error => console.log("error establishing the connection", error));
      
      // LoadComments text must be same with that text in ChatHub.cs in server side
      this.hubConnection.on('LoadComments', (comments: ChatComment[]) => {
        // update the comments state with the loaded comments
        runInAction(() => {
          comments.forEach(comment => {
            comment.createdAt = new Date(comment.createdAt + 'Z');  // add Z so we can get comment info from database to get correct time
          })
          this.comments = comments;}
        )})
      
      // add a comment to the comment state
      this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
        runInAction(() => {
          comment.createdAt = new Date(comment.createdAt);
          this.comments.unshift(comment)  // use unshift instead of push to input an element to start of an array
        });
      })
    }
  }

  // stop the connection
  stopHubConnection = () => {
    this.hubConnection?.stop().catch(error => console.log('Error stopping connection: ', error))
  }

  // remove all comments from store
  clearComments = () => {
    this.comments = [];
    this.stopHubConnection();
  }

  // add a comment to store
  addComment =async (values:any) => {
    values.activityId = store.activityStore.selectedActivity?.id;
    try {
      // call the SendComment method on hubConnection
      // the SendComment will call the ReceiveComment method (in ChatHub) => also add this comment to the comment store
      await this.hubConnection?.invoke('SendComment', values);  
    } catch (error) {
      console.log(error);
    }
  }
}