import { observer } from "mobx-react-lite";
import React, { useEffect } from "react";
import { Grid } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import ActivityList from "./ActivityList";

// wrap function in observer so the change if state can be reflected
export default observer(function ActivityDashboard() {  // destructure the Prop, activities with type of Props
  
  const {activityStore} = useStore();
  const {loadActivities, activityResistry} = activityStore;

  // useEffect hook run the included task after component get rendered
  // same with componentDidMount and ComponentDid...
  useEffect(() => {
    // only load activity from database at app start. 
    // After that it is not needed. But if we cancel at the edit form and go back, 
    // there will be only 1 act in the list ==> need to load from data base in this case also
    if (activityResistry.size <= 1) loadActivities();
  }, [activityResistry.size, loadActivities])

  // show page is loading when start the page and load data from database
  if (activityStore.loadingInitial) return <LoadingComponent content='Loading app' />

  return (
    <Grid>
      <Grid.Column width='10'>
        <ActivityList />
      </Grid.Column>
      <Grid.Column width='6'>
        <h2>Activity filters</h2>
       </Grid.Column>
    </Grid>
  )
})