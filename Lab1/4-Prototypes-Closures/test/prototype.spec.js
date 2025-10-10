"use strict";

import { expect } from "chai";
import Vehicle from "../src/prototype.js";

describe("Vehicle test | Prototype", () => {
  it("should create a vehicle with the given properties", () => {
    // Arrange
    const id = 1,
      maxVelocity = 200,
      velocity = 0;

    // Act
    const vehicle = new Vehicle(id, maxVelocity, velocity);

    // Assert
    expect(vehicle.getId()).to.equal(id);
    expect(vehicle.getMaxVelocity()).to.equal(maxVelocity);
    expect(vehicle.getVelocity()).to.equal(velocity);
  });

  it("should start the vehicle with a valid velocity", () => {
    // Arrange
    const vehicle = new Vehicle(1, 200, 0);
    const newVelocity = 100;

    // Act
    vehicle.start(newVelocity);

    // Assert
    expect(vehicle.getVelocity()).to.equal(newVelocity);
  });

  it("should not start the vehicle with a negative velocity", () => {
    // Arrange
    const vehicle = new Vehicle(1, 200, 0);
    const newVelocity = -50;

    // Act
    vehicle.start(newVelocity);

    // Assert
    expect(vehicle.getVelocity()).to.equal(0);
  });

  it("should not start the vehicle with a velocity greater than maxVelocity", () => {
    // Arrange
    const maxVelocity = 200;
    const vehicle = new Vehicle(1, maxVelocity, 0);
    const newVelocity = 250;

    // Act
    vehicle.start(newVelocity);

    // Assert
    expect(vehicle.getVelocity()).to.equal(maxVelocity);
  });

  it("should return the correct status string", () => {
    // Arrange
    const id = 1,
      maxVelocity = 200,
      velocity = 100;
    const vehicle = new Vehicle(id, maxVelocity, velocity);
    const expectedStatus = `Vehicle #${id} | Velocity: ${velocity} / ${maxVelocity}`;

    // Act
    const status = vehicle.status();

    // Assert
    expect(status).to.equal(expectedStatus);
  });

  it("should expose only vehicle properties", () => {
    // Arrange
    const vehicle = new Vehicle(1, 200, 0);
    const members = Object.keys(vehicle);

    // Assert
    expect(members).to.have.members(["id", "maxVelocity", "velocity"]);
  });

  it("should have methods in the prototype", () => {
    // Arrange
    const vehicle = new Vehicle(1, 200, 0);
    const prototypeMembers = Object.getOwnPropertyNames(
      Object.getPrototypeOf(vehicle)
    );

    // Assert
    expect(prototypeMembers).to.have.members([
      "getId",
      "getMaxVelocity",
      "getVelocity",
      "status",
      "start",
    ]);
  });

  it("should propagate methods from the prototype", () => {
    // Arrange
    const test = function () {
      return "test";
    };
    const vehicle = new Vehicle(1, 200, 0);
    const prototype = Object.getPrototypeOf(vehicle);

    // Act
    prototype.test = test;

    // Assert
    expect(vehicle.test).to.equal(test);
  });
});
