import { useState } from "react";

function useModal(){
    const [show, setShow] = useState(false);

    const toggleModal = () => {
        setShow(!show);
    }

    return {toggleModal, show, setShow}
}

export default useModal;