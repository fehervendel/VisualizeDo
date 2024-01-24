import API_URL from "../Pages/config.js";
import Cookies from "js-cookie";

export const getUserByEmail = async () => {
    try {
        const userEmail = Cookies.get("userEmail");
        const response = await fetch(`${API_URL}/VisualizeDo/GetUserByEmail?email=${userEmail}`, {
            method: "GET",
            headers: {
                //Authorization: `Bearer ${token}`
                'Content-Type': 'application/json',
                'Accept': 'application/json',
            },
        });
        const jsonData = await response.json();

        return { status: response.status, data: jsonData }
        // setBoards(jsonData.boards);
        // const selected = jsonData.boards.find((board) => board.id == selectedBoard?.id);
        // setSelectedBoard(selected);
    } catch (err) {
        return { status: 500, data: err }
    }
};
