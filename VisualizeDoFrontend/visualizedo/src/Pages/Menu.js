import Cookies from "js-cookie";
import LoginMenu from "./LoginMenu/LoginMenu";
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

function Menu() {
    const [token, setToken] = useState(Cookies.get("userToken"));
    const navigate = useNavigate();

    const handleLogout = (e) => {
        e.preventDefault();
        Cookies.remove("userToken");
        setToken(null);
        navigate("/");
    }
    console.log(token);
    return(<div>
                <div>Menu</div>
                <button onClick={(e) => handleLogout(e)}>Logout</button>
        </div>)
}

export default Menu;