import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import "bootstrap/dist/css/bootstrap.min.css";
import { Card, Typography } from "@material-tailwind/react";
import EditBook from "./EditBook";
import DeleteBook from "./DeleteBook";
import AddBook from "./AddBook";

function Books() {
  const TABLE_HEAD = [
    "ID",
    "Title",
    "Author",
    "Descrition",
    "Category",
    "Number of copies",
    "Available",
    "",
  ];
  const [error, setError] = useState(false);
  const [books, setBooks] = useState();
  function updateBook(
    id,
    newTitle,
    newAuthor,
    newDescription,
    newCategory,
    newNumberOfCopies,
    newAvailable
  ) {
    const updatedBooks = books.map((book) => {
      if (id === book.id) {
        return {
          ...book,
          title: newTitle,
          author: newAuthor,
          description: newDescription,
          category: newCategory,
          numberOfCopies: newNumberOfCopies,
          available: newAvailable,
        };
      }
      return book;
    });
    setBooks(updatedBooks);
  }
  function newBook(title, author, description,category,numberOfCopies,available) {
    const newBook = {
        title: title,
        author: author,
        description: description,
        category: category,
        numberOfCopies:numberOfCopies,
        available:available,
    };
    setBooks([...books, newBook]);
  }
  useEffect(() => {
    fetch("https://localhost:7241/api/Book/GetAllBooks", {
      headers: {
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (response.status !== 200) {
          setError(true);
        } else {
          return response.json();
        }
      })
      .then((data) => {
        if (error === false) {
          setBooks(data);
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
    <div >
      {books ? (
        <>
          <Card className="h-full w-full overflow-scroll">
            <table className="w-full min-w-max table-auto text-left" data-testid="booksTable">
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
                {books.map((book) => {
                  return (
                    <tr key={book.id} data-testid={book.id}>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.id}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.title}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.author}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.description}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.category}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.numberOfCopies}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <Typography
                          variant="small"
                          color="blue-gray"
                          className="font-normal"
                        >
                          {book.available}
                        </Typography>
                      </td>
                      <td className="p-4 border-b border-blue-gray-50 bg-blue-gray-50/50">
                        <div className="flex">
                          <div className="flex-1">
                            <EditBook
                              title={book.title}
                              author={book.author}
                              description={book.description}
                              category={book.category}
                              numberOfCopies={book.numberOfCopies}
                              available={book.available}
                              updateBook={updateBook}
                              id={book.id}
                            />
                          </div>
                          <div className="flex-1">
                            <DeleteBook id={book.id} />
                          </div>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </Card>
          <div>
          <AddBook
            newBook={newBook}
            />
          </div>
        </>
      ) : null}
    </div>
  );
}
export default Books;
