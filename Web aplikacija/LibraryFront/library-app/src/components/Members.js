import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import EditMember from "./EditMember";
import "bootstrap/dist/css/bootstrap.min.css";
import DeleteMember from "./DeleteMember";
import { Card, Typography } from "@material-tailwind/react";
import ExtendMembership from "./ExtendMembership";
import AddMember from "./AddMember";
import AddLoan from "./AddLoan";
import MemberLoans from "./MemberLoans";

function Members() {
  const TABLE_HEAD = [
    "Card number",
    "Firstname",
    "Lastname",
    "Email",
    "Age",
    "Membership expiration",
    "",
    "",
    "",
  ];
  const [error, setError] = useState(false);
  const [members, setMembers] = useState();
  const [loans, setLoans] = useState();
  function updateMember(
    id,
    newFirstname,
    newLastname,
    newEmail,
    newAge,
    newMembershipExpiration
  ) {
    const updatedMembers = members.map((member) => {
      console.log("Members:");
      console.log(member);
      if (id === member.id) {
        return {
          ...member,
          firstname: newFirstname,
          lastname: newLastname,
          email: newEmail,
          age: newAge,
          MembershipExpiration: newMembershipExpiration,
        };
      }
      return member;
    });
    setMembers(updatedMembers);
  }
  function newMember(firstname, lastname, email, age) {
    const newMember = {
      firstname: firstname,
      lastname: lastname,
      email: email,
      age: age,
    };
    setMembers([...members, newMember]);
  }
  function newLoan(memberId, bookId) {
    const newLoan = {
      memberId: memberId,
      bookId: bookId,
    };
    setLoans([...loans, newLoan]);
  }
  useEffect(() => {
    console.log("Fetching...");
    fetch("https://localhost:7241/api/Member/GetAllMembers", {
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
          setMembers(data);
          console.log(data);
        }
      });
  }, []);
  if (error === true) {
    return (
      <>
        <p>Something went wrong </p>
        <Link to="/Members">Return</Link>
      </>
    );
  }
  return (
    <div data-testid="membersTable">
      {members ? (
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
                {members.map((member) => {
                  return (
                    <tr key={member.id} data-testid={member.id}>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {member.id}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {member.firstname}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-gray">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {member.lastname}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {member.email}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {member.age}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          <time dateTime={member.membershipExpiration}>
                            {member.membershipExpiration}
                          </time>
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <div className="flex-1 py-2">
                          <AddLoan memberId={member.id} newLoan={newLoan} />
                        </div>
                        <div className="flex-1">
                          <MemberLoans id={member.id} />
                        </div>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <div className="flex-1 py-2">
                          <EditMember
                            firstname={member.firstname}
                            lastname={member.lastname}
                            email={member.email}
                            age={member.age}
                            membershipExpiration={member.membershipExpiration}
                            updateMember={updateMember}
                            id={member.id}
                          />
                        </div>
                        <div className="flex-1">
                          <DeleteMember id={member.id} />
                        </div>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <div className="flex-1">
                          <ExtendMembership id={member.id} />
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </Card>
          <div>
            <AddMember newMember={newMember} />
          </div>
        </>
      ) : null}
    </div>
  );
}
export default Members;
