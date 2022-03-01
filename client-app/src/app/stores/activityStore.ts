import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Activity } from "../models/activity";

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

  // computed function to sort activities by date
  get activitiesByDate() {
    return Array.from(this.activityResistry.values())
                .sort((a, b) => Date.parse(a.date) - Date.parse(b.date));
  }

  // computed function to group activities by date
  get groupedActivities() {
    // Object.entries cut and object array into 1 array of keys and 1 array of value [[keys],[vals]]
    return Object.entries(
      this.activitiesByDate.reduce((activities, activity) => {
        // get the date of each activity
        const date = activity.date;
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

      // must use runinaction in order to change state in async mode
   
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

  private setActivity = (activity: Activity) => {
    // get the date part only, remove the time part
    activity.date = activity.date.split('T')[0];  

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
  // set the loading state to true & after finish to false
  // update the activities state by adding a new activity to the array
  // set the selectedActivity state to the new activity
  // set the edit mode to false after finish
  createActivity = async (activity: Activity) => {
    this.loading = true;
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
  updateActivity = async (activity:Activity) => {
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
}