import { useLocation } from 'react-router-dom';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';

function useLogout(){
    const location = useLocation();
    const [token, setToken] = useState(Cookies.get("userToken"));
    const navigate = useNavigate();

    const handleLogout = (e) => {
        e.preventDefault();
        Cookies.remove("userToken");
        setToken(null);
        navigate("/");
    }

    return {
        location,
        handleLogout
    }
}

export default useLogout;