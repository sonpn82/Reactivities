import { observer } from "mobx-react-lite";
import React, { SyntheticEvent, useState } from "react";
import { Button, Card, Grid, Header, Image, Tab } from "semantic-ui-react";
import PhotoUploadWidget from "../../app/common/imageUpload/PhotoUploadWidget";
import { Photo, Profile } from "../../app/models/profile";
import { useStore } from "../../app/stores/store";

interface Props {
  profile: Profile;
}

export default observer(function ProfilePhotos({profile}: Props) {
  const {profileStore: {isCurrentUser, uploadPhoto, uploading, 
                        loading, setMainPhoto, deletePhoto}} = useStore();
  const [addPhotoMode, setAddPhotoMode] = useState(false);
  const [target, setTarget] = useState('');

  function handlePhotoUpload(file: Blob) {
    uploadPhoto(file).then(() => setAddPhotoMode(false));
  }

  // only show loading symbol on the button which is clicked by set the target of event here
  function handleSetMainPhoto(photo: Photo, e: SyntheticEvent<HTMLButtonElement>) {
    setTarget(e.currentTarget.name);
    setMainPhoto(photo);
  }

  // handle the delete button click - setTarget to allow only clicked button to be in loading state
  function handleDeletePhoto(photo: Photo, e: SyntheticEvent<HTMLButtonElement>) {
    setTarget(e.currentTarget.name);
    deletePhoto(photo);
  }

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16}>
          <Header floated="left" icon='image' content='Photos' />
          {isCurrentUser && (
            <Button floated="right" basic 
              content={addPhotoMode ? 'Cancel' : 'Add Photo'} 
              onClick={() => setAddPhotoMode(!addPhotoMode)}  
            />
          )}
        </Grid.Column>
        <Grid.Column width={16}>
          {addPhotoMode ? (
            <PhotoUploadWidget uploadPhoto={handlePhotoUpload} loading={uploading}/>
          ) : (
            <Card.Group itemsPerRow={5}>
              {profile.photos?.map(photo => (
                <Card key={photo.id}>
                  <Image src={photo.url} />
                  {isCurrentUser && !photo.isMain &&
                    (
                    <Button.Group fluid widths={2}>
                      <Button
                        basic
                        color="green"
                        content='Main'
                        name={'main' + photo.id}  // add main to photo.id to separate setMain button and del button
                        disabled={photo.isMain}
                        loading={target === 'main' + photo.id && loading}  // only clicked button shows loading state
                        onClick={e => handleSetMainPhoto(photo, e)}
                      />
                      <Button 
                        basic 
                        color="red" 
                        icon='trash' 
                        name={photo.id}  // target will be set = name of clicked button, so we name this button and above button differently 
                        disabled={photo.isMain}
                        loading={target === photo.id && loading}
                        onClick={e => handleDeletePhoto(photo, e)}
                      />
                    </Button.Group>
                  )}
                </Card>
        ))}         
      </Card.Group>
          )}
        </Grid.Column>
      </Grid>
      
      
    </Tab.Pane>
  )
})