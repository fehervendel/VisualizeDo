import React from "react";
import "./Create.css"
import {useState} from 'react';

function Create() {
    const [isCreating, setIsCreating] = useState(false);
    const [boardName, setBoardName] = useState("");

    function toggleCreateClick() {
        setIsCreating(!isCreating);
    }
//https://localhost:7225/VisualizeDo/AddBoard?userId=01010&name=yxcyxcyx
    return (<div>
        {isCreating ? (<div className="create-container">
        <div className="create-inputs">
            <div className="inputs">
            <h2 className="instruction">Enter your board's name</h2>
            <input type="text" onChange={(e) => setBoardName(e.target.value)} placeholder="Board name"></input>
            </div>
            <button className="save">Save</button>
        </div>
        <div className="create-space"></div>
        <div className="create-preview"></div>
        </div>) : (<button className="create-button" onClick={toggleCreateClick}>New board</button>)}
    </div>)
}

export default Create;