import Cookies from "js-cookie";
import LoginMenu from "../LoginMenu/LoginMenu";
import React, { useState, useEffect } from "react";
import { json, useNavigate } from "react-router-dom";
import "./Menu.css";
import API_URL from "../config";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";


function Menu() {
    const token = Cookies.get("userToken");
    const userEmail = Cookies.get("userEmail");
    const [boards, setBoards] = useState(null);
    const [lists, setLists] = useState(null);
    const [selectedBoard, setSelectedBoard] = useState(null);
    //console.log(selectedBoard);
    const [selectedBoardId, setSelectedBoardId] = useState(null);

    const fetchData = async () => {
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
        fetchData();
    }, [userEmail])

    const changeCardListById = async (cardId, listId) => {
        try {
            const response = await fetch(`https://localhost:7225/VisualizeDo/ChangeCardsListById?cardId=${cardId}&listId=${listId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
            })
            console.log("Successful request", response);
            if(response.ok){
                //fetchData();
                let tempBoard = JSON.parse(JSON.stringify(selectedBoard));
        
                let originalColumn = tempBoard.lists.filter((list) => list.cards.find((card) => card.id == cardId))[0];
                let updatedSourceColumn = originalColumn.cards.filter((card) => card.id != cardId);
                originalColumn.cards = updatedSourceColumn;
                
                let movedCard = originalColumn.cards.filter((card) => card.id == cardId)[0];
                let updatedDestinationColumn = tempBoard.lists.filter((list) => list.id == listId)[0];
                updatedDestinationColumn.cards.push(movedCard);
                //console.log(updatedDestinationColumn);
                console.log("temp1", tempBoard);
                tempBoard.lists = tempBoard.lists.map((list) => list.id == originalColumn.id ? (originalColumn) : (list));
                console.log("temp2", tempBoard);
                //console.log(updatedSourceColumn);
                setSelectedBoard(tempBoard);
            }
        } catch (err) {
            console.error(err);
        }
    };


    const handleBoardChange = (e) => {
        const boardId = e.target.value;
        console.log(e.target.value);
        setSelectedBoardId(boardId);
        const selected = boards.find((board) => board.id == boardId);
        setSelectedBoard(selected);
        if (selected != undefined) {
            setLists(selected.lists);
        }
    }

    function handleDragEnd(result) {
        const { source, destination } = result;
        if (!destination) {
            return;
        }
        const cardId = parseInt(result.draggableId);
        const listId = parseInt(destination.droppableId);

        changeCardListById(cardId, listId);
    }

    return (
        <div className="main-div">
            <select onChange={(e) => handleBoardChange(e)}>
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
                            <h3>{selectedBoard.name}</h3>
                            <div className="all-list-container">
                                {selectedBoard.lists.map((list, index) => (
                                    <Droppable key={list.id} droppableId={list.id.toString()}>
                                        {(provided) => (
                                            <div className="list-container" ref={provided.innerRef} {...provided.droppableProps}>
                                                <div className="list-head">
                                                    <h4>{list.name}</h4>
                                                    <button className="add-button">Add card</button>
                                                </div>
                                                {list.cards.map((card, index) => (
                                                    <Draggable key={card.id} draggableId={card.id.toString()} index={index}>
                                                        {(provided) => (
                                                            <div {...provided.draggableProps} {...provided.dragHandleProps} ref={provided.innerRef} className="card">
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
                                ))}
                            </div>
                        </div>
                    </DragDropContext>
                )
            )}

        </div>
    );
}

export default Menu;