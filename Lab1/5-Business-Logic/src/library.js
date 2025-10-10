"use strict";

// 1. Class
class Rental {
  constructor(book, borrower = undefined) {
    // 4. Default parameter
    this.book = book;
    this.borrower = borrower;
    this.returned = false;
  }
}

class Library {
  constructor(rentals = []) {
    this.rentals = rentals;
  }

  #addRentals(rentals) {
    // 12. Private class method
    this.rentals = [...this.rentals, ...rentals]; // 5. Spread operator
  }

  getRentals() {
    return this.rentals;
  }

  getActiveRentalByBook = (book) =>
    this.rentals.find((rental) => rental.book === book && !rental.returned);

  getRentalsByBorrower(borrower) {
    return this.rentals.filter((rental) => rental?.borrower === borrower); // 9. Optional chaining
  }

  getRentalsByBookTags(tags) {
    return this.rentals.filter(
      (
        rental // 2. Anonymous function with arrow syntax
      ) => rental.book.tags.some((tag) => tags.includes(tag))
    );
  }

  rentBook(book, borrower) {
    const rental = new Rental(book, borrower);
    this.rentals.push(rental);
  }

  returnBook(book) {
    const rental = this.getActiveRentalByBook(book);
    if (rental) {
      rental.returned = true;
      return rental;
    }
    return undefined;
  }
}

export default Library;
