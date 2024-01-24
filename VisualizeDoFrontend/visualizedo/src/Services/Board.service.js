import API_URL from "../Pages/config";

export const boardNameChange = async (boardId, newBoardName) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/ChangeBoardName?id=${boardId}&newName=${newBoardName}`, {
            method: "PUT"
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
            method: "DELETE"
        });
        const data = await response.text();
        
        return {status: response.status, data: data}
    } catch (e) {
        return { status: 500, data: e}
    }
};