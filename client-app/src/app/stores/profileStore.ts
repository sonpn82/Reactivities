import { makeAutoObservable, reaction, runInAction } from "mobx";
import agent from "../api/agent";
import { Photo, Profile } from "../models/profile";
import { store } from "./store";

// contain 2 states about profile: the user profile info and the loadingProfile state
export default class ProfileStore {
  profile: Profile | null = null;
  loadingProfile = false;
  uploading = false;
  loading = false;
  loadingFollowings = false;
  followings: Profile[] = [];
  activeTab = 0;  // to know thich tab pane is selected in  user profile - for follower and following

  constructor() {
    makeAutoObservable(this);

    // react to a change in some state - here is activeTab state
    reaction(
      () => this.activeTab,
      activeTab => {
        if (activeTab === 3 || this.activeTab === 4) {
          const predicate = activeTab === 3 ? 'followers' : 'following';
          this.loadFollowings(predicate);          
        } else {
          this.followings = [];
        }
      }
    )
  }

  // change the activeTab state
  setActiveTab = (activeTab: any) => {
    this.activeTab = activeTab;
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

  // update profile with a partial profile (not contain all fields of profile, only displayName and Bio)
  // change the profile state. Change loading state from true to false
  updateProfile =async (profile:Partial<Profile>) => {
    this.loading = true;
    try {
      // update profile using API end point
      await agent.Profiles.updateProfile(profile);
      runInAction(() => {
        // if profile displayName is Changed then update the state in store
        if (profile.displayName && profile.displayName !== store.userStore.user?.displayName) {
          store.userStore.setDisplayName(profile.displayName);
        }
        // use spread operator to overwrite all duplicated field between input profile and our profile
        this.profile = {...this.profile, ...profile as Profile};
        this.loading = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => this.loading = false);
    }
  }

  // update the follower count and following status in profile
  // update Attendee following in activity
  updateFollowing =async (username:string, following: boolean) => {
    this.loading = true;
    try {
      await agent.Profiles.updateFollowing(username);
      store.activityStore.updateAttendeeFollowing(username);
      runInAction(() => {
        if (this.profile && 
            this.profile.username !== store.userStore.user?.username && 
            this.profile.username === username) {
          following ? this.profile.followersCount++ : this.profile.followersCount--;
          this.profile.following = !this.profile.following;
        }
        if (this.profile && this.profile.username === store.userStore.user?.username) {
          following ? this.profile.followingCount++ : this.profile.followingCount--;
        }
        this.followings.forEach(profile => {
          if (profile.username === username) {
            profile.following ? profile.followersCount-- : profile.followersCount++;
            profile.following = !profile.following;
          }
        })
        this.loading = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => this.loading = false)
    }
  }

  loadFollowings =async (predicate:string) => {
    this.loadingFollowings = true;
    try {
      const followings = await agent.Profiles.listFollowings(this.profile!.username, predicate);
      runInAction(() => {
        this.followings = followings;
        this.loadingFollowings = false;
      })
    } catch (error) {
      console.log(error);
      runInAction(() => this.loadingFollowings = false);
    }
  }
}