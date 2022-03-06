import { useField } from "formik";
import React from "react";
import { Form, Label } from "semantic-ui-react";
import DatePicker, {ReactDatePickerProps} from 'react-datepicker';

// Partial: make all property inside ReactDatePickerProps optional
export default function MyDateInput(props: Partial<ReactDatePickerProps>) {
  const [field, meta, helpers] = useField(props.name!); // add ! to remove null warning
  return (
    // !! to convert a value to boolean - true or false
    <Form.Field error={meta.touched && !!meta.error}>
      <DatePicker 
        {...field}
        {...props}
        selected={(field.value && new Date(field.value)) || null}
        onChange={value => helpers.setValue(value)}
      />
      {meta.touched && meta.error? (
        <Label basic color='red'>{meta.error}</Label>
      ) : null}
    </Form.Field>
  )
}