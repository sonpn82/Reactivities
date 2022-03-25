import React, { useEffect } from 'react';
import { Container } from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import { observer } from 'mobx-react-lite';
import { Route, Routes, useLocation } from 'react-router-dom';
import HomePage from '../../features/home/HomePage';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../features/errors/TestError';
import { ToastContainer } from 'react-toastify';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';
import LoginForm from '../../features/users/LoginForm';
import { useStore } from '../stores/store';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../common/modals/ModalContainer';
import ProfilePage from '../../features/profiles/ProfilePage';

function App() { 
  const {commonStore, userStore} = useStore();

  useEffect(() => {
    if (commonStore.token) {
      userStore.getUser().finally(() => commonStore.setAppLoaded());
    } else {
      commonStore.setAppLoaded();
    }
  }, [commonStore, userStore])

  if (!commonStore.appLoaded) return <LoadingComponent content='Loading app...' />

  return (
    // ToastContainer to show toast error (a quick self-closed message)
    // ModalContainer to show a temporary input or message panel above the main page - like a message box
    <> 
      <ModalContainer />      
      <ToastContainer position='bottom-right' hideProgressBar />
      <Routes>        
        <Route path='/' element={<HomePage />} />
        <Route path='/*' element={<Nav />} />
      </Routes>                     
    </>
  );
}

// separate navbar and homepage - do not show navbar on homepage
function Nav() {
  // using components from Semantic UI - <> is shortcut for <Fragment> component
  // passdown activities state to ActivityDashboard - need to create an interface in ActivityDashboard to verify type of this var
    
  const location = useLocation();

  return (
    <>
      <NavBar /> 
      <Container style={{marginTop: '7em'}}>  
        <Routes>        
          <Route path='/activities' element={<ActivityDashboard />} />
          <Route path='/activities/:id' element={<ActivityDetails />} />
          <Route key={location.key} path='/createActivity' element={<ActivityForm />} />
          <Route key={location.key} path='/manage/:id' element={<ActivityForm />} />
          <Route path='/profiles/:username' element={<ProfilePage />} />
          <Route path='/errors' element={<TestErrors />} />
          <Route path='/server-error' element={<ServerError />} />
          <Route path='/login' element={<LoginForm />} />
          <Route path='*' element={<NotFound />} />
        </Routes>  
      </Container>
    </>
  )
}

export default observer(App);
