import { useLocation } from 'react-router-dom';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import './Layout.css';

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

    return (<div>{location.pathname === '/' ? (null) : (<div className='menuBar'>
        <div className='menuItems'>
            <button className='button'>Create</button>
        </div>
        <div className='menuItems'>
            <button className='button' id='boardButton'>Boards</button>
        </div>
        <div className='menuItems' id='logoutButton'>
            <button className='logoutButton' onClick={(e) => handleLogout(e)}>Logout</button>
        </div>
    </div>)}</div>);
}

export default Layout;