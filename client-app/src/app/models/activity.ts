import { Profile } from "./profile";

export interface Activity {
  id: string;
  title: string;
  date: Date | null;
  description: string;
  category: string;
  city: string;
  venue: string;
  hostUsername: string;  
  isCancelled: boolean;  // activity is cancelled or not - same name with prop in Activity table
  isGoing: boolean;  // check if current User go to this event or not
  isHost: boolean;  // check if current user is host of this event or not - Same name with prop in ActivityAttendees table
  host?: Profile;  // who is the host of the activity
  attendees: Profile[]; // who are the attendees of the activity
}

// create a new Activity class which has same field as the Activity interface
// and get init from ActivityFormValues which has less field than the Activity interface
export class Activity implements Activity {
  constructor(init?: ActivityFormValues) {
    Object.assign(this, init);
  }
}

// Edit and update Form value of Activity
// Has less field than the actual Activity object
// The ActivityFormValues will take all the data in the Edit and update Form of activity
// and pass these data to the Activity Class
export class ActivityFormValues {
  id?: string = undefined;
  title: string = '';
  category: string = '';
  description: string = '';
  date: Date | null = null;
  city: string = '';
  venue: string = '';

  constructor(activity?: ActivityFormValues) {
    if (activity) {
      this.id = activity.id;
      this.title = activity.title;
      this.category = activity.category;
      this.description = activity.description;
      this.date = activity.date;
      this.venue = activity.venue;
      this.city = activity.city;
    }
  }
}