let count = 0;
function counter(button) {
  button.innerHTML = `Click me (${++count})!`;
  console.log(`Clicked ${count} times`);
}

export default counter;
