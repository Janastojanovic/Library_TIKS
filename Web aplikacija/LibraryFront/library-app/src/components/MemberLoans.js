import { Card, Typography } from "@material-tailwind/react";
import { useEffect, useState } from "react";
import Modal from "react-bootstrap/Modal";
import { Link } from "react-router-dom";
import DeleteLoan from "./DeleteLoan";
import Return from "./Return";

export default function MemberLoans(props) {
  const TABLE_HEAD = ["ID","Book", "Loan date", "Returned", "", ""];
  const [error, setError] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(true);
  const [loans, setLoans] = useState([]);
  const [memberName, setMemberName] = useState("");
  const [bookTitles, setBookTitles] = useState({});

  const handleOpenModal = () => {
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
  };

  useEffect(() => {
    console.log("Fetching member loans...");
    setLoading(true);
    fetch("https://localhost:7241/api/Loan/GetLoansByMemberId/" + props.id, {
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        console.log("Fetching loans response:", response);
        if (!response.ok) {
          throw new Error("Failed to fetch loans");
        }
        return response.json();
      })
      .then((data) => {
        console.log("Loans data:", data);
        setLoans(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Error fetching loans:", error);
        setError(true);
        setLoading(false);
      });
  }, [props.id]);

  useEffect(() => {
    if (loans.length === 0) return;

    console.log("Fetching member name...");
    fetchMemberName(props.id);

    console.log("Fetching book titles...");
    fetchBookTitles(loans.map((loan) => loan.bookId));
  }, [loans]);

  const fetchMemberName = (id) => {
    const url = "https://localhost:7241/api/Member/GetMemberName/" + id;
    fetch(url, {
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch member name");
        }
        return response.json();
      })
      .then((data) => {
        setMemberName(data.name + " " + data.surname);
      })
      .catch((error) => {
        console.error("Error fetching member name:", error);
        setError(true);
      });
  };

  const fetchBookTitles = (bookIds) => {
    const requests = bookIds.map((id) => {
      const url = "https://localhost:7241/api/Book/GetBookTitle/" + id;
      return fetch(url, {
        headers: {
          "Content-Type": "application/json",
        },
      }).then((response) => {
        if (!response.ok) {
          throw new Error("Failed to fetch book title");
        }
        return response.json().then((data) => data.title + "-" + data.author);
      });
    });

    Promise.all(requests)
      .then((titles) => {
        const titleMap = {};
        titles.forEach((title, index) => {
          titleMap[bookIds[index]] = title;
        });
        setBookTitles(titleMap);
      })
      .catch((error) => {
        console.error("Error fetching book titles:", error);
        setError(true);
      });
  };

  if (error) {
    return (
      <>
        <p>Something went wrong</p>
        <Link to="/members">Return</Link>
      </>
    );
  }

  return (
    <>
      <button
        onClick={handleOpenModal}
        className="px-4 py-1 text-sm text-purple-600 font-semibold rounded-full border border-purple-200 hover:text-white hover:bg-purple-600 hover:border-transparent focus:outline-none focus:ring-2 focus:ring-purple-600 focus:ring-offset-2"
      >
        Member loans
      </button>

      <Modal
        data-testid="memberLoansModal"
        size="lg"
        show={showModal}
        onHide={handleCloseModal}
        backdrop="static"
        keyboard={false}
      >
        <Modal.Header closeButton>
          <Modal.Title>{memberName}</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {loading ? (
            <p>Loading...</p>
          ) : loans.length > 0 ? (
            <Card className="overflow-hidden">
              <table
                className="table-auto text-left"
                data-testid="memberLoansTable"
              >
                <thead>
                  <tr>
                    {TABLE_HEAD.map((head) => (
                      <th
                        key={head}
                        className="border-b border-blue-gray-100 bg-blue-gray-50 p-4"
                      >
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal leading-none opacity-70"
                        >
                          {head}
                        </Typography>
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {loans.map((loan) => {
                    const rowClassName = loan.returned ? "" : "bg-purple-100";
                    return (
                      <tr
                        key={loan.id}
                        className={rowClassName}
                        data-testid={loan.id}
                      >
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {loan.id}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {bookTitles[loan.bookId]} ({loan.bookId})
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            <time dateTime={loan.loanDate}>
                              {loan.loanDate}
                            </time>
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50">
                          <Typography
                            variant="small"
                            color="blue-gray"
                            className="font-normal"
                          >
                            {loan.returned ? <p>Yes</p> : <p>No</p>}
                          </Typography>
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          {!loan.returned ? (
                            <div className="flex">
                              <div className="flex-1">
                                <Return id={loan.id} />
                              </div>
                            </div>
                          ) : null}
                        </td>
                        <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                          <div className="flex-1">
                            <DeleteLoan id={loan.id} />
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </Card>
          ) : (
            <p>No loans available</p>
          )}
        </Modal.Body>
        <Modal.Footer>
          <button
            className="bg-slate-400 hover:bg-slate-500 text-white font-bold py-2 px-4 rounded"
            onClick={handleCloseModal}
          >
            Close
          </button>
        </Modal.Footer>
      </Modal>
    </>
  );
}
