import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import { Card, Typography } from "@material-tailwind/react";
import Return from "./Return";
import DeleteLoan from "./DeleteLoan";

function Loans() {
  const TABLE_HEAD = ["ID", "Member", "Book", "Loan date", "Returned", "", ""];
  const [error, setError] = useState(false);
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true); // Add loading state
  const [memberNames, setMemberNames] = useState({});
  const [bookTitles, setBookTitles] = useState({});

  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7241/api/Loan/GetAllLoans", {
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        console.log("uslooooo");
        if (response.status !== 200) {
          setError(true);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        console.log(data);
        if (error === false) {
          setLoans(data);
          console.log(data);
        }
      });
  }, []);

  useEffect(() => {
    if (loans.length === 0) return; 
    const memberNamesObj = {};
    const bookTitlesObj = {};

    Promise.all(
      loans.map((loan) => {
        return Promise.all([
          getName(loan.memberId).then((name) => {
            memberNamesObj[loan.memberId] = name;
          }),
          getTitle(loan.bookId).then((title) => {
            bookTitlesObj[loan.bookId] = title;
          }),
        ]);
      })
    )
      .then(() => {
        setMemberNames(memberNamesObj);
        setBookTitles(bookTitlesObj);
        setLoading(false); 
      })
      .catch((error) => setError(true));
  }, [loans]);

  function getName(id) {
    const url = "https://localhost:7241/api/Member/GetMemberName/" + id;
    return fetch(url, {
      headers: {
        "Content-Type": "application/json",
      },
    }).then((response) => {
      if (response.status !== 200) {
        setError(true);
      } else {
        return response.json().then((data) => data.name + " " + data.surname);
      }
    });
  }

  function getTitle(id) {
    const url = "https://localhost:7241/api/Book/GetBookTitle/" + id;
    return fetch(url, {
      headers: {
        "Content-Type": "application/json",
      },
    }).then((response) => {
      if (response.status !== 200) {
        setError(true);
      } else {
        return response.json().then((data) => data.title + "-" + data.author);
      }
    });
  }

  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/Loans">Return</Link>
      </>
    );
  }

  if (loading) {
    return <p>Loading...</p>;
  }

  return (
    <div data-testid="loansTable">
      {loans.length > 0 ? (
        <>
          <Card className="h-full w-full overflow-scroll">
            <table className="w-full min-w-max table-auto text-left">
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
                {loans.map((loan) => (
                  <tr data-testid={loan.id} key={loan.id}>
                    <td className="p-4 border-b border-blue-gray-50">
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {loan.id}
                      </Typography>
                    </td>
                    <td className="p-4 border-b border-blue-gray-50">
                      <Typography
                        variant="small"
                        color="blue-gray"
                        className="font-normal"
                      >
                        {memberNames[loan.memberId]} ({loan.memberId})
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
                        <time dateTime={loan.loanDate}>{loan.loanDate}</time>
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
                      ) : (
                        <div>
                          <p></p>
                        </div>
                      )}
                    </td>
                    <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                      <div className="flex-1">
                        <DeleteLoan id={loan.id} />
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </Card>
        </>
      ) : (
        <p>No loans available</p>
      )}
    </div>
  );
}

export default Loans;
