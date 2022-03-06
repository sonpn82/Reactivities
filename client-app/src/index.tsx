import React, { useLayoutEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import 'react-calendar/dist/Calendar.css';
import './app/layout/styles.css';
import 'react-toastify/dist/ReactToastify.css';
import 'react-datepicker/dist/react-datepicker.css';
import App from './app/layout/App';
import reportWebVitals from './reportWebVitals';
import { store, StoreContext } from './app/stores/store';
import { unstable_HistoryRouter as HistoryRouter } from 'react-router-dom';  // for react-router V6 use unstable_HistoryRouter
import { createBrowserHistory} from 'history';

// not import semaintic UI.min.css - cause error - using cdn link instead

export const history = createBrowserHistory();

ReactDOM.render(  // should remove strictmode of 3rd party app has error
  //<React.StrictMode>
  <StoreContext.Provider value={store}>
    <HistoryRouter history={history}>
      <App />
    </HistoryRouter>     
  </StoreContext.Provider>,
  //</React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
