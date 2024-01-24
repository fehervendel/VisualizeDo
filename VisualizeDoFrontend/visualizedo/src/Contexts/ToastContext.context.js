import { createContext, useState } from "react";
import { Outlet } from "react-router-dom";
import ToastContainer from "../Components/ToastContainer";

export const ToastContext = createContext(null)


export function ToastContextProvider() {
    const [toasts, setToasts] = useState([])

    function NextToastId() {
        if (toasts.length == 0) {
            return 1;
        }
        return toasts[toasts.length-1].id + 1
    }

    function AddToast(message) {
        const newToast = { message: message, id: NextToastId() }
        setToasts((prevState) => [...prevState, newToast])
    }

    function RemoveToast(id) {
        setToasts((prevState) => [...prevState.filter(nextToast => nextToast.id != id)])
    }

    return (
        <ToastContext.Provider value={{ toasts, setToasts, AddToast, RemoveToast }}>
            <Outlet/>
            <ToastContainer/>
        </ToastContext.Provider>
    )
}