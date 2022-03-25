import { makeAutoObservable, runInAction } from "mobx";
import agent from "../api/agent";
import { Photo, Profile } from "../models/profile";
import { store } from "./store";

// contain 2 states about profile: the user profile info and the loadingProfile state
export default class ProfileStore {
  profile: Profile | null = null;
  loadingProfile = false;
  uploading = false;
  loading = false;

  constructor() {
    makeAutoObservable(this);
  }

  // check if this profile belong to current user
  get isCurrentUser() {
    if (store.userStore.user && this.profile) {
      return store.userStore.user.username === this.profile.username;
    }
    return false;
  }

  // get the user profile from backend and set the profile state to that value
  // set the loading status from true to false
  loadProfile = async (username: string) => {
    this.loadingProfile = true;
    try {
      const profile = await agent.Profiles.get(username);
      runInAction(() => {
        this.profile = profile;
        this.loadingProfile = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => this.loadingProfile = false);
    }
  }

  uploadPhoto =async (file:Blob) => {
    this.uploading = true;
    try {
      const response = await agent.Profiles.uploadPhoto(file);
      const photo = response.data;
      runInAction(() => {
        if (this.profile) {
          this.profile.photos?.push(photo);         
          if (photo.isMain && store.userStore.user) {
             // set this photo to user image if it is the main photo
            store.userStore.setImage(photo.url);              
            this.profile.image = photo.url;
          }
        }
        this.uploading = false;
      })
    } catch (error) {
      console.log(error)
      runInAction(() => this.uploading = false);
    }
  }

  // set a photo to be main photo using agent setMainPhoto function
  // set the current main photo to be not main photo
  // set the profile image to photo url
  // set the loading state from true to false
  setMainPhoto =async (photo:Photo) => {
    this.loading = true;
    try {
      await agent.Profiles.setMainPhoto(photo.id);
      store.userStore.setImage(photo.url);
      runInAction(() => {
        if (this.profile && this.profile.photos) {
          this.profile.photos.find(p => p.isMain)!.isMain = false;
          this.profile.photos.find(p=>p.id === photo.id)!.isMain = true;
          this.profile.image = photo.url;
          this.loading = false;
        }
      })
    } catch (error) {
      runInAction(() => this.loading = false);
      console.log(error);
    }
  }

  deletePhoto =async (photo:Photo) => {
    this.loading = true
    try {
      await agent.Profiles.deletePhoto(photo.id);
      runInAction(() => {
        if (this.profile) {
          this.profile.photos = this.profile.photos?.filter(p => p.id !== photo.id);
          this.loading = false;
        }
      })
    } catch (error) {
      runInAction(() => this.loading = false);
      console.log(error);
    }
  }
}