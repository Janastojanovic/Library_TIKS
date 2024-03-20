import { useState } from "react";
import Modal from "react-bootstrap/Modal";
import { Link } from "react-router-dom";

export default function EditMember(props) {
  const [error, setError] = useState(false);
  const [firstname, setFirstname] = useState(props.firstname);
  const [lastname, setLastname] = useState(props.lastname);
  const [email, setEmail] = useState(props.email);
  const [age, setAge] = useState(props.age);
  const [membershipExpiration, setMembershipExpiration] = useState(
    props.membershipExpiration
  );
  const [show, setShow] = useState(false);

  const handleClose = () => setShow(false);
  const handleShow = () => setShow(true);

  function handleChange(e) {
    console.log("uslooo");
    e.preventDefault();
    const url = "https://localhost:7241/api/Member/EditMember/" + props.id;
    if (firstname === "" || lastname === "") {
      alert("Firstname and lastname fields are required");
      window.location.reload();
    }
    fetch(url, {
      method: "PUT",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        id: props.id,
        firstname: firstname,
        lastname: lastname,
        email: email,
        age: age,
        membershipExpiration: membershipExpiration,
      }),
    })
      .then((response) => {
        if (response.status !== 200) {
          if (response.status === 400) {
            return response.json();
          }
          setError(true);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (data && data.errorMessage !== undefined) {
          console.log(data.errorMessage);
          alert(data.errorMessage);
          window.location.reload();
        }
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
    <form onSubmit={handleChange}>
      <>
        <button
          onClick={handleShow}
          className="px-4 py-1 text-sm text-purple-600 font-semibold rounded-full border border-purple-200 hover:text-white hover:bg-purple-600 hover:border-transparent focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
        >
          Edit
        </button>

        <Modal
          data-testid="editMemberModal"
          show={show}
          onHide={handleClose}
          backdrop="static"
          keyboard={false}
        >
          <Modal.Header closeButton>
            <Modal.Title>Edit member</Modal.Title>
          </Modal.Header>
          <Modal.Body>
            <form
              id="editmodal"
              className="w-full max-w-sm"
              onSubmit={(e) => {
                handleClose();
                console.log("Data:");
                e.preventDefault();
                props.updateMember(
                  props.id,
                  firstname,
                  lastname,
                  email,
                  age,
                  membershipExpiration
                );
              }}
            >
              <div className="md:flex md:items-center mb-6">
                <div className="md:w-1/3">
                  <label
                    className="block text-gray-500 font-bold md:text-right mb-1 md:mb-0 pr-4"
                    for="firstname"
                  >
                    Firstname
                  </label>
                </div>
                <div className="md:w-2/3">
                  <input
                    className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                    id="firstname"
                    type="text"
                    value={firstname}
                    onChange={(e) => {
                      setFirstname(e.target.value);
                    }}
                  />
                </div>
              </div>
              <div className="md:flex md:items-center mb-6">
                <div className="md:w-1/3">
                  <label
                    className="block text-gray-500 font-bold md:text-right mb-1 md:mb-0 pr-4"
                    for="lastname"
                  >
                    Lastname
                  </label>
                </div>
                <div className="md:w-2/3">
                  <input
                    className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                    id="lastname"
                    type="text"
                    value={lastname}
                    onChange={(e) => {
                      setLastname(e.target.value);
                    }}
                  />
                </div>
              </div>
              <div className="md:flex md:items-center mb-6">
                <div className="md:w-1/3">
                  <label
                    className="block text-gray-500 font-bold md:text-right mb-1 md:mb-0 pr-4"
                    for="email"
                  >
                    Email
                  </label>
                </div>
                <div className="md:w-2/3">
                  <input
                    className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                    id="email"
                    type="text"
                    value={email}
                    onChange={(e) => {
                      setEmail(e.target.value);
                    }}
                  />
                </div>
              </div>
              <div className="md:flex md:items-center mb-6">
                <div className="md:w-1/3">
                  <label
                    className="block text-gray-500 font-bold md:text-right mb-1 md:mb-0 pr-4"
                    for="age"
                  >
                    Age
                  </label>
                </div>
                <div className="md:w-2/3">
                  <input
                    className="bg-gray-200 appearance-none border-2 border-gray-200 rounded w-full py-2 px-4 text-gray-700 leading-tight focus:outline-none focus:bg-white focus:border-purple-500"
                    id="age"
                    type="text"
                    value={age}
                    onChange={(e) => {
                      setAge(e.target.value);
                    }}
                  />
                </div>
              </div>
            </form>
          </Modal.Body>
          <Modal.Footer>
            <button
              className="bg-purple-600 hover:bg-purple-700 text-white font-bold py-2 px-4 rounded"
              form="editmodal"
            >
              Edit
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
    </form>
  );
}
