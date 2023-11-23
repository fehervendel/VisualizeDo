import './App.css';
import LoginMenu from './Pages/LoginMenu/LoginMenu';
import Menu from './Pages/Menu';

function App() {
  const isLoggedIn = false;

  return (
    <div className="App">
      {isLoggedIn ? <Menu/> : <LoginMenu/>}
    </div>
  );
}

export default App;
