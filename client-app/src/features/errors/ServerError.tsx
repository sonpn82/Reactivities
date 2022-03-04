import { observer } from "mobx-react-lite";
import React from "react";
import { Container, Header, Segment } from "semantic-ui-react";
import { useStore } from "../../app/stores/store";

// The component to be display in /server-error route 
// to show the server error message
export default observer(function ServerError() {
  // extract the common store which contains the error state
  const {commonStore} = useStore();
  // error can be null so add ? to be 'error?.message' to avoid exception
  // render the component from the error detail
  return (
    <Container>
      <Header as='h1' content='Server Error' />
      <Header sub as='h5' color="red" content={commonStore.error?.message} />  
      {commonStore.error?.details && (
        <Segment>
          <Header as='h4' content='Stack trace' color="teal" />
          <code style={{marginTop: '10px'}}>{commonStore.error.details}</code>
        </Segment>
      )}
    </Container>
  )
})