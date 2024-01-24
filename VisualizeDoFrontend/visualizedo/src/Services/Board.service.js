import API_URL from "../config";
import Cookies from "js-cookie";

export const boardNameChange = async (boardId, newBoardName) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/ChangeBoardName?id=${boardId}&newName=${newBoardName}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                //"Authorization": `Bearer ${token}`
            },
        });
        const data = await response.json();
        return {status: response.status, data: data}
    } catch (e) {
        return { status: 500, data: e}
    }
};

export const boardDelete = async (selectedBoardId) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/DeleteBoardById?id=${selectedBoardId}`, {
            method: "DELETE",
            headers: {
                "Content-Type": "text/plain",
                //"Authorization": `Bearer ${token}`
            },
        });
        const data = await response.text();
        return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err}
    }
};

export const addNewBoard = async (boardName) => {
    try {
        const userEmail = Cookies.get("userEmail");
        const response = await fetch(`${API_URL}/VisualizeDo/AddBoard?email=${userEmail}&name=${boardName}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                //"Authorization": `Bearer ${token}`
            },
        })
        const data = await response.json();
        return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err}
    }
};