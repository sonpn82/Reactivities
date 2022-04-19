import { ErrorMessage, Form, Formik } from "formik";
import { observer } from "mobx-react-lite";
import React from "react";
import { Button, Header, Label } from "semantic-ui-react";
import MyTextInput from "../../app/common/form/MyTextInput";
import { useStore } from "../../app/stores/store";

export default observer(function LoginForm() {
  const {userStore} = useStore();
  return (
    // formik automatically create a loading symbol when we are submitting the form data
    <Formik
      initialValues={{email: '', password: '', error: null}}
      onSubmit={(values, {setErrors}) => userStore
        .login(values)
        .catch(error => setErrors({error: error.response.data}))} //replace this error:'Invalid email or password'}))} when use email verify
    > 
      {({handleSubmit, isSubmitting, errors}) => (
        <Form className='ui form' onSubmit={handleSubmit} autoComplete='off'>
          <Header as='h2' content='Login to Reactivities' color="teal" textAlign="center"/>
          <MyTextInput name='email' placeholder="Email" />
          <MyTextInput name='password' placeholder="Password" type="password"/>
          <ErrorMessage
            name="error" 
            render={() => 
              <Label style={{marginBottom: 10}} 
                     basic color='red' content={errors.error} />
            }
          />
          <Button loading={isSubmitting} positive   
                  content='Login' type="submit" fluid />  
        </Form>
      )}
    </Formik>
  )
})