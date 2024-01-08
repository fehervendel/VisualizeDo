import React from "react";
import useModal from "../Hooks/useModal";

function Modal({children, setListId}) {
   const {toggleModal, show, setShow} = useModal();

    return(
        <>
        <button className="add-button" onClick={() => { toggleModal(); setListId(); }}>Add card</button>
        {show && React.cloneElement(children, {toggleAddModal: () => setShow(!show)})}
        </>
    )
}

export default Modal;