import './App.css';
import LoginMenu from './Pages/LoginMenu/LoginMenu';
import Menu from './Pages/Menu';
import Cookies from 'js-cookie';

function App() {
  const token = null;//Cookies.get("userToken");
  console.log(token);

  return (
    <div className="App">
      {token === null ?  <LoginMenu/> : <Menu/>}
    </div>
  );
}

export default App;
