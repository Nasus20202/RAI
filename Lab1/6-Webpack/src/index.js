import _ from "lodash";
import "./style.css";
import counter from "./counter.js";
import Logo from "./logo.svg";

function logoComponent() {
  const element = document.createElement("div");
  element.classList.add("logo");

  const logo = new Image();
  logo.src = Logo;

  const hideLogoButton = document.createElement("button");
  hideLogoButton.innerHTML = "Hide logo";
  hideLogoButton.onclick = () => {
    const hiddenOpacity = "0.1",
      visibleOpacity = "1";
    if (logo.style.opacity === hiddenOpacity) {
      logo.style.opacity = visibleOpacity;
      hideLogoButton.innerHTML = "Hide logo";
    } else {
      logo.style.opacity = hiddenOpacity;
      hideLogoButton.innerHTML = "Show logo";
    }
  };

  element.appendChild(logo);
  element.appendChild(hideLogoButton);
  return element;
}

function counterComponent() {
  const element = document.createElement("div");

  const counterButton = document.createElement("button");
  counterButton.innerHTML = `Click me!`;
  counterButton.onclick = () => counter(counterButton);

  element.appendChild(counterButton);
  return element;
}

let logoElement = logoComponent();
document.body.appendChild(logoElement);

let counterElement = counterComponent();
document.body.appendChild(counterElement);

if (module.hot) {
  module.hot.accept("./counter.js", () => {
    document.body.removeChild(counterElement);
    counterElement = counterComponent();
    document.body.appendChild(counterElement);
  });
}
