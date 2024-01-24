import { useContext } from "react"
import { ToastContext } from "../Contexts/ToastContext.context"
import ToastItem from "./ToastItem"


export default function ToastContainer() {
    const { toasts, RemoveToast } = useContext(ToastContext)

    return(
        <div className="toastContainer">
            { toasts.map(nextToast => <div key={nextToast.id}><ToastItem toast={nextToast} RemoveToast={RemoveToast}></ToastItem></div>) }
        </div>
    )
}