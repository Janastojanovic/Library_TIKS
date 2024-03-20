const { test, expect } = require("@playwright/test");

test.describe("Books Page", () => {
  let page;

  test.beforeEach(async ({ browser }) => {
    page = await browser.newPage();
    await page.goto("http://localhost:3000/Books");
  });

  test("should display books table", async () => {
    await expect(page.getByTestId("booksTable")).toBeVisible();
  });

  test("should open edit modal on button click", async () => {
    await page.click('button:has-text("Edit")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("editBookModal")).toBeVisible();
  });

  test("should open add modal on button click", async () => {
    await page.click('button:has-text("+ Add Book")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("addBookModal")).toBeVisible();
  });

  test("should open delete modal on button click", async () => {
    await page.click('button:has-text("Delete")');

    await expect(page.locator("text=Delete book")).toBeVisible();
    //await expect(page.getByTestId("deleteBookModal")).toBeVisible();
  });

  test("should delete book on modal confirmation", async () => {
    // da bih potvrdila da li se knjiga zaista obrisalla,biram odredjeni red iz tabele,tj.ID(31), ali koja nema trenutno pozajmice
    //Proveriti da li taj id postoji u trenutnoj tabeli
    ///validni booId za brisanje: 31,32
    const bookIdTest = 31;

    await page
      .getByTestId(bookIdTest)
      .getByRole("button", { name: "Delete" })
      .click();
    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    const rowsInFirstColumn = await page.evaluate(() => {
      const columns = document.querySelectorAll(
        '[data-testid="booksTable"] tbody tr td:first-child'
      );
      return Array.from(columns).map((column) => column.innerText.trim());
    });

    for (const rowText of rowsInFirstColumn) {
      expect(rowText).not.toEqual(bookIdTest.toString());
    }
  });

  test("should close delete book modal", async () => {
    await page.click('button:has-text("Delete")');
    await expect(page.getByTestId("deleteBookModal")).toBeVisible();
    await page.getByRole("button", { name: "No" }).click();

    await expect(page.getByTestId("deleteBookModal")).not.toBeVisible();

    await page.click('button:has-text("Delete")');
    await expect(page.getByTestId("deleteBookModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("deleteBookModal")).not.toBeVisible();
  });

  test("should close add book modal", async () => {
    await page.click('button:has-text("+ Add Book")');
    await expect(page.getByTestId("addBookModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("addBookModal")).not.toBeVisible();

    await page.click('button:has-text("+ Add Book")');
    await expect(page.getByTestId("addBookModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("addBookModal")).not.toBeVisible();
  });

  test("should close edit book modal", async () => {
    await page.click('button:has-text("Edit")');
    await expect(page.getByTestId("editBookModal")).toBeVisible();
    await page.click('button:has-text("Close")');

    await expect(page.getByTestId("editBookModal")).not.toBeVisible();

    await page.click('button:has-text("Edit")');
    await expect(page.getByTestId("editBookModal")).toBeVisible();
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("editBookModal")).not.toBeVisible();
  });

  test("should edit book on modal confirmation", async () => {
    //Staviti validan ID
    ////trenutni 25-32
    const bookIdTest = 27;
    await page
      .getByTestId(bookIdTest)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Title").click();
    await page.getByLabel("Title").fill("Nesto");
    await page.getByLabel("Author").click();
    await page.getByLabel("Author").fill("neko");
    await page.getByLabel("Description").click();
    await page.getByLabel("Description").fill("blabla");
    await page.getByLabel("Category").click();
    await page.getByLabel("Category").fill("roman");
    await page.getByLabel("Number of copies").click();
    await page.getByLabel("Number of copies").fill("20");

    await page
      .getByTestId("editBookModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);

    const titleCell = await page.$eval(
      `table tbody tr[data-testid="${bookIdTest}"] td:nth-child(2)`,
      (cell) => cell.innerText.trim()
    );
    await expect(titleCell).toContain("Nesto");

    const authorCell = await page.$eval(
      `table tbody tr[data-testid="${bookIdTest}"] td:nth-child(3)`,
      (cell) => cell.innerText.trim()
    );
    await expect(authorCell).toContain("neko");

    const descriptionCell = await page.$eval(
      `table tbody tr[data-testid="${bookIdTest}"] td:nth-child(4)`,
      (cell) => cell.innerText.trim()
    );
    await expect(descriptionCell).toContain("blabla");

    const categoryCell = await page.$eval(
      `table tbody tr[data-testid="${bookIdTest}"] td:nth-child(5)`,
      (cell) => cell.innerText.trim()
    );
    await expect(categoryCell).toContain("roman");

    const copiesCell = await page.$eval(
      `table tbody tr[data-testid="${bookIdTest}"] td:nth-child(6)`,
      (cell) => cell.innerText.trim()
    );
    await expect(copiesCell).toContain("20");
  });

  test("should get alert when numofcopies is 0 , edit", async () => {
     //Staviti validan ID
    ////trenutni 25-32
    const bookIdTest = 27;
    await page
      .getByTestId(bookIdTest)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Number of copies").click();
    await page.getByLabel("Number of copies").fill("0");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Number of copies can't be 0");
      await dialog.accept();
    });

    await page
      .getByTestId("editBookModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should get alert when title or author is empty, edit ", async () => {
     //Staviti validan ID
    ////trenutni 25-32
    const bookIdTest = 27;
    await page
      .getByTestId(bookIdTest)
      .getByRole("button", { name: "Edit" })
      .click();

    await page.getByLabel("Title").click();
    await page.getByLabel("Title").fill("");
    await page.getByLabel("Author").click();
    await page.getByLabel("Author").fill("neko");
    await page.getByLabel("Description").click();
    await page.getByLabel("Description").fill("blabla");
    await page.getByLabel("Category").click();
    await page.getByLabel("Category").fill("roman");
    await page.getByLabel("Number of copies").click();
    await page.getByLabel("Number of copies").fill("20");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Title and Author are required");
      await dialog.accept();
    });

    await page
      .getByTestId("editBookModal")
      .getByRole("button", { name: "Edit" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should get alert when book has active loans,delete", async () => {
    ////trenutni 25-30 koji imaju pozajmice
    const bookIdTest = 27;

    await page
      .getByTestId(bookIdTest)
      .getByRole("button", { name: "Delete" })
      .click();

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain(
        "Can't delete book,there are loans for this book that are not returned"
      );
      await dialog.accept();
    });

    await page.getByRole("button", { name: "Yes" }).click();
    await page.waitForTimeout(1000);
  });

  test("should get alert when title or author is empty, add", async () => {
    await page.getByRole("button", { name: "+ Add Book" }).click();

    await page.getByPlaceholder("Harry Poter").click();
    await page.getByPlaceholder("Harry Poter").fill("Nesto");
    await page.getByPlaceholder("J.K.R.").click();
    await page.getByPlaceholder("J.K.R.").fill("");
    await page.getByPlaceholder("blah blah").click();
    await page.getByPlaceholder("blah blah").fill("blabla");
    await page.getByPlaceholder("fantasy").click();
    await page.getByPlaceholder("fantasy").fill("roman");
    await page.getByPlaceholder("22").click();
    await page.getByPlaceholder("22").fill("10");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Title and Author are required");
      await dialog.accept();
    });

    await page
      .getByTestId("addBookModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should get alert when numOfCopies is 0, add", async () => {
    await page.getByRole("button", { name: "+ Add Book" }).click();

    await page.getByPlaceholder("Harry Poter").click();
    await page.getByPlaceholder("Harry Poter").fill("Nesto");
    await page.getByPlaceholder("J.K.R.").click();
    await page.getByPlaceholder("J.K.R.").fill("Neko");
    await page.getByPlaceholder("blah blah").click();
    await page.getByPlaceholder("blah blah").fill("blabla");
    await page.getByPlaceholder("fantasy").click();
    await page.getByPlaceholder("fantasy").fill("roman");
    await page.getByPlaceholder("22").click();
    await page.getByPlaceholder("22").fill("0");

    page.on("dialog", async (dialog) => {
      console.log(await dialog.message());
      expect(dialog.type()).toContain("alert");
      expect(dialog.message()).toContain("Number of copies can't be 0");
      await dialog.accept();
    });

    await page
      .getByTestId("addBookModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);
  });

  test("should add book on modal confirmation", async () => {
    const rowsBeforeAddingBook = await page.$$eval(
      '[data-testid="booksTable"] tbody tr',
      (rows) => rows.length
    );

    await page.getByRole("button", { name: "+ Add Book" }).click();

    await page.getByPlaceholder("Harry Poter").click();
    await page.getByPlaceholder("Harry Poter").fill("Nesto");
    await page.getByPlaceholder("J.K.R.").click();
    await page.getByPlaceholder("J.K.R.").fill("Neko");
    await page.getByPlaceholder("blah blah").click();
    await page.getByPlaceholder("blah blah").fill("blabla");
    await page.getByPlaceholder("fantasy").click();
    await page.getByPlaceholder("fantasy").fill("roman");
    await page.getByPlaceholder("22").click();
    await page.getByPlaceholder("22").fill("20");

    await page
      .getByTestId("addBookModal")
      .getByRole("button", { name: "Add" })
      .click();

    await page.waitForTimeout(1000);

    const rowsAfterAddingBook = await page.$$eval(
      '[data-testid="booksTable"] tbody tr',
      (rows) => rows.length
    );
    if (rowsAfterAddingBook > rowsBeforeAddingBook) {
      console.log("Novi red je dodat.");
    } else {
      console.log("Novi red nije dodat.");
    }

    const titleCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(2)`,
      (cell) => cell.innerText.trim()
    );
    await expect(titleCell).toContain("Nesto");

    const authorCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(3)`,
      (cell) => cell.innerText.trim()
    );
    await expect(authorCell).toContain("Neko");

    const descriptionCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(4)`,
      (cell) => cell.innerText.trim()
    );
    await expect(descriptionCell).toContain("blabla");

    const categoryCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(5)`,
      (cell) => cell.innerText.trim()
    );
    await expect(categoryCell).toContain("roman");

    const copiesCell = await page.$eval(
      `table tbody tr:last-child td:nth-child(6)`,
      (cell) => cell.innerText.trim()
    );
    await expect(copiesCell).toContain("20");
  });
});
