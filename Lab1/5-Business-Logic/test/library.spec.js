"use strict";

import { expect } from "chai";
import Library from "../src/library.js";
import Book from "../src/book.js";

describe("Library", function () {
  let library; // 3. Block-scoped variable

  beforeEach(function () {
    library = new Library();
  });

  const createBook = (title, tags = ["fiction"]) =>
    new Book(`Author of ${title}`, title, 10_000, "Publisher", tags); // 11. Numeric separator

  it("should return an empty array when there are no rentals", function () {
    // Act
    const rentals = library.getRentals();

    // Assert
    expect(rentals).to.be.an("array").that.is.empty;
  });

  it("should add a rental when a book is rented", function () {
    // Arrange
    const book = createBook("Title");
    const borrower = "John Doe";

    // Act
    library.rentBook(book, borrower);
    const rentals = library.getRentals();

    // Assert
    expect(rentals).to.have.lengthOf(1);
    expect(rentals[0]).to.include({
      book,
      borrower,
      returned: false,
    });
  });

  it("should return a rental when a book is returned", function () {
    // Arrange
    const book = createBook("Title");
    library.rentBook(book, "John Doe");

    // Act
    const rental = library.returnBook(book);

    // Assert
    expect(rental).to.exist;
    expect(rental.returned).to.be.true;
  });

  it("should return undefined when returning a book that was not rented", function () {
    // Arrange
    const book = createBook("Title");

    // Act
    const rental = library.returnBook(book);

    // Assert
    expect(rental).to.be.undefined;
  });

  it("should return undefined when returning a book that was already returned", function () {
    // Arrange
    const book = createBook("Title");
    library.rentBook(book, "John Doe");
    library.returnBook(book);

    // Act
    const rental = library.returnBook(book);

    // Assert
    expect(rental).to.be.undefined;
  });

  it("should return a rental when a book is rented after being returned", function () {
    // Arrange
    const book = createBook("Title");
    const borrower = "John Doe";
    library.rentBook(book, borrower);
    library.returnBook(book);

    // Act
    library.rentBook(book, borrower);
    const rentals = library.getRentals();

    // Assert
    expect(rentals).to.have.lengthOf(2);
    expect(rentals[1]).to.include({
      book,
      borrower,
      returned: false,
    });
  });

  it("should get active rental by book", function () {
    // Arrange
    const book1 = createBook("Book 1");
    const book2 = createBook("Book 2");
    library.rentBook(book1, "John Doe");

    // Act
    const activeRental1 = library.getActiveRentalByBook(book1);
    const activeRental2 = library.getActiveRentalByBook(book2);

    // Assert
    expect(activeRental1).to.exist.and.include({
      book: book1,
      borrower: "John Doe",
    });
    expect(activeRental2).to.be.undefined;
  });

  it("should get rentals by borrower", function () {
    // Arrange
    const book1 = createBook("Book 1");
    const book2 = createBook("Book 2");
    library.rentBook(book1, "John Doe");
    library.rentBook(book2, "John Doe");
    library.rentBook(book1, "Jane Smith");

    // Act
    const rentalsByJohn = library.getRentalsByBorrower("John Doe");
    const rentalsByJane = library.getRentalsByBorrower("Jane Smith");

    // Assert
    expect(rentalsByJohn).to.have.lengthOf(2);
    expect(rentalsByJane).to.have.lengthOf(1);
  });

  it("should get rentals by book tags", function () {
    // Arrange
    const book1 = createBook("Book 1", ["fiction", "adventure"]);
    const book2 = createBook("Book 2", ["non-fiction", "history"]);
    const book3 = createBook("Book 3", ["fiction", "mystery"]);
    library.rentBook(book1, "John Doe");
    library.rentBook(book2, "John Doe");
    library.rentBook(book3, "John Doe");

    // Act
    const fictionRentals = library.getRentalsByBookTags(["fiction"]);
    const historyRentals = library.getRentalsByBookTags(["history"]);
    const adventureAndMysteryRentals = library.getRentalsByBookTags([
      "adventure",
      "mystery",
    ]);

    // Assert
    expect(fictionRentals).to.have.lengthOf(2);
    expect(historyRentals).to.have.lengthOf(1);
    expect(adventureAndMysteryRentals).to.have.lengthOf(2);
  });
});
