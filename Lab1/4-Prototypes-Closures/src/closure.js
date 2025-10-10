"use strict";

function Vehicle(id, maxVelocity, velocity) {
  let _id = id;
  let _maxVelocity = maxVelocity;
  let _velocity = velocity;

  this.getId = () => _id;
  this.getMaxVelocity = () => _maxVelocity;
  this.getVelocity = () => _velocity;

  this.status = () =>
    `Vehicle #${this.getId()} | Velocity: ${this.getVelocity()} / ${this.getMaxVelocity()}`;

  this.start = (velocity) => {
    if (velocity < 0) {
      velocity = 0;
    } else if (velocity > this.getMaxVelocity()) {
      velocity = this.getMaxVelocity();
    }
    _velocity = velocity;
  };
}

export default Vehicle;
