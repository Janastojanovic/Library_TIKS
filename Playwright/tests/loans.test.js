const { test, expect } = require("@playwright/test");

test.describe("Loans Page", () => {
  let page;

  test.beforeEach(async ({ browser }) => {
    page = await browser.newPage();
    await page.goto("http://localhost:3000/loans");
  });

  test("should display loans table", async () => {
    await expect(page.getByTestId("loansTable")).toBeVisible();
  });

  test("should open return modal on button click", async () => {
    await page.click('button:has-text("Return book")');

    //const modal = await page.waitForSelector(".modal", { state: "visible" });
    //expect(modal).not.toBeNull();

    await expect(page.getByTestId("returnModal")).toBeVisible();
  });
  test("should open delete modal on button click", async () => {
    await page.click('button:has-text("Delete")');

    await expect(page.locator("text=Delete loan")).toBeVisible();
    //await expect(page.getByTestId("deleteModal")).toBeVisible();
  });

  test("should delete loan on modal confirmation", async () => {
    // da bih potvrdila da li se pozajmica zaista obrisalla,biram odredjeni red iz tabele(68),pozajmica koja je vracena
    ///trenutni Id,vracena pozjamica:68,69,73,75,77,79,80,81
    //Proveriti da li taj id postoji u trenutnoj tabeli
    const loanTestId = 68;

    await page
      .getByTestId(loanTestId)
      .getByRole("button", { name: "Delete" })
      .click();

    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    const rowsInFirstColumn = await page.evaluate(() => {
      const columns = document.querySelectorAll(
        '[data-testid="loansTable"] tbody tr td:first-child'
      );
      return Array.from(columns).map((column) => column.innerText.trim());
    });

    for (const rowText of rowsInFirstColumn) {
      expect(rowText).not.toEqual(loanTestId.toString());
    }
  });

  test("should get alert when book isn't returned,delete", async () => {
    ////trenutne koje nisu vracene:70,71,72,74,76,78,-
    const loanTestId = 70;

    await page
      .getByTestId(loanTestId)
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

  test("should return loan on modal confirmation", async () => {
    ////trenutne koje nisu vracene:70,71,72,74,76,78,-
    //Proveriti da li je ID validan
    const loanTestId = 74;
    await page
      .getByTestId(loanTestId)
      .getByRole("button", { name: "Return book" })
      .click();

    await page.getByRole("button", { name: "Yes" }).click();

    await page.waitForTimeout(1000);

    const returnCell = await page.$eval(
      `table tbody tr[data-testid="${loanTestId}"] td:nth-child(5)`,
      (cell) => cell.innerText.trim()
    );
    await expect(returnCell).toContain("Yes");
  });

  test("should close return loan modal confirmation", async () => {
    await page.click('button:has-text("Return book")');
    await page.getByRole("button", { name: "No" }).click();

    await expect(page.getByTestId("returnModal")).not.toBeVisible();

    await page.click('button:has-text("Return book")');
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("returnModal")).not.toBeVisible();
  });

  test("should close delete loan modal confirmation", async () => {
    await page.click('button:has-text("Delete")');
    await page.getByRole("button", { name: "No" }).click();

    await expect(page.getByTestId("deleteLoanModal")).not.toBeVisible();

    await page.click('button:has-text("Delete")');
    await page.getByLabel("Close").click();

    await expect(page.getByTestId("deleteLoanModal")).not.toBeVisible();
  });
});
