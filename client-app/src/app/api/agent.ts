import axios, { AxiosError, AxiosRequestConfig, AxiosRequestHeaders, AxiosResponse } from "axios";
import { toast } from "react-toastify";  // show error message as a flash on bottom of page
import { history } from "../..";
import { Activity } from "../models/activity";
import { User, UserFormValues } from "../models/user";
import { store } from "../stores/store";

// provide a fake delay time for request
const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay)
  })
}

axios.defaults.baseURL = 'http://localhost:5000/api';

// intercept the user request, check if store has token,
// if has token then add the Bearer + token to request header
axios.interceptors.request.use((config: AxiosRequestConfig) => {
  const token = store.commonStore.token;
  if (token) config.headers ? config.headers.Authorization = `Bearer ${token}` : console.log('config blank')
  return config;
})

// apply 1 section delay for each response
axios.interceptors.response.use(async response => 
  // the resolve part
{  
  await sleep(1000);
  return response; 
}, 
  // the reject part
  (error: AxiosError) => {
  const {data, status, config} = error.response!;  // add ! to remove type check

  switch (status) {
    case 400:
      // bad request
      if (typeof data === 'string') {
        toast.error(data);  
      }
      // bad gui error
      if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
        history.push('/not-found');  // redirect to not-found page
      }
      // validation error
      if (data.errors) {
        const modelStateErros = [];
        for (const key in data.errors) {
          if (data.errors[key]) {
            modelStateErros.push(data.errors[key])
          }
        }
        throw modelStateErros.flat();
      } 
      break;
    case 401:
      toast.error('unauthorized'); // show toast error of unauthorized
      break;
    case 404:
      history.push('/not-found');  // redirect to notfound page
      break;
    case 500:
      store.commonStore.setServerError(data);  // change the error state in commonStore to data
      history.push('/server-error');  // redirect to server-error page (get rendered by the error state above)
      break;      
  }
  return Promise.reject(error);
})

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

// create a requests object which can do get, post, put and delete using axios
const requests = {
  get: <T> (url: string) => axios.get<T>(url).then(responseBody),
  post: <T> (url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
  put: <T> (url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  delete: <T> (url: string) => axios.delete<T>(url).then(responseBody),
}

// create Activities object which use request object to obtain list of act, 
// detail of act, update and del act by our API end points
const Activities = {
  list: () => requests.get<Activity[]>('/activities'),
  details: (id: string) => requests.get<Activity>(`/activities/${id}`),
  create: (activity: Activity) => axios.post<void>('/activities', activity),
  update: (activity: Activity) => axios.put<void>(`/activities/${activity.id}`, activity),
  delete: (id: string) => axios.delete<void>(`/activities/${id}`)
}

// Account object, use requests object and our API end point to 
// get current user, login and register new user
const Account = {
  current: () => requests.get<User>('/account'),
  login: (user: UserFormValues) => requests.post<User>('/account/login', user),
  register: (user: UserFormValues) => requests.post<User>('/account/register', user)  
}

// wrap all in the agent object
const agent = {
  Activities,
  Account
}

export default agent;