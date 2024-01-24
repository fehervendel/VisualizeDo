import API_URL from "../config";

export const listNameChange = async (listNameEditId, newNameOfList) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/ChangeListName?listId=${listNameEditId}&newName=${newNameOfList}`, {
            method: "PUT"
        });
        const data = await response.text();
        
        return {status: response.status, data: data}
    } catch (e) {
        return { status: 500, data: e}
    }
};

export const addNewList = async (boardId, newListName) => {
    try {
      
        const response = await fetch(`${API_URL}/VisualizeDo/AddList?boardId=${boardId}&name=${newListName}`, {
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
}

export const deleteList = async (deleteListId) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/DeleteListById?id=${deleteListId}`, {
            method: "DELETE"
        });
        const data = await response.text();
        return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err}
    }
}

export const getListByBoardId = async (id) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/GetListsByBoardId?id=${id}`, {
            method: "GET",
            headers: {
                //Authorization: `Bearer ${token}`
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
        });
            const data = await response.json();
            return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err}
    }
};

export const addNewLists = async (id, listNames) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/AddLists?boardId=${id}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(listNames),
        });
        const data = await response.json();
        return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err}
    }
};