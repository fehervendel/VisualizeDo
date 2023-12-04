import Cookies from "js-cookie";
import LoginMenu from "../LoginMenu/LoginMenu";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./Menu.css";
import API_URL from "../config";

function Menu() {
    const [todos, setTodos] = useState(null);
    const token = Cookies.get("userToken");
    const userEmail = Cookies.get("userEmail");
    const [boards, setBoards] = useState(null);
    const [selectedBoard, setSelectedBoard] = useState(null);
    console.log(boards);
    


    useEffect(() => {
        const fetchData = async () => {
            try{
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
            } catch(err) {
                console.error(err);
            }
            
        };
        fetchData();
    }, [token])

    const dragStart = (e, id) => {
        e.dataTransfer.setData("todoId", id);
        console.log("drag started");
    }

    const draggingOver = (e) => {
        e.preventDefault();
        console.log("dragging over now");
    }

    const dropped = (e, targetDivId) => {
        e.preventDefault();
        const droppedTodo = e.dataTransfer.getData("todoId");
    }

    const handleBoardChange = (e) => {
        const boardId = e.target.value;
        const selected = boards.find((board) => board.id == boardId);
        //const selectedb = {...selected};
        //console.log(selectedb + "selected");
        setSelectedBoard(selected);
    }

    return(<div className="main-div">
                <select onChange={(e) => handleBoardChange(e)}>
                <option value="">Choose one of your boards...</option>
                {boards && boards.map((board, index) => (
                    <option key={board.id} value={board.id}>
                        {board.name}
                    </option>
                ))}
            </select>
            {selectedBoard && (<div>
                <h3>{selectedBoard.name}</h3>
                    <div className="all-list-container">
                    
                    {selectedBoard.lists.map((list, index) =>(
                        <div className="list-container" key={list.id}>
                        <h4>{list.name}</h4>
                        <div className="div-container">
                        {list.cards.map((card) => (
                            <div key={card.id} className="card">
                            <div>Title: {card.title}</div>
                            <div>Description: {card.description}</div>
                            <div>Priority: {card.priority}</div>
                            <div>Size: {card.size}</div>
                            </div>
                        ))}
                        </div>
                        </div>
                        
                    ))}
                </div> 
                </div>
            )}    
        </div>)
}

export default Menu;
//<div onDragOver={(e) => draggingOver(e)} onDrop={(e) => dropped(e, "div1")} className="list">Div1</div>
  //              <div onDragOver={(e) => draggingOver(e)} onDrop={(e) => dropped(e, "div2")} className="list">Div2</div>

//   <div 
//                     draggable
//                     onDragStart={(e) => dragStart(e)}>
//                         todo1
//                     </div>