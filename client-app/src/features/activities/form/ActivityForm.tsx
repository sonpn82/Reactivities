import React, { ChangeEvent, useState } from "react";
import { Button, Form, Segment } from "semantic-ui-react";
import { Activity } from "../../../app/models/activity";

interface Props {
  activity: Activity | undefined;
  closeForm: () => void;
  createOrEdit: (activity: Activity) => void;
}

// refer to activity as selectedActivity to avoid duplicated with below useState function
export default function ActivityForm({activity: selectedActivity, closeForm, createOrEdit}: Props) {

  // set initial state  // ?? operator will return the value inside {} if the activity (left side) is null or undefined
  const initialState = selectedActivity ?? {
    id: '',
    title: '',
    category: '',
    description: '',
    date: '',
    city: '',
    venue: ''
  }

  // useState hook to set the activity state with an initial state
  const [activity, setActivity] = useState(initialState);

  function handleSubmit() {
    createOrEdit(activity);
  }

  // htmltextareaelement is event for the textarea
  function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) {
    const {name, value} = event.target;  // extract name and value of input field
    setActivity({...activity, [name]: value})  // set corresponding field in activity with input value
  }

  return (
    <Segment clearing>
      <Form onSubmit={handleSubmit} autoComplete='off'>
        <Form.Input placeholder='Title' value={activity.title} name='title' onChange={handleInputChange}/>
        <Form.TextArea placeholder='Description' value={activity.description} name='description' onChange={handleInputChange}/>
        <Form.Input placeholder='Category' value={activity.category} name='category' onChange={handleInputChange}/>
        <Form.Input placeholder='Date' value={activity.date} name='date' onChange={handleInputChange}/>
        <Form.Input placeholder='City' value={activity.city} name='city' onChange={handleInputChange}/>
        <Form.Input placeholder='Venue' value={activity.venue} name='venue' onChange={handleInputChange}/>
        <Button floated="right" positive type="submit" content='Submit' />
        <Button 
          floated="right" 
          positive type="button" 
          content='Cancel' 
          onClick={closeForm}
        />
      </Form>
    </Segment>
  )
}