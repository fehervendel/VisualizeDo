import { Navigate, useLocation } from 'react-router-dom';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import Cookies from 'js-cookie';
import './Layout.css';
import useLogout from '../Hooks/useLogout';

const Layout = () => {
    const navigate = useNavigate();
   const {location,
    handleLogout} = useLogout();

    const handleCreateClick = () => {
        navigate('/create');
    }

    const handleBoardsClick = () => {
        navigate('/Menu');
    }



    return (<div>{location.pathname === '/' ? (null) : (<div className='menuBar'>
        <div className='menuItems'>
            <a className='button' onClick={handleCreateClick}>Create</a>
        </div>
        <div className='menuItems'>
            <a className='button' id='boardButton' onClick={handleBoardsClick}>Boards</a>
        </div>
        <div className='menuItems' id='logoutButton'>
            <a className='logoutButton' onClick={(e) => handleLogout(e)}>Logout</a>
        </div>
    </div>)}</div>);
}

export default Layout;