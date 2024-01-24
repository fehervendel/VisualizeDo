import Cookies from "js-cookie";
import React, { useState, useEffect } from "react";
import "./Boards.css";
import API_URL from "../config";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";
import AddModal from "../../Components/AddModal";
import EditModal from "../../Components/EditModal";
import useModal from "../../Hooks/useModal";
import Modal from "../../Components/Modal";
import { faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { getUserByEmail } from "../../Services/User.service";
import { useContext } from "react";
import { ToastContext } from "../../Contexts/ToastContext.context";

function Menu() {
    const token = Cookies.get("userToken");
    const userEmail = Cookies.get("userEmail");
    const [boards, setBoards] = useState(null);
    const [lists, setLists] = useState(null);
    const [selectedBoard, setSelectedBoard] = useState(null);
    const [listId, setListId] = useState(null);
    const [boardId, setBoardId] = useState(null);
    const [card, setCard] = useState(null);
    const { toggleModal: toggleEditModal, show: showEditModal } = useModal();
    const [addListClicked, setAddListClicked] = useState(false);
    const [newListName, setNewListName] = useState("");
    const [listNameWarning, setListNameWarning] = useState(false);
    const [confirmationModal, setConfirmationModal] = useState(false);
    const [deleteListId, setDeleteListId] = useState(null);
    const [boardNameEditClicked, setBoardNameEditClicked] = useState(false);
    const [newBoardName, setNewBoardName] = useState("");
    const [newBoardNameOk, setNewBoardNameOk] = useState(true);
    const [boardDeleteConfirmation, setBoardDeleteConfirmation] = useState(false);
    const [listNameEditId, setListNameEditId] = useState(null);
    const [newNameOfList, setNewNameOfList] = useState("");
    //console.log(selectedBoard);

    const { AddToast } = useContext(ToastContext)

    const fetchBoard = async () => {
        try {
            // const response = await fetch(`${API_URL}/VisualizeDo/GetUserByEmail?email=${userEmail}`, {
            //     method: "GET",
            //     headers: {
            //         //Authorization: `Bearer ${token}`
            //         'Content-Type': 'application/json',
            //         'Accept': 'application/json',
            //     },
            // });
            // const jsonData = await response.json();
            //console.log("userboards" + jsonData);
            const jsonData = await getUserByEmail()
            setBoards(jsonData.data.boards);
            const selected = jsonData.data.boards.find((board) => board.id == selectedBoard?.id);
            setSelectedBoard(selected);
        } catch (err) {
            console.error(err);
        }
    };

    const updateBoardName = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/ChangeBoardName?id=${boardId}&newName=${newBoardName}`, {
                method: "PUT"
            });
            const data = await response.json();
            console.log(data);
            setSelectedBoard(data);
            fetchBoard();
            toggleBoardEditModal();
        } catch (e) {
            console.error(e);
        }
    }

    const changeListName = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/ChangeListName?listId=${listNameEditId}&newName=${newNameOfList}`, {
                method: "PUT"
            });
            const data = await response.text();
            console.log(data);
            fetchListByBoardId(boardId);
            setListNameEditId(null);
        } catch (e) {
            console.error(e);
        }
    }

    const deleteBoard = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/DeleteBoardById?id=${selectedBoard.id}`, {
                method: "DELETE"
            });
            const data = await response.text();
            console.log(data);
            fetchBoard();
            toggleBoardEditModal();
            setBoardDeleteConfirmation(false);
        } catch (e) {
            console.error(e);
        }
    }

    useEffect(() => {
        fetchBoard();
    }, [userEmail])

    const addList = async () => {
        try {
          
            const response = await fetch(`${API_URL}/VisualizeDo/AddList?boardId=${boardId}&name=${newListName}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    //"Authorization": `Bearer ${token}`
                },
            })
            const data = await response.json();
            console.log(data);
            setNewListName("");
            await fetchListByBoardId(boardId);
         
            AddToast("Hurray, new item added!")
        } catch (err) {
            console.error(err);
        }
    }

    const handleDelete = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/DeleteListById?id=${deleteListId}`, {
                method: "DELETE"
            });
            const data = await response.text();
            await fetchListByBoardId(boardId);
            toggleConfirmationModal();
        } catch (err) {
            console.error(err.message);
        }
    }

    const fetchListByBoardId = async (id) => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/GetListsByBoardId?id=${id}`, {
                method: "GET",
                headers: {
                    //Authorization: `Bearer ${token}`
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                },
            });
            if (response.ok) {
                const jsonData = await response.json();
                setLists(jsonData);
            }

            //console.log("userboards" + jsonData);
        } catch (err) {
            console.error(err);
        }
    };

    const deepCopyLists = (lists) => {
        return lists.map((list) => {
            return {
                ...list,
                cards: list.cards.map((card) => ({ ...card })),
            };
        });
    };

    const changeCardListById = async (cardId, listId) => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/ChangeCardsListById?cardId=${cardId}&listId=${listId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
            //console.log("Successful request", response);
        } catch (err) {
            console.error(err);
        }
    };


    const handleBoardChange = (e) => {
        const boardId = e.target.value;
        setBoardId(boardId);
        //console.log(e.target.value);
        const selected = boards.find((board) => board.id == boardId);
        setSelectedBoard(selected);
        //selectedBoard.lists.map((list) => list.id)
        if (selected != undefined) {
            fetchListByBoardId(boardId);
        }
    }

    function handleDragEnd(result) {
        const { source, destination } = result;
        if (!destination) {
            return;
        }
        const cardId = parseInt(result.draggableId);
        const listId = parseInt(destination.droppableId);
        //card swap logic
        let tempLists = deepCopyLists(lists);
        let originalColumn = tempLists.find((list) => list.cards.find((card) => card.id == cardId)); // This is the column where we took a card from
        let movedCard = originalColumn.cards.find((card) => card.id == cardId); // This is the card we moved
        let updatedSourceColumn = originalColumn.cards.filter((card) => card.id != cardId); // These are the remaining cards of the column where we took a card from
        originalColumn.cards = updatedSourceColumn; // Changing the original column to the updated one

        let updatedDestinationColumn = tempLists.find((list) => list.id == listId); // This is the destination column
        updatedDestinationColumn.cards.splice(destination.index, 0, movedCard); // Here we just add the card to the destination column with the correct index

        setLists(tempLists);
        //card swap logic end

        changeCardListById(cardId, listId);
    }

    const toggleAddList = () => {
        setAddListClicked(!addListClicked);
    }

    const handleAddList = () => {
        if (newListName.length < 3) {
            setListNameWarning(true);
        } else {
            setListNameWarning(false);
            addList();
            toggleAddList();
        }
    }

    const toggleConfirmationModal = () => {
        setConfirmationModal(!confirmationModal);
    }

    const toggleBoardEditModal = () => {
        setBoardNameEditClicked(!boardNameEditClicked);
    }

    const handleBoardNameChange = () => {
        if (newBoardName.length < 3) {
            setNewBoardNameOk(false);
        } else {
            setNewBoardNameOk(true);
            updateBoardName();
        }
    }

    const handleBoardDelete = () => {
        deleteBoard();
    }

    const handleListNameChange = () => {
        if (newNameOfList.length < 3) {
            console.log("wrong new name of list");
        } else {
            changeListName();
        }
    }

    return (
        <div className="main-div">
            <select className='selectBar' onChange={(e) => handleBoardChange(e)}>
                <option>Choose one of your boards...</option>
                {boards &&
                    boards.map((board, index) => (
                        <option key={board.id} value={board.id}>
                            {board.name}
                        </option>
                    ))}
            </select>
            {(selectedBoard === null) || (selectedBoard === undefined) || (selectedBoard.lists.length < 1) ? (null) : (
                selectedBoard && (
                    <DragDropContext onDragEnd={handleDragEnd}>
                        <div className="board-div">
                            <div className="board-name-container">
                                <h3 className="board-name">{selectedBoard.name}</h3>
                                <button className="board-name-edit-button" onClick={() => toggleBoardEditModal()}><FontAwesomeIcon icon={faPencilAlt} /></button>
                            </div>
                            <button className="add-list-button" onClick={toggleAddList}>
                                Add List +
                            </button>
                            {addListClicked ? (<div className="add-list-input-container">
                                <h3 id="list-name">List name</h3>
                                <input type="text" className="input" maxLength={22} value={newListName} onChange={(e) => setNewListName(e.target.value)}></input>
                                <button className="add-button" onClick={handleAddList}>Add</button>
                                <button className="add-button" onClick={toggleAddList}>Cancel</button>
                                {listNameWarning ? (<p className="warning">List name must be 3-22 characters long!</p>) : (null)}
                            </div>) : (null)}
                            <div className="all-list-container">
                                {lists && lists.map((list, index) => (
                                    <div className="list-container" key={list.id}>
                                        <div className="list-head">
                                            {listNameEditId === list.id ? (<input type="text" className="list-name-edit-input" value={newNameOfList} onChange={(e) => setNewNameOfList(e.target.value)} maxLength={22}></input>) : (<h4>{list.name}</h4>
                                            )}
                                            {listNameEditId === list.id ? (<button className="add-button" id="list-name-save" onClick={handleListNameChange}>Save</button>) : (<button className="list-name-edit-button" onClick={(e) => { e.preventDefault(); setListNameEditId(list.id); setNewNameOfList(list.name) }}><FontAwesomeIcon icon={faPencilAlt} /></button>)}

                                            <Modal
                                                setListId={() => setListId(list.id)}
                                            >
                                                <AddModal
                                                    listId={listId}
                                                    boardId={boardId}
                                                    fetchListByBoardId={fetchListByBoardId}
                                                />
                                            </Modal>
                                            <button className="list-delete-button" onClick={() => { toggleConfirmationModal(); setDeleteListId(list.id); }}><FontAwesomeIcon icon={faTrash} /></button>
                                        </div>
                                        <Droppable droppableId={list.id.toString()}>
                                            {(provided) => (
                                                <div ref={provided.innerRef} {...provided.droppableProps} className="cards-container">
                                                    {list.cards.map((card, index) => (
                                                        <Draggable key={card.id} draggableId={card.id.toString()} index={index}>
                                                            {(provided) => (
                                                                <div
                                                                    {...provided.draggableProps}
                                                                    {...provided.dragHandleProps}
                                                                    ref={provided.innerRef}
                                                                    className="card"
                                                                >
                                                                    <div>Title: {card.title}</div>
                                                                    <button className="options-button" onClick={() => { toggleEditModal(); setCard(card) }}>...</button>
                                                                    <div>Description: {card.description}</div>
                                                                    <div>Priority: {card.priority}</div>
                                                                    <div>Size: {card.size}</div>
                                                                </div>
                                                            )}
                                                        </Draggable>
                                                    ))}
                                                    {provided.placeholder}
                                                </div>
                                            )}
                                        </Droppable>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </DragDropContext>
                )
            )}
            {showEditModal && (<EditModal
                toggleEditModal={toggleEditModal}
                card={card}
                fetchListByBoardId={fetchListByBoardId}
                boardId={boardId}
            />)}
            {confirmationModal && (<div className="confirmation-modal-overlay">
                <div className="confirmation-modal">
                    <h2>Are you sure?</h2>
                    <div>
                        <button onClick={() => { handleDelete() }}>Delete</button>
                        <button onClick={(e) => { e.preventDefault(); setConfirmationModal(false) }}>Cancel</button>
                    </div>
                </div>
            </div>)}
            {boardNameEditClicked && (<div className="confirmation-modal-overlay">
                <div className="confirmation-modal" id="board-name-edit">
                    <h2 id="board-edit-h2">You can edit your board name here, or delete the board!</h2>
                    <div className="board-name-edit-container">
                        <h3>Enter your new board name</h3>
                        <div className="board-name-input">
                            <input type="text" placeholder={selectedBoard.name} maxLength={22} onChange={(e) => setNewBoardName(e.target.value)}></input>
                            <button onClick={handleBoardNameChange}>Save</button>
                            {newBoardNameOk ? (null) : (<p className="warning">New board name must be 3-22 characters long!</p>)}
                        </div>
                    </div>
                    <div className="delete-cancel">
                        <button onClick={() => { setBoardDeleteConfirmation(true) }}>Delete</button>
                        <button onClick={(e) => { e.preventDefault(); setBoardNameEditClicked(false); setNewBoardName(""); setNewBoardNameOk(true) }}>Cancel</button>
                        {boardDeleteConfirmation && (<div className="confirmation-modal-overlay">
                            <div className="confirmation-modal">
                                <h2>Are you sure?</h2>
                                <div>
                                    <button onClick={() => { handleBoardDelete() }}>Delete</button>
                                    <button onClick={(e) => { e.preventDefault(); setBoardDeleteConfirmation(false) }}>Cancel</button>
                                </div>
                            </div>
                        </div>)}
                    </div>
                </div>
            </div>)}
        </div>
    );
}

export default Menu;