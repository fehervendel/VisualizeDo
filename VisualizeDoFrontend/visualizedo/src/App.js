import './App.css';
import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import LoginMenu from './Pages/LoginMenu/LoginMenu';
import Menu from './Pages/Menu/Menu';
import Layout from './Components/Layout';

function App() {
return (
  <BrowserRouter>
  <Layout/>
    <Routes>
      <Route path="/" element={<LoginMenu/>}></Route>
      <Route path="/Menu" element={<Menu/>}></Route>
    </Routes>
  </BrowserRouter>
)
}

export default App
