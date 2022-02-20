import React, { useEffect, useState } from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios';
import { Header, List } from 'semantic-ui-react';

function App() {

  // useState hook create 'activities' state & the 'setActivities' function that will update it
  // it provides initial value for activities with an empty array '[]'
  const [activities, setActivities] = useState([]);

  // useEffect hook run the included task after component get rendered
  // 
  useEffect(() => {
    axios.get('http://localhost:5000/api/activities')
      .then(response => {
        console.log(response);
        setActivities(response.data);
    })
  }, [])

  return (
    // using Header & List component from Semantic UI
    <div> 
      <Header as='h2' icon='users' content='Reactivities' />  
      <List>
        {activities.map((activity: any) => (
          <List.Item key={activity.id}>
            {activity.title}
          </List.Item>
        ))}
      </List>           
    </div>
  );
}

export default App;
