import React from "react";
import "./Create.css"
import { useState } from 'react';
import API_URL from "../../config";
import Cookies from "js-cookie";
import { Navigate, useNavigate } from "react-router-dom";

function Create() {
    const [isCreating, setIsCreating] = useState(false);
    const [boardName, setBoardName] = useState("");
    const userEmail = Cookies.get("userEmail");
    const [numberOfLists, setNumberOfLists] = useState(1);
    const [step, setStep] = useState(1);
    const [isBoardNameOk, setIsBoardNameOk] = useState(true);
    const [isListNamesOk, setIsListNamesOk] = useState(true);
    const [listNames, setListNames] = useState([]);
    const [cardTitle, setCardTitle] = useState("");
    const [cardDescription, setCardDescription] = useState("");
    const priorities = ["Urgent", "High", "Medium", "Low"];
    const sizes = ["X-Large", "Large", "Medium", "Small", "Tiny"];
    const [priority, setPriority] = useState("Choose priority...");
    const [size, setSize] = useState("Choose size...");
    const [isCardInputOk, setIsCardInputOk] = useState(true);
    const [isSelectOk, setIsSelectOk] = useState(true);
    const navigate = useNavigate();

    function toggleCreateClick() {
        setIsCreating(!isCreating);
    }

    const addCard = async (listId) => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/AddCard`, {

                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    //"Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    listId: listId,
                    title: cardTitle,
                    description: cardDescription,
                    priority: priority,
                    size: size
                }),
            })
            const data = response.json();
            navigate('/Boards');
        } catch (error) {
            console.error("Error:", error);
        }
    };

    const addLists = async (id) => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/AddLists?boardId=${id}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(listNames),
            });
            const data = await response.json();
            console.log(data);
            const listId = data[0].id;
            addCard(listId);
        } catch (error) {
            console.error("Error:", error);
        }
    }

    const addBoard = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/AddBoard?email=${userEmail}&name=${boardName}`, {

                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    //"Authorization": `Bearer ${token}`
                },
            })
            const data = await response.json();
            console.log(data);
            let boardId = data.id;
            addLists(boardId);
            setBoardName("");

        } catch (error) {
            console.error("Error:", error);
        }
    };
    //https://localhost:7225/VisualizeDo/AddLists?boardId=112323


    const handleSave = () => {
        let titleAndDescriptionOk = true;
        let selectOk = true;
        if (cardTitle.length < 3 || cardDescription.length < 3) {
            setIsCardInputOk(false);
            titleAndDescriptionOk = false;
        } else {
            setIsCardInputOk(true);
            titleAndDescriptionOk = true;
        }
        if (priority === "Choose priority..." || size === "Choose size...") {
            setIsSelectOk(false);
            selectOk = false;
        } else {
            setIsSelectOk(true);
            selectOk = true;
        }
        if (selectOk && titleAndDescriptionOk) {
            addBoard();
        }
    }

    const handleCancel = () => {
        toggleCreateClick();
    }

    const handleNext = () => {
        if (step === 1) {
            if (boardName.length > 2) {
                setStep(step + 1);
                setIsBoardNameOk(true);
            } else {
                setIsBoardNameOk(false);
            }
        } else if (step === 2) {
            let areAllListNamesOk = true;
            if (listNames.length < numberOfLists) {
                areAllListNamesOk = false;
                setIsListNamesOk(false);
            }

            listNames.forEach(name => {
                if (name.length < 3) {
                    areAllListNamesOk = false;
                    setIsListNamesOk(false);
                }
            });

            if (areAllListNamesOk === true) {
                setStep(step + 1);
                setIsListNamesOk(true);
            }
        }
    }

    const handleBack = () => {
        if (step > 1) {
            setStep(step - 1);
        }
    }

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
                            <select className="select" value={numberOfLists} onChange={(e) => setNumberOfLists(e.target.value)}>
                                <option>1</option>
                                <option>2</option>
                                <option>3</option>
                            </select>
                            <p className="note">Add lists to keep track of progression. You can also use them as notes for your plans. For example: backlog, in review, done, etc.(You can add more later)</p>
                            {Array.from(Array(parseInt(numberOfLists))).map((num, index) => (
                                <div>
                                    <h3 className="instruction">Add your {index + 1}. list name here</h3>
                                    <input className="input" minLength={3} maxLength={22} key={index} type="text" value={listNames[index]} onChange={(e) => {
                                        const newListNames = [...listNames]
                                        newListNames[index] = e.target.value;
                                        setListNames(newListNames);
                                    }}></input>
                                </div>
                            ))}
                            {isListNamesOk ? (null) : (<p className="warning">List names must be 3-22 characters long!</p>)}
                        </div>) : (null)}
                        {step === 3 ? (<div>
                            <h2 className="instruction">Add a card to your {listNames[0]} list</h2>
                            <p className="note">You can add more cards to any list later</p>
                            <h3 className="instruction">Add a title</h3>
                            <input type="text" className="input" value={cardTitle} onChange={(e) => setCardTitle(e.target.value)} maxLength={21}></input> {/**CAAAAAAAAAAAAAAAAAAARD */}
                            <h3 className="instruction">Add a short description</h3>
                            <input type="text" className="input" value={cardDescription} onChange={(e) => setCardDescription(e.target.value)} maxLength={52}></input>
                            {isCardInputOk ? (null) : (<p className="warning">Both title and description must be at least 3 characters long!</p>)}
                            <h3 className="instruction">Set Priority</h3>
                            <select className='priority-input' value={priority} onChange={(e) => setPriority(e.target.value)}>
                                <option disabled selected>Choose priority...</option>
                                {priorities.map((p) => (
                                    <option key={p} value={p}>{p}</option>
                                ))}
                            </select>
                            <h3 className="instruction">Set size</h3>
                            <select className="size-input" value={size} onChange={(e) => setSize(e.target.value)}>
                                <option disabled selected>Choose size...</option>
                                {sizes.map((s) => (
                                    <option key={s} value={s}>{s}</option>
                                ))}
                            </select>
                            {isSelectOk ? (null) : (<p className="warning">Choose priority and size!</p>)}
                        </div>) : (null)}
                    </div>
                    <button className="back" onClick={handleBack}>Back</button>
                    {step === 3 ? (<button className="next" id="save" onClick={handleSave}>Save</button>) : (<button className="next" onClick={handleNext}>Next</button>)}

                    <button className="cancel" onClick={handleCancel}>Cancel</button>
                </div>
                <div className="create-space"></div>
                <div className="create-preview">
                    <h2 className="board-name" placeholder="asd">{boardName}</h2>
                    {step > 1 ? (<div className="preview-all-list-container">
                        {Array.from(Array(parseInt(numberOfLists))).map((num, index) => (
                            <div key={index} className="preview-list-container">
                                <div className="preview-list-head">
                                    <h4>{listNames[index]}</h4>
                                </div>
                                <div className="preview-cards-container">
                                    {step > 2 ? (<div>{index === 0 ? (<div className="preview-card">
                                        <div className="card-text">Title: {cardTitle}</div>
                                        <div className="card-text">Description: {cardDescription}</div>
                                        <div className="card-text">Priority: {priority}</div>
                                        <div className="card-text">Size: {size}</div>
                                    </div>) : (null)}</div>) : (null)}
                                </div>
                            </div>
                        ))}
                    </div>) : (null)}
                </div>
            </div>) : (
            <button className="create-button" onClick={toggleCreateClick}>New board</button>)}
    </div>)
}

export default Create;