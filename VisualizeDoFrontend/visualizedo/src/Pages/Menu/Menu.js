import Cookies from "js-cookie";
import LoginMenu from "../LoginMenu/LoginMenu";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./Menu.css";
import API_URL from "../config";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";


function Menu() {
    const [todos, setTodos] = useState(null);
    const token = Cookies.get("userToken");
    const userEmail = Cookies.get("userEmail");
    const [boards, setBoards] = useState(null);
    const [selectedBoard, setSelectedBoard] = useState(null);
    //console.log(boards);



    useEffect(() => {
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
            } catch (err) {
                console.error(err);
            }

        };
        fetchData();
    }, [token])


    const handleBoardChange = (e) => {
        const boardId = e.target.value;
        const selected = boards.find((board) => board.id == boardId);
        setSelectedBoard(selected);
    }

    function handleDragEnd(result) {
        const { source, destiantion } = result;
        console.log(result);
        if (!destiantion) {
            return;
        }
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
            {selectedBoard && (
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
            )}
        </div>
    );
}

export default Menu;