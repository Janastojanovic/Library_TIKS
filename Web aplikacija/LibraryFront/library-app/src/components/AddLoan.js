import { useState } from "react";
import Modal from "react-bootstrap/Modal";
import { Link } from "react-router-dom";

function AddLoan(props) {
  const [error, setError] = useState(false);
  const [memberId, setMemberId] = useState(props.memberId);
  const [bookId, setBookId] = useState("");

  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  function handleChange(e) {
    e.preventDefault();
    const url = "https://localhost:7241/api/Loan/AddLoan";
    fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        memberId: memberId,
        bookId: bookId,
      }),
    })
      .then((response) => {
        if (response.status !== 200) {
          if (response.status === 400) {
            alert("This book is no longer available");
          }
          if (response.status === 404) {
            alert("Invalid book ID");
          }

          setError(true);
        } else {
          console.log(response);
          return response.json();
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
        <Link to="/loans">Return</Link>
      </>
    );
  }
  return (
    <>
      <button
        onClick={handleShow}
        className="px-4 py-1 text-sm text-purple-600 font-semibold rounded-full border border-purple-200 hover:text-white hover:bg-purple-600 hover:border-transparent focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
      >
        + Add Loan
      </button>

      <Modal
        data-testid="addLoanModal"
        show={show}
        onHide={handleClose}
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header closeButton>
          <Modal.Title>Add loan</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <form
            id="editmodal"
            className="w-full max-w-sm"
            onSubmit={(e) => {
              e.preventDefault();
              setMemberId(1);
              setBookId(0);
              props.newLoan(memberId, bookId);
            }}
          >
            <div className="md:flex md:items-center mb-6">
              <div className="md:w-1/3">
                <label
                  className="block text-gray-500 font-bold md:text-right mb-1 md:mb-0 pr-4"
                  for="bookId"
                >
                  BookId
                </label>
              </div>
              <div className="md:w-2/3">
                <input
                  className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                  id="bookId"
                  placeholder="Book ID"
                  type="text"
                  value={bookId}
                  onChange={(e) => {
                    setBookId(e.target.value);
                  }}
                />
              </div>
            </div>
          </form>
        </Modal.Body>
        <Modal.Footer>
          <button
            className="bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 px-4 rounded"
            onClick={handleChange}
            form="editmodal"
          >
            Add
          </button>
          <button
            className="bg-slate-400 hover:bg-slate-500 text-white font-bold py-2 px-4 rounded"
            onClick={handleClose}
          >
            Close
          </button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

export default AddLoan;
