import { useEffect } from "react"


export default function ToastItem({toast, RemoveToast}) {

    let timeOut = null;
    const duration = 3000;
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
        <div className="toastItem" style={{ "--duration": {durationForScss} }}>
            <h3>{toast.message}</h3>
        </div>
    )
}