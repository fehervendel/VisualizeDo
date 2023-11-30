import { useLocation } from 'react-router-dom';
import LoginMenu from '../Pages/LoginMenu/LoginMenu';

const Layout = () => {
    const location = useLocation();

    return(<div>{location.pathname === '/' ? (null) : (<div>
        Menu bar!
    </div>)}</div>);
}

export default Layout;