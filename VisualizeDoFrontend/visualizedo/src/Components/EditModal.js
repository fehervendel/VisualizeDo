import React from "react";
import { useState } from "react";
import API_URL from "../Pages/config";
import '../Pages/Menu/Menu.css';

function EditModal(props) {
    const priorities = ["Urgent", "High", "Medium", "Low"];
    const sizes = ["X-Large", "Large", "Medium", "Small", "Tiny"];
    const [confirmationModal, setConfirmationModal] = useState(false);
    const [title, setTitle] = useState(props.card.title);
    const [description, setDescription] = useState(props.card.description);
    const [priority, setPriority] = useState(props.card.priority);
    const [size, setSize] = useState(props.card.size);
    
    const handleSave = async () => {
      try {
          const response = await fetch(`${API_URL}/VisualizeDo/EditCard`, {

              method: "PUT",
              headers: {
                  "Content-Type": "application/json",
                  //"Authorization": `Bearer ${token}`
              },
              body: JSON.stringify({
                  id: props.card.id,
                  title: title,
                  description: description,
                  priority: priority,
                  size: size
              }),
          })
          if (!response.ok) {
            throw new Error('Error deleting card');
          } else {
            props.fetchListByBoardId(props.boardId);
            toggleConfirmationModal();
            props.toggleEditModal();
          }
      } catch (error) {
          console.error("Error:", error);
      }
  };

    const deleteCardById = async () => {
        try {
          const response = await fetch(`${API_URL}/VisualizeDo/DeleteCardById?id=${props.card.id}`, {
            method: 'DELETE',
          });
          if (!response.ok) {
            throw new Error('Error deleting card');
          } else {
            props.fetchListByBoardId(props.boardId);
            toggleConfirmationModal();
            props.toggleEditModal();
          }
        } catch (error) {
          console.error('Error deleting card', error);
        }
      };

      const toggleConfirmationModal = () => {
        setConfirmationModal(!confirmationModal);
    }

    return (<div className="modal-overlay">
    <div className="modal">
        <div className="modal-content">
            <h2>Edit your card here!</h2>
            <h3>Title</h3>
            <input className="title-input" onChange={(e) => setTitle(e.target.value)} type="text" value={title}/>
            <h3>Description</h3>
            <textarea className='description-input' onChange={(e) => setDescription(e.target.value)} type="text" value={description}/>
            <div className="dropdown-inputs">
                <div>
                <h3>Priority</h3>
                <select className='priority-input' value={priority} onChange={(e) => setPriority(e.target.value)}>
                    <option disabled selected>Choose priority...</option>
                    {priorities.map((p) => (
                        <option key={p} value={p}>{p}</option>
                    ))}
                </select>
                </div>
                <div>
                <h3>Size</h3>
                <select className="size-input" value={size} onChange={(e) => setSize(e.target.value)}>
                    <option disabled selected>Choose size...</option>
                    {sizes.map((s) => (
                        <option key={s} value={s}>{s}</option>
                    ))}
                </select>
                </div>
                </div>
            <button className="close-button" onClick={props.toggleEditModal}>Close</button> 
        </div>
        <button className="delete-button" onClick={toggleConfirmationModal}>Delete card</button>
        <button className="save-button" onClick={handleSave}>Save</button>
    </div>
    {confirmationModal && (<div className="confirmation-modal-overlay">
    <div className="confirmation-modal">
        <h2>Are you sure?</h2>
        <div>
        <button onClick={deleteCardById}>Delete</button>
        <button onClick={(e) => {e.preventDefault(); setConfirmationModal(false)}}>Cancel</button>
        </div>
        </div>
        </div>)}
</div>)
}

export default EditModal;