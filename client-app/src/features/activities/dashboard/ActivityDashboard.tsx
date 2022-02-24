import React from "react";
import { Grid } from "semantic-ui-react";
import { Activity } from "../../../app/models/activity";
import ActivityDetails from "../details/ActivityDetails";
import ActivityForm from "../form/ActivityForm";
import ActivityList from "./ActivityList";

// set type for the activities prop, which is passed down from App.tsx
interface Props {
  activities: Activity[];
  selectedActivity: Activity | undefined;
  selectActivity: (id: string) => void;
  cancelSelectActivity: () => void;
  editMode: boolean;
  openForm: (id: string) => void;
  closeForm: () => void;
  createOrEdit: (activity: Activity) => void;
  deleteActivity: (id: string) => void;
}

export default function ActivityDashboard({activities, selectedActivity, 
                        selectActivity, cancelSelectActivity,
                        editMode, openForm, closeForm, createOrEdit, deleteActivity}: Props) {  // destructure the Prop, activities with type of Props
  return (
    <Grid>
      <Grid.Column width='10'>
        <ActivityList 
          activities={activities}
          selectActivity={selectActivity}
          deleteActivity={deleteActivity}
          />
      </Grid.Column>
      <Grid.Column width='6'>
        {selectedActivity && !editMode &&   // only render the ActivityDetails if an activity is selected and not in edit mode
        <ActivityDetails 
          activity={selectedActivity} 
          cancelSelectActivity={cancelSelectActivity}  
          openForm={openForm}
        />}  
        {editMode &&  // only render the ActivityForm if it is in editMode
          <ActivityForm 
            closeForm={closeForm}
            activity={selectedActivity}
            createOrEdit={createOrEdit}
          /> 
        }
      </Grid.Column>
    </Grid>
  )
}