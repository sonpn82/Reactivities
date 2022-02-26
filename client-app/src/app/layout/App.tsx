import React, { useEffect } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import LoadingComponent from './LoadingComponent';
import { useStore } from '../stores/store';
import { observer } from 'mobx-react-lite';

function App() {

  const {activityStore} = useStore();

  // useEffect hook run the included task after component get rendered
  // same with componentDidMount and ComponentDid...
  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore])

  // show page is loading when start the page and load data from database
  if (activityStore.loadingInitial) return <LoadingComponent content='Loading app' />

  return (
    // using components from Semantic UI - <> is shortcut for <Fragment> component
    // passdown activities state to ActivityDashboard - need to create an interface in ActivityDashboard to verify type of this var
    <> 
      <NavBar /> 
      <Container style={{marginTop: '7em'}}>    
        <ActivityDashboard />  
      </Container>              
    </>
  );
}

export default observer(App);
