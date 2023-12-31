import React from "react";

function Registration(props){
    const className = props.className;
    const type = props.type;
    const label = props.label;
    const name = props.name;
    const value = props.value;
    const showPassword = props.showPassword;
    const setShowPassword = props.setShowPassword;
    const warnings = props.warnings;
    
    function changeShowPassword(){
        setShowPassword(!showPassword)
      }
      
    return (
        <div className={className}>
          <label>
            {label}
          </label>
          {type === "password" && (
            <div className="showPassword">
              Show password
              <input
                type="checkbox"
                id="checkbox"
                checked={showPassword}
                onChange={changeShowPassword}
              />
            </div>
          )}
          <input
            type={showPassword ? 'text' : type}
            name={name}
            value={value}
            onChange={props.onChange}
          />
          <div className="warning">{warnings}</div>
        </div>
      );
}

export default Registration;