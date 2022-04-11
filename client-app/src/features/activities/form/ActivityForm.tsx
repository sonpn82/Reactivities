import { observer } from "mobx-react-lite";
import React, { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Button, Segment, Header } from "semantic-ui-react";
import LoadingComponent from "../../../app/layout/LoadingComponent";
import { useStore } from "../../../app/stores/store";
import {v4 as uuid} from 'uuid';
import { useNavigate } from "react-router-dom";
import { Formik, Form } from "formik";
import * as Yup from 'yup';
import MyTextInput from "../../../app/common/form/MyTextInput";
import MyTextArea from "../../../app/common/form/MyTextArea";
import MySelectInput from "../../../app/common/form/MySelectInput";
import { categoryOptions } from "../../../app/common/options/categoryOptions";
import MyDateInput from "../../../app/common/form/MyDateInput";
import { ActivityFormValues } from "../../../app/models/activity";

// refer to activity as selectedActivity to avoid duplicated with below useState function
export default observer(function ActivityForm() {

  let navigate = useNavigate();

  const {activityStore} = useStore();
  const {createActivity,
         updateActivity,        
         loadActivity,
         loadingInitial    
        } = activityStore;
  
  const {id} = useParams<{id: string}>();

  // const defaultActivity = {
  //   id: '',
  //   title: '',
  //   category: '',
  //   description: '',
  //   date: null,
  //   city: '',
  //   venue: ''}

  // to validate the form data in Formik, need to install Yup package
  const validationSchema = Yup.object({
    title: Yup.string().required('The activity title is  required'),
    description: Yup.string().required('The activity description is  required'),
    category: Yup.string().required(),
    date: Yup.string().required('Date is required').nullable(),  // must set to nullable to be same with our activity date
    venue: Yup.string().required(),
    city: Yup.string().required(),
  })

  // useState hook to set the activity state with an initial state
  const [activity, setActivity] = useState<ActivityFormValues>(new ActivityFormValues());
 
  // if an id exist in the link parameter then load the activity from that id
  useEffect(() => {
    if (id) {  
      loadActivity(id).then(activity => setActivity(new ActivityFormValues(activity)))  // add ! to remove the undefined warning, not a good practice      
    } //else {
      //setActivity(defaultActivity)
    //}
     
  }, [id, loadActivity]);  // effect will only activate if value in [id, loadActivity] change. If not apply this then render will call setActivity => render again => call setAct again ! forever loop!

  // can be create a new activity or just update an activity
  function handleFormSubmit(activity: ActivityFormValues) {
    if(!activity.id) {
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

  // Show the loading page
  if (loadingInitial) return <LoadingComponent content="Loading activity..." />

  return (
    // enableReinitialize to re populate the form with new activity value
    <Segment clearing>
      <Header content='Activity Details' sub color='teal'/>
      <Formik 
        validationSchema={validationSchema}
        enableReinitialize 
        initialValues={activity} 
        onSubmit={values => handleFormSubmit(values)}>           
        {({handleSubmit, isValid, isSubmitting, dirty}) => ( // all insize the {} is props of Formik, dirty is if form data is diff with initial val or not
          <Form className="ui form" onSubmit={handleSubmit} autoComplete='off'>
            <MyTextInput name="title" placeholder="Title" />            
            <MyTextArea rows={3} placeholder='Description' name='description'/>
            <MySelectInput options={categoryOptions} placeholder='Category' name='category'/>
            <MyDateInput 
              placeholderText='Date' 
              name='date' 
              showTimeSelect
              timeCaption="time"
              dateFormat='MMMM d, yyyy h:mm aa'
            />
            <Header content='Location Details' sub color='teal'/>
            <MyTextInput placeholder='City' name='city'/>
            <MyTextInput placeholder='Venue'  name='venue'/>
            <Button 
              disabled={isSubmitting || !dirty || !isValid}  // not enable this button if form is in submitting or not changed or not valid
              floated="right" 
              positive type="submit" 
              content='Submit' 
              loading={isSubmitting} // button show loading state (circle rotating symbol) when it is submitting the value
            />
            <Button 
              floated="right" 
              positive type="button" 
              content='Cancel' 
              as={Link} to='/activities'
            />
        </Form>
  
        )}
      </Formik>
    </Segment>
  )
})