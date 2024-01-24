import API_URL from "../config";

export const changeCardList = async (cardId, listId) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/ChangeCardsListById?cardId=${cardId}&listId=${listId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
            },
        })
            const data = await response.json();
            return {status: response.status, data: data}
    } catch (err) {
        return { status: 500, data: err }
    }
};

export const add = async (listId, cardTitle, cardDescription, priority, size) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/AddCard`, {

            method: "POST",
            headers: {
                "Content-Type": "application/json",
                //"Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                listId: listId,
                title: cardTitle,
                description: cardDescription,
                priority: priority,
                size: size
            }),
        })
        const data = response.json();
        return { status: response.status, data: data }
    } catch (error) {
        return { status: 500, data: error }
    }
};

export const editCard = async (propsCardId, title, description, priority, size) => {
    try {
        const response = await fetch(`${API_URL}/VisualizeDo/EditCard`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                //"Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                id: propsCardId,
                title: title,
                description: description,
                priority: priority,
                size: size
            }),
        })
        const data = await response.json();
        return { status: response.status, data: data }
    } catch (error) {
        return { status: 500, data: error }
    }
};

export const deleteCard = async (propsCardId) => {
    try {
      const response = await fetch(`${API_URL}/VisualizeDo/DeleteCardById?id=${propsCardId}`, {
        method: 'DELETE',
      });
      const data = await response.json();
      return { status: response.status, data: data }
    } catch (error) {
        return { status: 500, data: error }
    }
  };