import { observer } from "mobx-react-lite";
import React, { useEffect } from "react";
import { useParams } from "react-router-dom";
import { Grid } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import ActivityDetailChat from "./ActivityDetailedChat";
import ActivityDetailHeader from "./ActivityDetailedHeader";
import ActivityDetailInfo from "./ActivityDetailedInfo";
import ActivityDetailSidebar from "./ActivityDetailedSidebar";

export default observer(function ActivityDetails() {

  const {activityStore} = useStore();
  const {selectedActivity: activity, loadActivity, loadingInitial} = activityStore;
  const {id} = useParams<{id: string}>();  // id from route parameters activity/:id
  
  useEffect(() => {
    if (id) loadActivity(id);
  }, [id, loadActivity])

  // return nothing if activity is undefined, to avoid error in rendering below
  // need to return a html element or activityDashboard will have error
  if (loadingInitial || !activity) return <LoadingComponent />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <ActivityDetailHeader activity={activity}/>
        <ActivityDetailInfo activity={activity}/>
        <ActivityDetailChat />
      </Grid.Column>
      <Grid.Column width={6}>
        <ActivityDetailSidebar />
      </Grid.Column>
    </Grid>
  )
})