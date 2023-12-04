import { useLocation } from 'react-router-dom';
import LoginMenu from '../Pages/LoginMenu/LoginMenu';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';

const Layout = () => {
    const location = useLocation();
    const [token, setToken] = useState(Cookies.get("userToken"));
    const navigate = useNavigate();

    const handleLogout = (e) => {
        e.preventDefault();
        Cookies.remove("userToken");
        setToken(null);
        navigate("/");
    }

    return(<div>{location.pathname === '/' ? (null) : (<div>
        Menu bar!
        <button onClick={(e) => handleLogout(e)}>Logout</button>
    </div>)}</div>);
}

export default Layout;