const { test, expect } = require("@playwright/test");

test.describe("Members Page", () => {
  let page;

  test.beforeEach(async ({ browser }) => {
    page = await browser.newPage();
    await page.goto("http://localhost:3000/Members");
  });

  test("should display members table", async () => {
    await expect(page.getByTestId("membersTable")).toBeVisible();
  });

  test("should open edit modal on button click", async () => {
    await page.click('button:has-text("Edit")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("editMemberModal")).toBeVisible();
  });

  test("should open add member modal on button click", async () => {
    await page.click('button:has-text("+ Add Member")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("addMemberModal")).toBeVisible();
  });

  test("should open add loan modal on button click", async () => {
    await page.click('button:has-text("+ Add Loan")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("addLoanModal")).toBeVisible();
  });
  test("should open member loans modal on button click", async () => {
    await page.click('button:has-text("Member loans")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();
  });
  test("should open extend membership modal on button click", async () => {
    await page.click('button:has-text("extend membership")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("extendMembershipModal")).toBeVisible();
  });
  test("should open delete modal on button click", async () => {
    await page.click('button:has-text("Delete")');

    await expect(page.locator("text=Delete member")).toBeVisible();
    //await expect(page.getByTestId("deleteBookModal")).toBeVisible();
  });
  ////////////////////////////////////////////////////////////////////////////////////////////////
  test("should close delete member modal", async () => {
    await page.click('button:has-text("Delete")');
    await expect(page.getByTestId("deleteMemberModal")).toBeVisible();
    await page.getByRole("button", { name: "No" }).click();

    await expect(page.getByTestId("deleteMemberModal")).not.toBeVisible();

    await page.click('button:has-text("Delete")');
    await expect(page.getByTestId("deleteMemberModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("deleteMemberModal")).not.toBeVisible();
  });

  test("should close add member modal", async () => {
    await page.click('button:has-text("+ Add Member")');
    await expect(page.getByTestId("addMemberModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("addMemberModal")).not.toBeVisible();

    await page.click('button:has-text("+ Add Member")');
    await expect(page.getByTestId("addMemberModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("addMemberModal")).not.toBeVisible();
  });

  test("should close edit member modal", async () => {
    await page.click('button:has-text("Edit")');
    await expect(page.getByTestId("editMemberModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("editMemberModal")).not.toBeVisible();

    await page.click('button:has-text("Edit")');
    await expect(page.getByTestId("editMemberModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("editMemberModal")).not.toBeVisible();
  });
  test("should close extend membership modal", async () => {
    await page.click('button:has-text("Extend membership")');
    await expect(page.getByTestId("extendMembershipModal")).toBeVisible();
    await page.getByRole("button", { name: "No" }).click();

    await expect(page.getByTestId("extendMembershipModal")).not.toBeVisible();

    await page.click('button:has-text("Extend membership")');
    await expect(page.getByTestId("extendMembershipModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("extendMembershipModal")).not.toBeVisible();
  });
  test("should close add loan modal", async () => {
    await page.click('button:has-text("+ Add Loan")');
    await expect(page.getByTestId("addLoanModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("addLoanModal")).not.toBeVisible();

    await page.click('button:has-text("+ Add Loan")');
    await expect(page.getByTestId("addLoanModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("addLoanModal")).not.toBeVisible();
  });
  test("should close member loans modal", async () => {
    await page.click('button:has-text("Member loans")');
    await expect(page.getByTestId("memberLoansModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("memberLoansModal")).not.toBeVisible();

    await page.click('button:has-text("Member loans")');
    await expect(page.getByTestId("memberLoansModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("memberLoansModal")).not.toBeVisible();
  });
  //////////////////////////////////////////////////////////////////////
  test("should delete member on modal confirmation", async () => {
    // da bih potvrdila da li je clan zaista obrisan,biram odredjeni red iz tabele,tj.ID(25), ali koji trenutno nema pozajmice
    //Proveriti da li taj id postoji u trenutnoj tabeli
    /////validni Id: 26,27,24,23,22,21
    const testIdMember = 25;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Delete" })
      .click();
    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    const rowsInFirstColumn = await page.evaluate(() => {
      const columns = document.querySelectorAll(
        '[data-testid="membersTable"] tbody tr td:first-child'
      );
      return Array.from(columns).map((column) => column.innerText.trim());
    });

    for (const rowText of rowsInFirstColumn) {
      expect(rowText).not.toEqual(testIdMember.toString());
    }
  });

  test("should get alert when member has active loans,delete", async () => {
    ///test id:18,19,20
    const testIdMember = 18;
    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Delete" })
      .click();

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain(
        "Can't delete member,books are not returned."
      );
      await dialog.accept();
    });

    await page.getByRole("button", { name: "Yes" }).click();
    await page.waitForTimeout(1000);
  });

  ///////////////////////////////////////////////////
  test("should edit member on modal confirmation", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("Marina");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("28");

    await page
      .getByTestId("editMemberModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);

    const nameCell = await page.$eval(
      `table tbody tr[data-testid="${testIdMember}"] td:nth-child(2)`,
      (cell) => cell.innerText.trim()
    );
    await expect(nameCell).toContain("Marina");

    const surnameCell = await page.$eval(
      `table tbody tr[data-testid="${testIdMember}"] td:nth-child(3)`,
      (cell) => cell.innerText.trim()
    );
    await expect(surnameCell).toContain("Milic");

    const emailCell = await page.$eval(
      `table tbody tr[data-testid="${testIdMember}"] td:nth-child(4)`,
      (cell) => cell.innerText.trim()
    );
    await expect(emailCell).toContain("marina22@gmail.com");

    const ageCell = await page.$eval(
      `table tbody tr[data-testid="${testIdMember}"] td:nth-child(5)`,
      (cell) => cell.innerText.trim()
    );
    await expect(ageCell).toContain("28");
  });

  test("should get alert when title or author is empty, edit ", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("25");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain(
        "Firstname and lastname fields are required"
      );
      await dialog.accept();
    });

    await page
      .getByTestId("editMemberModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should get alert when age is 0, edit", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("Marina");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("0");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Invalid age");
      await dialog.accept();
    });

    await page
      .getByTestId("editMemberModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);
  });
  //////////////////////////////////////////////////////////////////////////////
  test("should add member on modal confirmation", async () => {
    const rowsBeforeAddingMember = await page.$$eval(
      '[data-testid="membersTable"] tbody tr',
      (rows) => rows.length
    );

    await page.click('button:has-text("+ Add Member")');

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("Marina");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("25");

    await page
      .getByTestId("addMemberModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);

    const rowsAfterAddingMember = await page.$$eval(
      '[data-testid="membersTable"] tbody tr',
      (rows) => rows.length
    );
    if (rowsAfterAddingMember > rowsBeforeAddingMember) {
      console.log("Novi red je dodat.");
    } else {
      console.log("Novi red nije dodat.");
    }

    const nameCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(2)`,
      (cell) => cell.innerText.trim()
    );
    await expect(nameCell).toContain("Marina");

    const surnameCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(3)`,
      (cell) => cell.innerText.trim()
    );
    await expect(surnameCell).toContain("Milic");

    const emailCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(4)`,
      (cell) => cell.innerText.trim()
    );
    await expect(emailCell).toContain("marina22@gmail.com");

    const ageCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(5)`,
      (cell) => cell.innerText.trim()
    );
    await expect(ageCell).toContain("25");
  });

  test("should get alert when title or author is empty, add", async () => {
    await page.click('button:has-text("+ Add Member")');

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("25");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain(
        "Firstname and lastname fields are required"
      );
      await dialog.accept();
    });

    await page
      .getByTestId("addMemberModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);
  });
  test("should get alert when age is 0, add", async () => {
    await page.click('button:has-text("+ Add Member")');

    await page.getByLabel("Firstname").click();
    await page.getByLabel("Firstname").fill("Marina");
    await page.getByLabel("Lastname").click();
    await page.getByLabel("Lastname").fill("Milic");
    await page.getByLabel("Email").click();
    await page.getByLabel("Email").fill("marina22@gmail.com");
    await page.getByLabel("Age").click();
    await page.getByLabel("Age").fill("0");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Invalid age");
      await dialog.accept();
    });

    await page
      .getByTestId("addMemberModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should extend membership for member on modal confirmation", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Extend membership" })
      .click();

    await page.getByRole("button", { name: "Yes" }).click();
    await page.waitForTimeout(1000);

    const todayDate = getFormattedDate(1);

    const membershipExpirationCell = await page.$eval(
      `table tbody tr[data-testid="${testIdMember}"] td:nth-child(6)`,
      (cell) => cell.innerText.trim()
    );

    expect(membershipExpirationCell).toContain(todayDate);
  });
  ///////////////////////////////////////////////////////////

  test("should add loan on modal confirmation", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;
    //Staviti validan ID
    ////trenutni Id:25-32
    const existingBookID = 26;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();
    const rowsBeforeAddingLoan = await page.$$eval(
      '[data-testid="memberLoansTable"] tbody tr',
      (rows) => rows.length
    );

    await page.click('button:has-text("Close")');

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "+ Add Loan" })
      .click();

    await page.getByPlaceholder("Book ID").click();
    await page.getByPlaceholder("Book ID").fill(existingBookID.toString());

    await page
      .getByTestId("addLoanModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();

    const rowsAfterAddingLoan = await page.$$eval(
      '[data-testid="memberLoansTable"] tbody tr',
      (rows) => rows.length
    );

    if (rowsAfterAddingLoan > rowsBeforeAddingLoan) {
      console.log("Novi red je dodat.");
    } else {
      console.log("Novi red nije dodat.");
    }

    const lastRow = await page.$(
      '[data-testid="memberLoansTable"] tbody tr:last-child'
    );

    const lastRowText = await lastRow.innerText();
    console.log(lastRowText);

    const todayDate = getFormattedDate(0);

    expect(
      lastRowText.includes("(" + existingBookID.toString() + ")")
    ).toBeTruthy(); // Proverava da li tekst sadrži podstring "(existingBookID)",jer je u zagradama bookID
    expect(lastRowText.includes(todayDate)).toBeTruthy(); // Proverava da li tekst sadrži današnji datum
    expect(lastRowText.includes("No")).toBeTruthy(); // Proverava da li tekst sadrži "No" za returned,default je no
  });

  test("should get alert when invalid book ID, add loan", async () => {
    //Staviti validan ID
    ////trenutni Id:18-27
    const testIdMember = 20;
    //validani bookID
    ////trenutni Id:25-32
    const notExistingBookID = 2;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "+ Add Loan" })
      .click();

    await page.getByPlaceholder("Book ID").click();
    await page.getByPlaceholder("Book ID").fill(notExistingBookID.toString());

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Invalid book ID");
      await dialog.accept();
    });

    await page
      .getByTestId("addLoanModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);
  });
  //////////////////////////////////////////////////////////////////////

  test("should get No loans available,member loans", async () => {
    ///////Za clana koji jos uvek nema pozajmice
    //trenutni koji nemaju 21,22,23,24,26,27
    const testIdMember = 27;
    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.locator("text=No loans available")).toBeVisible();
  });

  test("should return book,member loans", async () => {
    //18-70,71
    //19-72,74
    //20-76,78
    const testIdLoan = 70;
    const testIdMember = 18;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();

    await page
      .getByTestId(testIdLoan)
      .getByRole("button", { name: "Return book" })
      .click();

    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();

    const returnedCell = await page.$eval(
      `table tbody tr[data-testid="${testIdLoan}"] td:nth-child(4)`,
      (cell) => cell.innerText.trim()
    );
    expect(returnedCell).toContain("Yes");
  });

  test("should delete loan,member loans", async () => {
    //////Kada je knjiga vracena
    //////18-68,69
    //////19-73,75,77
    //////20-nijedna
    //////25-79,80,81
    const testIdLoan = 58;
    const testIdMember = 8;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await expect(page.getByTestId("memberLoansModal")).toBeVisible();

    await page
      .getByTestId(testIdLoan)
      .getByRole("button", { name: "Delete" })
      .click();

    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    const rowWithTestId = await page.$(
      `table tbody tr[data-testid="${testIdLoan}"]`
    );
    await expect(rowWithTestId).toBeNull();
    //await expect(rowWithTestId).toBeVisible();
  });

  test("should get alert when book isn't returned,delete", async () => {
    //////Kada nije knjiga vracena
    //18-70,71
    //19-72,74
    //20-76,78
    const testIdLoan = 56;
    const testIdMember = 8;

    await page
      .getByTestId(testIdMember)
      .getByRole("button", { name: "Member loans" })
      .click();

    await page
      .getByTestId(testIdLoan)
      .getByRole("button", { name: "Delete" })
      .click();

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain(
        "Can't delete loan,book isn't returnd"
      );
      await dialog.accept();
    });

    await page.getByRole("button", { name: "Yes" }).click();
    await page.waitForTimeout(1000);
  });
});

function getFormattedDate(addedYears) {
  const today = new Date();
  const year = today.getFullYear() + addedYears;
  let month = today.getMonth() + 1;
  if (month < 10) {
    month = "0" + month;
  }
  let day = today.getDate();
  if (day < 10) {
    day = "0" + day;
  }
  return `${year}-${month}-${day}`;
}
