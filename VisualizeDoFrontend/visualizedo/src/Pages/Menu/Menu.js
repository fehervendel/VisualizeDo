import Cookies from "js-cookie";
import LoginMenu from "../LoginMenu/LoginMenu";
import React, { useState, useEffect } from "react";
import { json, useNavigate } from "react-router-dom";
import "./Menu.css";
import API_URL from "../config";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";
import Modal from "../../Components/Modal";


function Menu() {
    const token = Cookies.get("userToken");
    const userEmail = Cookies.get("userEmail");
    const [boards, setBoards] = useState(null);
    const [lists, setLists] = useState(null);
    const [selectedBoard, setSelectedBoard] = useState(null);
    const [modal, setModal] = useState(false);
    const [listId, setListId] = useState(null);
    const [boardId, setBoardId] = useState(null);
    //console.log(selectedBoard);

    const fetchBoard = async () => {
        try {
            const response = await fetch(`${API_URL}/VisualizeDo/GetUserByEmail?email=${userEmail}`, {
                method: "GET",
                headers: {
                    //Authorization: `Bearer ${token}`
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                },
            });
            const jsonData = await response.json();
            //console.log("userboards" + jsonData);
            setBoards(jsonData.boards);
            const selected = jsonData.boards.find((board) => board.id == selectedBoard?.id);
            setSelectedBoard(selected);
        } catch (err) {
            console.error(err);
        }

    };

    useEffect(() => {
        fetchBoard();
    }, [userEmail])

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
            const response = await fetch(`https://localhost:7225/VisualizeDo/ChangeCardsListById?cardId=${cardId}&listId=${listId}`, {
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
        originalColumn.cards = updatedSourceColumn;

        let updatedDestinationColumn = tempLists.find((list) => list.id == listId); // This is the destination column
        updatedDestinationColumn.cards.splice(destination.index, 0, movedCard); // Here we just add the card to the destination column with the correct index

        setLists(tempLists);
        //card swap logic end

        changeCardListById(cardId, listId);
    }

    const toggleModal = () => {
        setModal(!modal);
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
                            <h3 className="board-name">{selectedBoard.name}</h3>
                            <div className="all-list-container">
                                {lists && lists.map((list, index) => (
                                    <div className="list-container" key={list.id}>
                                        <div className="list-head">
                                            <h4>{list.name}</h4>
                                            <button className="add-button" onClick={() => { toggleModal(); setListId(list.id); }}>Add card</button>
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
            {modal && (<Modal
                toggleModal={toggleModal}
                listId={listId}
                boardId={boardId}
                fetchListByBoardId={fetchListByBoardId}
            />)}
        </div>
    );
}

export default Menu;