import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Activity, ActivityFormValues } from "../models/activity";
import {format} from 'date-fns';  // to format Date object
import { store } from "./store";
import { Profile } from "../models/profile";

export default class ActivityStore {

  // list of all states used in our program activity
  activityResistry = new Map<string, Activity>();
  selectedActivity: Activity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;

  constructor() {
    makeAutoObservable(this)
  }

  // computed function to sort activities by date
  get activitiesByDate() {
    return Array.from(this.activityResistry.values())
                .sort((a, b) => a.date!.getTime() - b.date!.getTime());
  }

  // computed function to group activities by date
  get groupedActivities() {
    // Object.entries cut and object array into 1 array of keys and 1 array of value [[keys],[vals]]
    return Object.entries(
      this.activitiesByDate.reduce((activities, activity) => {
        // get the date of each activity as a string with date only
        const date = format(activity.date!, 'dd MMM yyyy');
        // if have array activities[date] then add that activity to the activities[date] array.
        // If not, create a new activities[date] array with that activity
        activities[date] = activities[date] ? [...activities[date], activity] : 
                                              [activity]
        return activities;
      }, {} as {[key: string]: Activity[]})
    )
  }

  loadActivities = async () => {   
    this.loadingInitial = true;
    try {      
      // get the list of activity from database
      const activities = await agent.Activities.list();     
   
        // format the date string of each activity
      activities.forEach(activity => {
        this.setActivity(activity);
      })

      this.setLoadingInitial(false);         

    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  }

  // load an activity from its id
  loadActivity = async (id:string) => {
    // check if we can get activity from current memory
    let activity = this.getActivity(id);
    if (activity) {
      // set the selectedActivity state
      this.selectedActivity = activity;
      // return an activity from this promise
      return activity;

    } else {
      // if not then load the activity from database using agent
      this.loadingInitial = true;      
      try {
        activity = await agent.Activities.details(id);
        // modify the date format
        this.setActivity(activity);
        runInAction(() => {
          // set the selectedActivity status
          this.selectedActivity = activity;
        })       
        // set loadingInitial state to false
        this.setLoadingInitial(false);
        // return an activity from this promis
        return activity;

      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  }

  // set the activityRegistry state by adding a new activity to it
  // config the isGoing, isHost, host and date field of activity
  private setActivity = (activity: Activity) => {
    // get user state from user store
    const user = store.userStore.user;
    // if user is login (have user state) then
    if (user) {
      // if username exist in the attendees list of event then user isGoing to this event (isGoing set=true)
      activity.isGoing = activity.attendees!.some(
        a => a.username === user.username
      )
      // check if user is host or not
      activity.isHost = activity.hostUsername === user.username;
      // find the host of the activity
      activity.host = activity.attendees?.find(x => x.username === activity.hostUsername);      
    }
    // get the date part only, remove the time part
    activity.date = new Date(activity.date!);  

    // push the activity to activities state array 
    this.activityResistry.set(activity.id, activity);
  }

  // get an activity in the activityResistry by id
  private getActivity = (id: string) => {
    return this.activityResistry.get(id);
  }

  // set the loadingInitial state
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  }
  
  // create a new activity  
  // update the activitieRegistry state by adding a new activity to the array
  // set the selectedActivity state to the new activity  
  createActivity = async (activity: ActivityFormValues) => {
    const user = store.userStore.user;
    const attendee = new Profile(user!);

    try {
      // use agent to create a new activity in database
      await agent.Activities.create(activity);

      // set the create from value to the Activity object
      const newActivity = new Activity(activity);

      // add missing fields to the Activity object
      newActivity.hostUsername = user!.username;
      newActivity.attendees = [attendee];  // array of attendees - 1st one is the user

      // add the activity to the activityRegistry state and config remain field like isGoing, isHost, host...
      this.setActivity(newActivity);

      // update the selectedActivity state
      runInAction(() => {      
        this.selectedActivity = newActivity;  
      })
    } catch (error) {
      console.log(error)  
    }
  }

  // update the activities state
  // set the selectedActivity to the new activity
  updateActivity = async (activity:ActivityFormValues) => {

    try {
      // use the agent to update the activity to database
      await agent.Activities.update(activity);    
      runInAction(() => {
        if (activity.id) {
          // using spread operator to get all fields of current activity 
          // and then overwrite them with the same field in new activity
          let updatedActivity = {...this.getActivity(activity.id), ...activity}
          this.activityResistry.set(activity.id, updatedActivity as Activity);  // must use as Activity to avoid type error warning
          this.selectedActivity = updatedActivity as Activity;
        }               
      }) 
    } catch (error) {
      console.log(error);    
    }
  }

  // delete an activity
  // set the activities state with one act removed
  // set the loading state to true then after finish to false
  deleteActivity = async (id:string) => {
    this.loading = true;
    try {
      await agent.Activities.delete(id);
      runInAction(() => {
        this.activityResistry.delete(id);
        this.loading = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      })
    }
  }

  // update attendance list in an activity
  // modify the loading state: => true => false
  // modify the selectedActivity state (isGoing, attendees state)
  updateAttendance = async () => {
    const user = store.userStore.user;
    this.loading = true;
    try {
      // post the user id to the API attend end point
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        // if user currently is going to this event then that mean he will leave this event
        if (this.selectedActivity?.isGoing) {
          this.selectedActivity.attendees = this.selectedActivity.attendees?.filter(
            a => a.username !== user?.username);
          this.selectedActivity.isGoing = false;
        } else {  //  if user did not join this event yet then they will join this event now - their profile will be pushed to attendees list
          const attendee = new Profile(user!);
          this.selectedActivity?.attendees?.push(attendee);
          this.selectedActivity!.isGoing = true;
        }
        // update the activity in activity list (activityRegistry)
        this.activityResistry.set(this.selectedActivity!.id, this.selectedActivity!)
      })
    } catch (error) {
      
    } finally {
      runInAction(() => this.loading = false);
    }
  }

  // toggle the isCancelled field to true or false
  // set the loading state to true then false
  // update the selectedActivity state and activityRegistry state
  cancelActivityToggle =async () => {
    this.loading = true;
    try {
      // use the attend end point to either attend the activity or leave (cancel) the activity
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        // toggle the isCancelled field in selectedActivity state
        this.selectedActivity!.isCancelled = !this.selectedActivity?.isCancelled;

        // update the activityRegistry state
        this.activityResistry.set(this.selectedActivity!.id, this.selectedActivity!);
      })
    } catch (error) {
      console.log(error)
    } finally {
      runInAction (() => this.loading = false);
    }
  }

  // clear the selected activity from memory
  // if not clear then ChatHub will have initial connection error when switch from activity to other activity
  clearSelectedActivity = () => {
    this.selectedActivity = undefined;
  }
}