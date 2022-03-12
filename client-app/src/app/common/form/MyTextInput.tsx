import { useField } from "formik";
import React from "react";
import { Form, Label } from "semantic-ui-react";

interface Props {  // all properties of MyTextInput
  placeholder: string;
  name: string;
  type?: string;
  label?: string;
}

export default function MyTextInput(props: Props) {
  const [field, meta] = useField(props.name);
  return (
    // !! to convert a value to boolean - true or false
    // set mytextbox with a label, an input and an error text 
    <Form.Field error={meta.touched && !!meta.error}> 
      <label>{props.label}</label>  
      <input {...field} {...props} /> 
      {meta.touched && meta.error? (  // set error text when input has error
        <Label basic color='red'>{meta.error}</Label>
      ) : null}
    </Form.Field>
  )
}