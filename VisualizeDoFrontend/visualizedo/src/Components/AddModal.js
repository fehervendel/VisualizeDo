import React from "react";
import { useState } from "react";
import '../Pages/Boards/Boards.css';
import API_URL from "../config";


function AddModal(props) {
    const priorities = ["Urgent", "High", "Medium", "Low"];
    const sizes = ["X-Large", "Large", "Medium", "Small", "Tiny"];
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [priority, setPriority] = useState("");
    const [size, setSize] = useState("");
    const [warnings, setWarnings] = useState([]);
    const [isError, setIsError] = useState(false);

    const addCard = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/AddCard`, {

                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    //"Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    listId: props.listId,
                    title: title,
                    description: description,
                    priority: priority,
                    size: size
                }),
            })
            const data = response.json();
            console.log(data);
            props.fetchListByBoardId(props.boardId);
            props.toggleAddModal();

        } catch (error) {
            console.error("Error:", error);
        }
    };

    const handleSave = () => {
        let isAnyError = false;
        let warnings = [];
        if (title === "") {
            warnings.push("title")
            isAnyError = true;
            setIsError(true);
        }
        if (description === "") {
            warnings.push("description")
            isAnyError = true;
            setIsError(true);
        }
        if (priority === "") {
            warnings.push("priority")
            isAnyError = true;
            setIsError(true);
        }
        if (size === "") {
            warnings.push("size")
            isAnyError = true;
            setIsError(true);
        }
        setWarnings(warnings);
        if (!isAnyError) {
            addCard();
        }
    };

    return (<div className="modal-overlay">
        <div className="modal">
            <div className="modal-content">
                <h2>Add Card</h2>
                <h3>Title</h3>
                <input className='title-input' onChange={(e) => setTitle(e.target.value)} type="text" placeholder="Add your card title..." />
                <h3>Description</h3>
                <textarea className='description-input' onChange={(e) => setDescription(e.target.value)} type="text" placeholder="Add your description here..." />
                <div className="dropdown-inputs">
                    <div>
                        <h3>Priority</h3>
                        <select className='priority-input' onChange={(e) => setPriority(e.target.value)}>
                            <option disabled selected>Choose priority...</option>
                            {priorities.map((p) => (
                                <option key={p} value={p}>{p}</option>
                            ))}
                        </select>
                    </div>
                    <div>
                        <h3>Size</h3>
                        <select className="size-input" onChange={(e) => setSize(e.target.value)}>
                            <option disabled selected>Choose size...</option>
                            {sizes.map((s) => (
                                <option key={s} value={s}>{s}</option>
                            ))}
                        </select>
                    </div>
                </div>
                <button className="close-button" onClick={props.toggleAddModal}>Close</button>
            </div>
            <button onClick={handleSave} className="save-button">Save</button>
            {isError ? (<p className="save-warning">Please complete the following fields: {warnings.join(', ')}.</p>) : (null)}
        </div>
    </div>)
}

export default AddModal;