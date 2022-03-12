import { observer } from "mobx-react-lite";
import React from "react";
import { Modal } from "semantic-ui-react";
import { useStore } from "../../stores/store";

export default observer(function ModalContainer() {
  // get the modalStore
  const {modalStore} = useStore();

  return (
    // show a modal element in semantic-ui-react with content is the modalStore.modal.body
    <Modal open={modalStore.modal.open} onClose={modalStore.closeModal} size='mini'>
      <Modal.Content>
        {modalStore.modal.body}
      </Modal.Content>
    </Modal>
  )
})