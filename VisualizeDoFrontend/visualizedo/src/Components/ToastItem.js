import { useEffect } from "react"


export default function ToastItem({toast, RemoveToast}) {

    let timeOut = null;
    const duration = 5000;
    const durationForScss = `${duration/1000}s`;

    useEffect(() => {
        timeOut = setTimeout(() => {
            RemoveToast(toast.id)
        }, duration)

        return () => {
            clearTimeout(timeOut)
        }
    }, [])

    

    return(
        <div className="toastItem" style={{ "--duration": durationForScss }}>
            <h3 className="toast-message">{toast.message}</h3>
        </div>
    )
}