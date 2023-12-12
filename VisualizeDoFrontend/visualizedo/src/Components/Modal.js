import React from "react";
import { useState } from "react";
import '../Pages/Menu/Menu.css';


function Modal(props) {
    const priorities = ["Urgent", "High", "Medium", "Low"];
    const sizes = ["Tiny", "Small", "Medium", "Large", "X-Large"];
    const [priority, setPriority] = useState(null);
    const [ size, setSize ] = useState(null);
    console.log(props.listId);
 
    return (<div className="modal-overlay">
    <div className="modal">
        <div className="modal-content">
            <h2>Add Card</h2>
            <h3>Title</h3>
            <input type="text" placeholder="Card Title" />
            <h3>Description</h3>
            <input type="text" placeholder="Description" />
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
        <button className="save-button">Save</button>
    </div>
</div>)
}

export default Modal;