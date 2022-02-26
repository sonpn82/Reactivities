import { observer } from "mobx-react-lite";
import React from "react";
import { Grid } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import ActivityDetails from "../details/ActivityDetails";
import ActivityForm from "../form/ActivityForm";
import ActivityList from "./ActivityList";

// wrap function in observer so the change if state can be reflected
export default observer(function ActivityDashboard() {  // destructure the Prop, activities with type of Props
  
  const {activityStore} = useStore();
  const {selectedActivity, editMode} = activityStore;

  return (
    <Grid>
      <Grid.Column width='10'>
        <ActivityList />
      </Grid.Column>
      <Grid.Column width='6'>
        {selectedActivity && !editMode &&   // only render the ActivityDetails if an activity is selected and not in edit mode
        <ActivityDetails />}  
        {editMode &&  // only render the ActivityForm if it is in editMode
          <ActivityForm /> 
        }
      </Grid.Column>
    </Grid>
  )
})