import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { Container } from 'semantic-ui-react';
import { Activity } from '../models/activity';  // Activity interface to set type for activities
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import {v4 as uuid} from 'uuid'; // need to install type script description also for this package, from the warning link

function App() {

  // useState hook create 'activities' state & the 'setActivities' function that will update it
  // it provides initial value for activities with an empty array '[]'
  const [activities, setActivities] = useState<Activity[]>([]);  // <Activity[]> => activities = Activity array

  // set state when user select an activity to view
  // initial state is undefined (nothing is selected)
  const [selectedActivity, setSelectedActivity] = useState<Activity | undefined>(undefined);

  // set state when user edit an activity
  // initial state is false
  const [editMode, setEditMode] = useState(false);

  // useEffect hook run the included task after component get rendered
  // same with componentDidMount and ComponentDid...
  useEffect(() => {
    axios.get<Activity[]>('http://localhost:5000/api/activities')
      .then(response => {      
        setActivities(response.data);  // response.data is part of axios response property / response.headers, .status ...
    })
  }, [])

  // set the state of selectedActivity to the id of selected activity
  function handleSelectActivity(id: string) {
    setSelectedActivity(activities.find(activity => activity.id === id));
  }

  // set the state of selectedActivity to undefined
  function handleCancelActivity() {
    setSelectedActivity(undefined);
  }

  // if have an id then set selectedActivity to that activity, if not, set to undefined
  // set the edit mode to true
  function handleFormOpen(id?: string) {  // id? can have an id or not
    id ? handleSelectActivity(id) : handleCancelActivity();
    setEditMode(true);
  }

  // set the editmode to false
  function handleFormClose() {
    setEditMode(false);
  }

  // set the activities state by either create a new activity or edit an activity
  function handleCreateOrEditActivity(activity: Activity) {
    activity.id
      ? setActivities([...activities.filter(act => act.id !== activity.id), activity])  // edit an activity
      : setActivities([...activities, {...activity, id: uuid()}])  // input new activity with id from uuid

    // set other state also
    setEditMode(false);  // not in edit mode
    setSelectedActivity(activity);  // switch to view mode
  }

  // set the activities state when delete an acvitity
  function handleDeleteActivity(id: string) {
    setActivities([...activities.filter(act => act.id !== id)])
  }

  return (
    // using components from Semantic UI - <> is shortcut for <Fragment> component
    // passdown activities state to ActivityDashboard - need to create an interface in ActivityDashboard to verify type of this var
    <> 
      <NavBar openForm={handleFormOpen} /> 
      <Container style={{marginTop: '7em'}}>
        <ActivityDashboard 
          activities={activities}  
          selectedActivity={selectedActivity}
          selectActivity={handleSelectActivity}
          cancelSelectActivity={handleCancelActivity}
          editMode={editMode}
          openForm={handleFormOpen}
          closeForm={handleFormClose}
          createOrEdit={handleCreateOrEditActivity}
          deleteActivity={handleDeleteActivity}
        />  
      </Container>              
    </>
  );
}

export default App;
