import { observer } from "mobx-react-lite";
import React, { Fragment } from "react";
import { Header } from "semantic-ui-react";
import { useStore } from "../../../app/stores/store";
import ActivityListItem from "./ActivityListItem";

export default observer(function ActivityList() {

  const {activityStore} = useStore();
  const {groupedActivities} = activityStore;

  // group is a string of date
  return (
    <>  
      {groupedActivities.map(([group, activities]) => (
        <Fragment key={group}> 
          <Header sub color="teal">
            {group}
          </Header>        
          {activities.map(activity => (
            // passdown the activity to render each activity item in list
            <ActivityListItem key={activity.id} activity={activity}/>
          ))}   
        </Fragment>
      ))}
    </>
    )
})