"use strict";

function Vehicle(id, maxVelocity, velocity) {
  this.id = id;
  this.maxVelocity = maxVelocity;
  this.velocity = velocity;
}

Vehicle.prototype = {};

Vehicle.prototype.getId = function () {
  return this.id;
};

Vehicle.prototype.getMaxVelocity = function () {
  return this.maxVelocity;
};

Vehicle.prototype.getVelocity = function () {
  return this.velocity;
};

Vehicle.prototype.status = function () {
  return `Vehicle #${this.getId()} | Velocity: ${this.getVelocity()} / ${this.getMaxVelocity()}`;
};

Vehicle.prototype.start = function (velocity) {
  if (velocity < 0) {
    velocity = 0;
  } else if (velocity > this.getMaxVelocity()) {
    velocity = this.getMaxVelocity();
  }
  this.velocity = velocity;
};

export default Vehicle;
