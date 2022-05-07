import React from "react";
import { v1 as uuid } from "uuid";
import { useNavigate } from 'react-router-dom';


const CreateRoom = (props) => {
    
    const history = useNavigate();

    function create() {
        const id = uuid(); 
        history(`/room/55555asca`);
    }

    return (
        <button onClick={create}>Create Room</button>
    );
}

export default CreateRoom;