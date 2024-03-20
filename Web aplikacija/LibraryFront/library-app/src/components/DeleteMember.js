import { useState } from "react";
import Modal from "react-bootstrap/Modal";
import { Link } from "react-router-dom";

export default function DeleteMember(props) {
  const [error, setError] = useState(false);

  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  function handleChange(e) {
    console.log("uslooo");
    e.preventDefault();
    const url = "https://localhost:7241/api/Member/DeleteMember/" + props.id;
    fetch(url, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (response.status !== 200) {
          if (response.status === 400) {
            alert("Can't delete member,books are not returned.")
          } else {
            setError(true);
            alert("Something is wrong");
          }
        }
      })
      .then((data) => {
        console.log(data);
        window.location.reload();
      })
      .catch((error) => {
        alert("Error: " + error.message);
      });
  }
  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/members">Return</Link>
      </>
    );
  }

  return (
    <>
      <button
        onClick={handleShow}
        className="px-4 py-1 text-sm text-purple-600 font-semibold rounded-full border border-purple-200 hover:text-white hover:bg-purple-600 hover:border-transparent focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
      >
        Delete
      </button>

      <Modal
      data-testid="deleteMemberModal"
        show={show}
        onHide={handleClose}
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header closeButton>
          <Modal.Title>Delete member</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form
            id="deletemodal"
            className="w-full max-w-sm"
            onSubmit={(e) => {
              handleClose();
              e.preventDefault();
            }}
          >
            <p>Are you sure?</p>
          </form>
        </Modal.Body>
        <Modal.Footer>
          <button
            className="bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 px-4 rounded"
            form="editmodal"
            onClick={handleChange}
          >
            Yes
          </button>
          <button
            className="bg-slate-400 hover:bg-slate-500 text-white font-bold py-2 px-4 rounded"
            onClick={handleClose}
          >
            No
          </button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
