import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import InfiniteScroll from "react-infinite-scroller";
import { Grid, Loader } from "semantic-ui-react";
import { PagingParams } from "../../../app/models/pagination";
import { useStore } from "../../../app/stores/store";
import ActivityFilters from "./ActivityFilters";
import ActivityList from "./ActivityList";
import ActivityListItemPlaceholder from "./ActivityListItemPlaceholder";

// wrap function in observer so the change if state can be reflected
export default observer(function ActivityDashboard() {  // destructure the Prop, activities with type of Props
  
  const {activityStore} = useStore();
  const {loadActivities, activityResistry, setPagingParams, pagination} = activityStore;
  const [loadingNext, setLoadingNext] = useState(false);  // add local state for pagination with initial value of false

  // load more activities by setting new value for pagingParams state
  function handleGetNext() {
    setLoadingNext(true);
    setPagingParams(new PagingParams(pagination!.currentPage + 1));
    loadActivities().then(() => setLoadingNext(false));
  }

  // useEffect hook run the included task after component get rendered
  // same with componentDidMount and ComponentDid...
  useEffect(() => {
    // only load activity from database at app start. 
    // After that it is not needed. But if we cancel at the edit form and go back, 
    // there will be only 1 act in the list ==> need to load from data base in this case also
    if (activityResistry.size <= 1) loadActivities();
  }, [activityResistry.size, loadActivities])

  return (
    <Grid>
      <Grid.Column width='10'>
        {activityStore.loadingInitial && !loadingNext ? (
          <>
            <ActivityListItemPlaceholder />
            <ActivityListItemPlaceholder />
          </>
        ) : (
          <InfiniteScroll
            pageStart={0}
            loadMore={handleGetNext}
            hasMore={!loadingNext && !!pagination && pagination.currentPage < pagination.totalPages}
            initialLoad={false}
          >
            <ActivityList /> 
          </InfiniteScroll>        
        )}
       
      </Grid.Column>
      <Grid.Column width='6'>
        <ActivityFilters />
       </Grid.Column>
       <Grid.Column width={10}>
         <Loader active={loadingNext} />
       </Grid.Column>
    </Grid>
  )
})