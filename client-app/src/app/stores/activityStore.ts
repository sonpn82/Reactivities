import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Activity } from "../models/activity";
import {v4 as uuid} from 'uuid';

export default class ActivityStore {

  // list of all states used in our program
  activityResistry = new Map<string, Activity>();
  selectedActivity: Activity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = true;

  constructor() {
    makeAutoObservable(this)
  }

  get activitiesByDate() {
    return Array.from(this.activityResistry.values())
                .sort((a, b) => Date.parse(a.date) - Date.parse(b.date));
  }

  loadActivities = async () => {   
    try {      
      // get the list of activity from database
      const activities = await agent.Activities.list();

      // must use runinaction in order to change state in async mode
   
        // format the date string of each activity
      activities.forEach(activity => {
        activity.date = activity.date.split('T')[0];  // get the date part only, remove the time part

        // push the activity to activities state array 
        this.activityResistry.set(activity.id, activity);
      })

      this.setLoadingInitial(false);         

    } catch (error) {
      console.log(error);
      this.setLoadingInitial(false);
    }
  }

  // set the loadingInitial state
  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  }

  // set the selectedActivity state to either an activity or undefined
  // find can return undefined so selectedActivity must included type undefined
  selectActivity = (id: string) => {
    this.selectedActivity = this.activityResistry.get(id);
  }

  // set the selectedActivity state to undefined
  cancelSelectedActivity = () => {
    this.selectedActivity = undefined;
  }
  
  // set the selectedActivity state & editMode state
  // open the edit form with an id (?=optional) or cancel the selected activity
  openForm = (id?: string) => {
    id ? this.selectActivity(id) : this.cancelSelectedActivity();
    this.editMode = true;
  }

  // change the editMode state to false
  closeForm = () => {
    this.editMode = false;
  }

  // create a new activity
  // set the loading state to true & after finish to false
  // update the activities state by adding a new activity to the array
  // set the selectedActivity state to the new activity
  // set the edit mode to false after finish
  createActivity = async (activity: Activity) => {
    this.loading = true;
    activity.id = uuid();
    try {
      // use agent to create a new activity in database
      await agent.Activities.create(activity);
      runInAction(() => {
        this.activityResistry.set(activity.id, activity);
        this.selectedActivity = activity;
        this.editMode = false;
        this.loading = false;
      })
    } catch (error) {
      console.log(error)
      runInAction(() => {  
        this.loading = false;
      })
    }
  }

  // update the activities state
  // set the selectedActivity to the new activity
  // set the editmode state to false
  // set the loading state to true then after finish to false
  updateActivity =async (activity:Activity) => {
    this.loading = true;
    try {
      // use the agent to update the activity to database
      await agent.Activities.update(activity);    
      runInAction(() => {
        this.activityResistry.set(activity.id, activity);
        this.selectedActivity = activity;  
        this.editMode = false;
        this.loading = false;            
      }) 
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;            
      })
    }
  }

  // delete an activity
  // set the activities state with one act removed
  // set the loading state to true then after finish to false
  deleteActivity =async (id:string) => {
    this.loading = true;
    try {
      await agent.Activities.delete(id);
      runInAction(() => {
        this.activityResistry.delete(id);
        if (this.selectedActivity?.id === id) this.cancelSelectedActivity();
        this.loading = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loading = false;
      })
    }
  }
}