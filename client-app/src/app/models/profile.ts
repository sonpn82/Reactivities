import { User } from "./user";

export interface Profile {  // all fields must have exact same name with backend field name in database
  username: string;
  displayName: string;  // same with prop in AppUser table
  image?: string;
  bio?: string;  // same with prop in AppUser table
}

// this class create an object that contains some prop of User interface - not have token prop
export class Profile implements Profile {
  constructor(user: User) {
    this.username = user.username;
    this.displayName = user.displayName;
    this.image = user.image;
  }
}