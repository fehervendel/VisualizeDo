import React, { useEffect } from "react";
import Registration from "../../Components/Registration";
import { useState } from "react";
import Cookies from "js-cookie";
import API_URL from "../config";
import "./LoginMenu.css";
import todoList from "../../images/todoList.png"
import { useNavigate } from "react-router-dom";
import todoList2 from "../../images/todoList.jpg";
import todoList3 from "../../images/todoList3.png";
import { useRef } from "react";

function LoginMenu() {
    const [isRegistrationClicked, setIsRegistrationClicked] = useState(false);
    const [inputValues, setInputValues] = useState({});
    const [saveEmail, setSaveEmail] = useState("");
    const [savePassword, setSavePassword] = useState("");
    const [userNameWarning, setUserNameWarning] = useState("");
    //const token = Cookies.get("userToken");
    const [emailWarning, setEmailWarning] = useState("");
    const [passwordWarning, setPassowrdWarning] = useState("");
    const [registrationSuccess, setRegistrationSuccess] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const warnings = [userNameWarning, emailWarning, passwordWarning];
    const navigate = useNavigate();
    const images = [todoList, todoList2, todoList3];
    const [currentImageIndex, setCurrentImageIndex] = useState(0);
    const intervalRef = useRef(null);

    const inputFields = [
        {className: "userName", type: "text", label: "Username:", name:"userName"},
        {className: "email", type: "email", label: "Email:", name: "email"},
        {className: "password", type: "password", label: "Password:", name:"password"}
      ];

      function handleInputChange(event) {
        event.preventDefault();
        const { name, value } = event.target;
        setInputValues({...inputValues, [name]: value});
      }

      const handleSubmit = async (event) => {
        event.preventDefault();
        setEmailWarning("");
        setPassowrdWarning("");
        try {
          await fetch(`${API_URL}/Auth/Login`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              email: saveEmail,
              password: savePassword,
            }),
          })
          .then((res) => res.json())
          .then((data) => {
            if(data && data.email && data.userName && data.token){
                Cookies.set("userEmail", data.email, { expires: 10 });
                Cookies.set("userUserName", data.userName, { expires: 10 });
                Cookies.set("userToken", data.token, { expires: 10 });

            console.log("Login response:", data);
              const tokenPayload = JSON.parse(atob(data.token.split('.')[1]));
              const userRole = tokenPayload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
              Cookies.set("userRole", userRole, { expires: 10 });
              navigate("/Menu");
            } else {
                if(data["Bad credentials"][0] === "Invalid email"){
                  setEmailWarning(data["Bad credentials"][0])
                }
                if(data["Bad credentials"][0] === "Invalid password"){
                  setPassowrdWarning(data["Bad credentials"][0])
                }
              }
          })
        } catch(err){
          console.error(err);
        }
      };

      const handleRegistrationSubmit = async (event) => {
        event.preventDefault();
        setUserNameWarning("");
        setEmailWarning("");
        setPassowrdWarning("");
        try {
          const response = await fetch(`${API_URL}/Auth/Register`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              email: inputValues.email,
              username: inputValues.userName,
              password: inputValues.password,
            }),
          });
      
          if (response.ok) {
            const data = await response.json();
            console.log(data); 
            setIsRegistrationClicked(false);
            setRegistrationSuccess(true);
            setTimeout(() => {
              setRegistrationSuccess(false);
            }, 5000);
          } else {
            const data = await response.json();
            console.log(data);
            if(data.DuplicateUserName && data.DuplicateUserName){
                setUserNameWarning("Username is already taken");
              }
              if(inputValues.userName === undefined || inputValues.userName === ""){
                setUserNameWarning("Please enter a username");
              }
              if((data.DuplicateEmail && data.DuplicateEmail) || inputValues.email === undefined){
                setEmailWarning("Email is already taken");
              }
              if(inputValues.email === undefined){
                setEmailWarning("Please anter an email address");
              }
              
              if(inputValues.password === undefined || inputValues.password.length < 6){
                setPassowrdWarning("Password must be at least 6 characters");
              }
          }
        } catch (error) {
          
          console.error('Network error:', error);
        }
      };

      function handleClick(){
        setIsRegistrationClicked(true);
        setInputValues({});
      }

      function back(){
        setIsRegistrationClicked(false);
        setUserNameWarning("");
        setEmailWarning("");
        setPassowrdWarning("");
      }

      const restartInterval = () => {
        clearInterval(intervalRef.current);
        intervalRef.current = setInterval(() => {
          setCurrentImageIndex((prevIndex) => {
            if (prevIndex === images.length - 1) {
              return 0;
            } else {
              return prevIndex + 1;
            }
          });
        }, 7000);
      };

      useEffect(() => {
        
        restartInterval();
    
        return () => {
          clearInterval(intervalRef.current);
        };
      }, []);
    
      const handleDotClick = (index) => {
        setCurrentImageIndex(index);
        restartInterval();
      }

      return (<div className="main-container">
        <div className="left-side">
          <div className="welcome-message">
        Welcome! Please {isRegistrationClicked ? 'Register' : 'Login'}
        </div>
        <p className="register-message">{registrationSuccess ? ("Successful registration! Log in.") : (null)}</p>
        <div className="register-container">
      {!isRegistrationClicked ? (
        <div>
        <section>
        <div className="register-input">
          <label>Email:</label>
          <input type="text" onChange={(e) => setSaveEmail(e.target.value)}></input>
          <div className="warning">{emailWarning}</div>
      </div>
      <div className="register-input" id="passwordInput">
        <label>Password:</label>  
        <input type="password" onChange={(e) => setSavePassword(e.target.value)}></input>
        <div className="warning">{passwordWarning}</div>
      </div>
      </section>
      <div>
        <button className="Button" onClick={handleSubmit}>Log in</button>
        </div>
        <div>
        <button className="Button" onClick={handleClick}>Register</button>
      </div>
      </div>
      ) : (
        <div className="register-input label">
        <form onSubmit={handleRegistrationSubmit}>
          {inputFields.map((inputField, index) =>(
            <Registration
            key={index}
            className={inputField.className}
            type={inputField.type}
            label={inputField.label}
            name={inputField.name}
            warnings={warnings[index]}
            value={inputValues[inputField.name] || ""}
            showPassword={showPassword}
            setShowPassword={setShowPassword}
            onChange={handleInputChange}/>
            
          ))}
          <button className="Button" type="submit">Submit</button>
          
        </form>
        <div>
          <button className="Button" onClick={back}>Already have an account? Sign in</button>
        </div>
        </div>
      )}
    </div>
    </div>
    <div className="right-side">
      <div className="image-container">
      <div className="slider" style={{ transform: `translateX(calc(-100% * ${currentImageIndex}))` }}>
          {images.map((image, index) => (
           <img
            key={index}
            src={image}
            alt={`Image ${index + 1}`}
            className={index === currentImageIndex ? 'active' : ''}
              />
          ))}
</div>
</div>
<div className="dots-container">
    {images.map((_, index) => (
      <span
        key={index}
        className={`dot ${index === currentImageIndex ? 'active-dot' : ''}`}
        onClick={() => handleDotClick(index)}
      ></span>
    ))}
  </div>
    <div className="footer-container">
      </div>
    </div>
        </div>);
}

export default LoginMenu;