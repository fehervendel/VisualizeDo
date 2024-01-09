import React from "react";
import "./Create.css"
import { useState } from 'react';
import API_URL from "../config";
import Cookies from "js-cookie";

function Create() {
    const [isCreating, setIsCreating] = useState(false);
    const [boardName, setBoardName] = useState("");
    const userEmail = Cookies.get("userEmail");
    const [numberOfLists, setNumberOfLists] = useState(1);
    const [step, setStep] = useState(1);
    const [isBoardNameOk, setIsBoardNameOk] = useState(true);

    function toggleCreateClick() {
        setIsCreating(!isCreating);
    }

    const addBoard = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/AddBoard?email=${userEmail}&name=${boardName}`, {

                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    //"Authorization": `Bearer ${token}`
                },
            }).then((res) => res.json())
                .then((data) => {
                    console.log(data);
                    setBoardName("");
                })
        } catch (error) {
            console.error("Error:", error);
        }
    };

    const handleSave = () => {
        addBoard();
        toggleCreateClick();
    }

    const handleCancel = () => {
        toggleCreateClick();
    }

    const handleNext = () => {
        if (boardName.length > 2) {
            if (step < 3) {
                setStep(step + 1);
                setIsBoardNameOk(true);
            }
        } else {
            setIsBoardNameOk(false);
        }
    }

    const handleBack = () => {
        if (step > 1) {
            setStep(step - 1);
        }
    }

const numbers = [1,2,3];

    return (<div className="main-div">
        {isCreating ? (
            <div className="create-container">
                <div className="create-inputs">
                    <div className="inputs">
                        {step === 1 ? (<div><h2 className="instruction">Enter your board's name</h2>
                            <input className="input" type="text" onChange={(e) => setBoardName(e.target.value)} value={boardName} placeholder="Board name" maxLength={22}></input>
                            {isBoardNameOk ? (null) : (<p className="warning">Board name must be 3-22 characters long!</p>)}
                            <p className="note">This will be the related area to your todos. For example: school, home, work, etc. Add something that can easily make this board separated from the others.</p>
                        </div>) : (null)}
                        {step === 2 ? (<div><h2 className="instruction">Add your lists</h2>
                            <select className="select" onChange={(e) => setNumberOfLists(e.target.value)}>
                                <option>1</option>
                                <option>2</option>
                                <option>3</option>
                            </select>
                            <p className="note">Add lists to keep track of progression. You can also use them as notes for your plans.(You can add more later)</p>
                            
                            {Array.from(Array(parseInt(numberOfLists))).map((num, index) => (
                                
                                <div>
                                    {console.log(index)}
                                <h3 className="instruction">Add your {index + 1}. list name here</h3>
                                <input className="input" maxLength={22} key={index} type="text"></input>
                                </div>
                            ))}
                            </div>) : (null)}
                            {step === 3 ? (<div>
                                <h2 className="instruction">Add some cards to your lists</h2>
                            </div>) : (null)}
                    </div>
                    <button className="back" onClick={handleBack}>Back</button>
                    <button className="next" onClick={handleNext}>Next</button>
                    <button className="cancel" onClick={handleCancel}>Cancel</button>
                </div>
                <div className="create-space"></div>
                <div className="create-preview"></div>
            </div>) : (
            <button className="create-button" onClick={toggleCreateClick}>New board</button>)}
    </div>)
}

export default Create;