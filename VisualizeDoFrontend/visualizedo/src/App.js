import './App.css';
import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import LoginMenu from './Pages/LoginMenu/LoginMenu';
import Boards from './Pages/Boards/Boards';
import Layout from './Components/Layout';
import Create from './Pages/Create/Create';

import { ToastContextProvider } from './Contexts/ToastContext.context';

function App() {
return (
  <BrowserRouter>
  <Layout/>
    <Routes>
      <Route path='/' element={<ToastContextProvider/>}>
        <Route path="/" element={<LoginMenu/>}></Route>
        <Route path="/Boards" element={<Boards/>}></Route>
        <Route path="/Create" element={<Create/>}></Route>
      </Route>
    </Routes>
  </BrowserRouter>
)
}

export default App
