import { makeAutoObservable } from "mobx"

interface Modal {
  open: boolean;
  body: JSX.Element | null;
}

// for log in part
export default class ModalStore {
  // A modal displays content that temporarily blocks interactions 
  // with the main view of a site - same as a messagebox
  modal: Modal = {
    open: false,  // initial value of modal
    body: null
  }

  constructor() {
    makeAutoObservable(this)
  }

  // set modal state at openModal
  openModal = (content: JSX.Element) => {
    this.modal.open = true;
    this.modal.body = content;
  }

  // set modal state at closeModal
  closeModal = () => {
    this.modal.open = false;
    this.modal.body = null;
  }
}