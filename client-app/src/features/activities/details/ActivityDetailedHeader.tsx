import { observer } from 'mobx-react-lite';
import React from 'react'
import { Link } from 'react-router-dom';
import {Button, Header, Item, Segment, Image, Label} from 'semantic-ui-react'
import {Activity} from "../../../app/models/activity";
import {format} from 'date-fns';  // to format Date object
import { useStore } from '../../../app/stores/store';

const activityImageStyle = {
    filter: 'brightness(30%)'
};

const activityImageTextStyle = {
    position: 'absolute',
    bottom: '5%',
    left: '5%',
    width: '100%',
    height: 'auto',
    color: 'white'
};

interface Props {
    activity: Activity
}

export default observer (function ActivityDetailedHeader({activity}: Props) {
    const {activityStore: {updateAttendance, loading, cancelActivityToggle}} = useStore();
    return (
        <Segment.Group>
            <Segment basic attached='top' style={{padding: '0'}}>
                {activity.isCancelled &&  // set a label to show the Cancelled state of activity if activity is cancelled
                    <Label style={{position: 'absolute', zIndex: 1000, left: -14, top: 20}} 
                           ribbon color='red' content='Cancelled' />
                }
                <Image src={`/assets/categoryImages/${activity.category}.jpg`} fluid style={activityImageStyle}/>
                <Segment style={activityImageTextStyle} basic>
                    <Item.Group>
                        <Item>
                            <Item.Content>
                                <Header  // show event date and name of the event host
                                    size='huge'
                                    content={activity.title}
                                    style={{color: 'white'}}
                                />
                                <p>{format(activity.date!, 'dd MMM yyyy')}</p>
                                <p>
                                    Hosted by <strong><Link to={`/profiles/${activity.host?.username}`}>{activity.host?.displayName}</Link></strong>
                                </p>
                            </Item.Content>
                        </Item>
                    </Item.Group>
                </Segment>
            </Segment>
            <Segment clearing attached='bottom' >   
                {activity.isHost ? ( // if current user is host of the event then they can manage the event
                    <>  <Button   // either reactivate the activity or cancel the activity
                            color={activity.isCancelled ? 'green' : 'red'}
                            floated='left'
                            basic
                            content={activity.isCancelled ? 'Re-activate Activity': 'Cancel Activity'}
                            onClick={cancelActivityToggle}
                            loading={loading}
                        />

                        <Button   // manage the activity (update activity)
                            disabled={activity.isCancelled}  //  can not update if activity is already cancelled
                            as={Link} 
                            to={`/manage/${activity.id}`} 
                            color='orange' 
                            floated='right'>
                            Manage Event
                        </Button>
                    </>                    
                ) : activity.isGoing ? (  // if current user is going to this event then they can cancel to join this event
                    <Button 
                        loading={loading} 
                        onClick={updateAttendance}>
                            Cancel attendance
                    </Button>
                ) : (  // else they can choose to join the event
                    <Button 
                        disabled={activity.isCancelled}   // cancelled event can not be joint
                        loading={loading} 
                        onClick={updateAttendance} 
                        color='teal'>
                            Join Activity
                    </Button>
                )}                                          
            </Segment>
        </Segment.Group>
    )
})

