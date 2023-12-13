import React from "react";
import { useState } from "react";
import '../Pages/Menu/Menu.css';
import API_URL from "../Pages/config";


function Modal(props) {
    const priorities = ["Urgent", "High", "Medium", "Low"];
    const sizes = ["Tiny", "Small", "Medium", "Large", "X-Large"];
    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [priority, setPriority] = useState("");
    const [size, setSize] = useState("");

    const addCard = async () => {
        console.log(description);
        console.log(title);
        console.log(size);
        console.log(priority);
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
            }).then((res) => res.json())
                .then((data) => {
                    console.log(data);
                    props.fetchListByBoardId(props.boardId);
                    props.toggleModal();
                })
        } catch (error) {
            console.error("Error:", error);
        }
    };

    return (<div className="modal-overlay">
        <div className="modal">
            <div className="modal-content">
                <h2>Add Card</h2>
                <h3>Title</h3>
                <input onChange={(e) => setTitle(e.target.value)} type="text" placeholder="Card Title" />
                <h3>Description</h3>
                <input onChange={(e) => setDescription(e.target.value)} type="text" placeholder="Description" />
                <h3>Priority</h3>
                <select onChange={(e) => setPriority(e.target.value)}>
                    <option disabled selected>Choose priority...</option>
                    {priorities.map((p) => (
                        <option key={p} value={p}>{p}</option>
                    ))}
                </select>
                <h3>Size</h3>
                <select onChange={(e) => setSize(e.target.value)}>
                    <option disabled selected>Choose size...</option>
                    {sizes.map((s) => (
                        <option key={s} value={s}>{s}</option>
                    ))}
                </select>
                <button className="close-button" onClick={props.toggleModal}>Close</button>
            </div>
            <button onClick={addCard} className="save-button">Save</button>
        </div>
    </div>)
}

export default Modal;