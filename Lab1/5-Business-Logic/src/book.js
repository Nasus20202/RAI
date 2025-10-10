"use strict";

class Book {
  constructor(author, title, price, publisher, tags) {
    this.author = author;
    this.title = title;
    this.price = price;
    this.publisher = publisher;
    this.tags = tags;
  }

  toString() {
    return `${this.title} by ${this.author}`; // 6. String interpolation
  }
}

export default Book;
