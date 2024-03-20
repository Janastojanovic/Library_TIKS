import "./App.css";
import Books from "./components/Books";
import Header from "./components/Header";
import Loans from "./components/Loans";
import Members from "./components/Members";
import { BrowserRouter, Routes, Route } from "react-router-dom";

function App() {
  return (
    <BrowserRouter>
      <Header>
        <Routes>
          <Route path="/members" element={<Members />} />
          <Route path="/books" element={<Books />} />
          <Route path="/loans" element={<Loans />} />
        </Routes>
      </Header>
    </BrowserRouter>
  );
}

export default App;
