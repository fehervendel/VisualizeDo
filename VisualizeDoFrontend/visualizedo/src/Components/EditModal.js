import React from "react";
import { useState } from "react";
import API_URL from "../Pages/config";
import '../Pages/Menu/Menu.css';

function EditModal(props) {
    const [confirmationModal, setConfirmationModal] = useState(false);
    

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
           {/*  <h3>Title</h3>
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
            </select>*/}
            <button className="close-button" onClick={props.toggleEditModal}>Close</button> 
        </div>
        <button className="delete-button" onClick={toggleConfirmationModal}>Delete card</button>
        <button onClick className="save-button">Save</button>
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