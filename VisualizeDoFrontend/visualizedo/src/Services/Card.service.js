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