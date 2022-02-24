import React, { useEffect, useState } from 'react';
import { Container } from 'semantic-ui-react';
import { Activity } from '../models/activity';  // Activity interface to set type for activities
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import {v4 as uuid} from 'uuid'; // need to install type script description also for this package, from the warning link
import agent from '../api/agent';
import LoadingComponent from './LoadingComponent';

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

  // set state when page is loading
  const [loading, setLoading] = useState(true);

  // set state when submit data to server - submit button will show the loading symbol
  const [submitting, setSubmitting] = useState(false);

  // useEffect hook run the included task after component get rendered
  // same with componentDidMount and ComponentDid...
  useEffect(() => {
    agent.Activities.list()
      .then(response => {      
        let activities: Activity[] = [];
        response.forEach(activity => {
          activity.date = activity.date.split('T')[0];  // get the date part only, remove the time part
          activities.push(activity);
        })
        setActivities(activities);  // response.data is part of axios response property / response.headers, .status ...
        setLoading(false);  // finished loading
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
    // set the submitting state to true
    setSubmitting(true);

    // update activity to database
    if (activity.id) {
      agent.Activities.update(activity).then(() => {
        // update the activities state
        setActivities([...activities.filter(act => act.id !== activity.id), activity]) 
        // update other states
        setSelectedActivity(activity);
        setEditMode(false);
        setSubmitting(false);
      })
    } else {
      // create a new activity id
      activity.id = uuid();
      // save new activity to database
      agent.Activities.create(activity).then(() => {
        // update the Activities state
        setActivities([...activities, activity])  
        // update other states also
        setSelectedActivity(activity);
        setEditMode(false);
        setSubmitting(false);
      })
    }
  }

  // set the activities state when delete an acvitity
  function handleDeleteActivity(id: string) {
    // set the Submitting state to true
    setSubmitting(true);
    // delete the data from server
    agent.Activities.delete(id).then(() => {
      // update the Activities state
    setActivities([...activities.filter(act => act.id !== id)])
      // update other state
      setSubmitting(false);
    })
    
  }

  if (loading) return <LoadingComponent content='Loading app' />

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
          submitting={submitting}
        />  
      </Container>              
    </>
  );
}

export default App;
