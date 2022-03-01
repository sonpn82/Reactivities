import { observer } from "mobx-react-lite";
import React, { ChangeEvent, useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Button, Form, Segment } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import {v4 as uuid} from 'uuid';
import { useNavigate } from "react-router-dom";

// refer to activity as selectedActivity to avoid duplicated with below useState function
export default observer(function ActivityForm() {

  let navigate = useNavigate();

  const {activityStore} = useStore();
  const {createActivity,
         updateActivity,
         loading,
         loadActivity,
         loadingInitial    
        } = activityStore;
  
  const {id} = useParams<{id: string}>();

  const defaultActivity = {
    id: '',
    title: '',
    category: '',
    description: '',
    date: '',
    city: '',
    venue: ''}

  // useState hook to set the activity state with an initial state
  const [activity, setActivity] = useState(defaultActivity);
 
  // if an id exist in the link parameter then load the activity from that id
  useEffect(() => {
    if (id) {  
      loadActivity(id).then(activity => setActivity(activity!))  // add ! to remove the undefined warning, not a good practice      
    } else {
      setActivity(defaultActivity)
    }
     
  }, [id, loadActivity]);  // effect will only activate if value in [id, loadActivity] change. If not apply this then render will call setActivity => render again => call setAct again ! forever loop!

  // can be create a new activity or just update an activity
  function handleSubmit() {
    if(activity.id.length === 0) {
      // create a new activity
      let newActivity = {
        ...activity,
        id: uuid()
      };
      createActivity(newActivity).then(() => navigate(`/activities/${newActivity.id}`));
    } else {
      // update an activity
      updateActivity(activity).then(() => navigate(`/activities/${activity.id}`));
    }
  }

  // htmltextareaelement is event for the textarea
  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const {name, value} = event.target;  // extract name and value of input field
    setActivity({...activity, [name]: value})  // set corresponding field in activity with input value
  }

  // Show the loading page
  if (loadingInitial) return <LoadingComponent content="Loading activity..." />

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete='off'>
        <Form.Input placeholder='Title' value={activity.title} name='title' onChange={handleInputChange}/>
        <Form.TextArea placeholder='Description' value={activity.description} name='description' onChange={handleInputChange}/>
        <Form.Input placeholder='Category' value={activity.category} name='category' onChange={handleInputChange}/>
        <Form.Input placeholder='Date' value={activity.date} name='date' onChange={handleInputChange} type='date'/>
        <Form.Input placeholder='City' value={activity.city} name='city' onChange={handleInputChange}/>
        <Form.Input placeholder='Venue' value={activity.venue} name='venue' onChange={handleInputChange}/>
        <Button 
          floated="right" 
          positive type="submit" 
          content='Submit' 
          loading={loading}
        />
        <Button 
          floated="right" 
          positive type="button" 
          content='Cancel' 
          as={Link} to='/activities'
        />
      </Form>
    </Segment>
  )
})