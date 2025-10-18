const API_BASE = "/api/bookings";
const ROOMS_API = "/api/rooms";
let currentDate = new Date();
let currentView = "day"; // 'day' or 'week'
let rooms = [];
let bookingModal;
let selectedRoom = "";

// Initialize
document.addEventListener("DOMContentLoaded", async () => {
  bookingModal = new bootstrap.Modal(document.getElementById("bookingModal"));
  setupEventListeners();

  // Fetch rooms from API
  try {
    const response = await fetch(ROOMS_API);
    if (!response.ok) {
      throw new Error("Failed to fetch rooms");
    }
    rooms = await response.json();
    await loadCalendar();
  } catch (error) {
    console.error("Error loading rooms:", error);
    document.getElementById("calendarContainer").innerHTML =
      '<div class="alert alert-danger">Failed to load rooms. Please refresh the page.</div>';
    document.getElementById("loadingSpinner").style.display = "none";
    document.getElementById("calendarContainer").style.display = "block";
  }
});

function setupEventListeners() {
  document
    .getElementById("dayView")
    .addEventListener("click", () => switchView("day"));
  document
    .getElementById("weekView")
    .addEventListener("click", () => switchView("week"));
  document
    .getElementById("prevBtn")
    .addEventListener("click", () => navigate(-1));
  document
    .getElementById("todayBtn")
    .addEventListener("click", () => navigateToToday());
  document
    .getElementById("nextBtn")
    .addEventListener("click", () => navigate(1));
  document
    .getElementById("confirmBooking")
    .addEventListener("click", createBookingFromModal);
}

function switchView(view) {
  currentView = view;
  document.getElementById("dayView").classList.toggle("active", view === "day");
  document
    .getElementById("weekView")
    .classList.toggle("active", view === "week");
  loadCalendar();
}

function navigate(direction) {
  if (currentView === "day") {
    currentDate.setDate(currentDate.getDate() + direction);
  } else {
    currentDate.setDate(currentDate.getDate() + direction * 7);
  }
  loadCalendar();
}

function navigateToToday() {
  currentDate = new Date();
  loadCalendar();
}

async function loadCalendar() {
  const loadingSpinner = document.getElementById("loadingSpinner");
  const calendarContainer = document.getElementById("calendarContainer");

  loadingSpinner.style.display = "block";
  calendarContainer.style.display = "none";

  try {
    if (currentView === "day") {
      await renderDayView();
    } else {
      await renderWeekView();
    }

    updateDateDisplay();
  } catch (error) {
    console.error("Error loading calendar:", error);
    calendarContainer.innerHTML =
      '<div class="alert alert-danger">Failed to load calendar</div>';
  } finally {
    loadingSpinner.style.display = "none";
    calendarContainer.style.display = "block";
  }
}

async function renderDayView() {
  const dateStr = currentDate.toISOString().split("T")[0];
  const response = await fetch(`${API_BASE}/day/${dateStr}`);
  const bookings = await response.json();

  const calendarContainer = document.getElementById("calendarContainer");
  let html = '<table class="table table-bordered table-hover">';
  html += '<thead class="table-light"><tr><th style="width: 100px;">Time</th>';

  // Room headers
  rooms.forEach((room) => {
    html += `<th class="text-center">${room.name}<br><small class="text-muted">Capacity: ${room.capacity}</small></th>`;
  });
  html += "</tr></thead><tbody>";

  // Time slots (0:00 - 23:00)
  for (let hour = 0; hour < 24; hour++) {
    html += "<tr>";
    html += `<td class="fw-bold">${hour}:00</td>`;

    rooms.forEach((room) => {
      const slotStart = new Date(currentDate);
      slotStart.setHours(hour, 0, 0, 0);
      const slotEnd = new Date(slotStart);
      slotEnd.setHours(hour + 1);

      const booking = bookings.find(
        (b) =>
          b.roomName === room.name &&
          new Date(b.startTime) < slotEnd &&
          new Date(b.endTime) > slotStart
      );

      if (booking) {
        const start = new Date(booking.startTime);
        const end = new Date(booking.endTime);
        html += `<td class="table-danger text-center">
                    <small><strong>${booking.username}</strong><br>
                    ${start.toLocaleTimeString("en-GB", {
                      hour: "2-digit",
                      minute: "2-digit",
                    })} - 
                    ${end.toLocaleTimeString("en-GB", {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}</small>
                </td>`;
      } else {
        html += `<td class="table-success text-center" style="cursor: pointer;" 
                         onclick="openBookingModal('${
                           room.name
                         }', '${slotStart.toISOString()}', '${slotEnd.toISOString()}')">
                    <i class="bi bi-plus-circle"></i> Available
                </td>`;
      }
    });
    html += "</tr>";
  }

  html += "</tbody></table>";
  calendarContainer.innerHTML = html;
}

async function renderWeekView() {
  const weekStart = new Date(currentDate);
  weekStart.setDate(currentDate.getDate() - currentDate.getDay() + 1); // Monday

  const calendarContainer = document.getElementById("calendarContainer");
  let html = '<table class="table table-bordered table-sm">';
  html += '<thead class="table-light"><tr><th style="width: 80px;">Time</th>';

  // Day headers (Monday to Friday)
  const days = [];
  for (let i = 0; i < 5; i++) {
    const day = new Date(weekStart);
    day.setDate(weekStart.getDate() + i);
    days.push(day);
    html += `<th class="text-center" colspan="${rooms.length}">
            ${day.toLocaleDateString("en-GB", {
              weekday: "short",
              month: "short",
              day: "numeric",
            })}
        </th>`;
  }
  html += "</tr><tr><th></th>";

  // Room headers for each day
  for (let i = 0; i < 5; i++) {
    rooms.forEach((room) => {
      html += `<th class="text-center" style="font-size: 0.8em;">${room.name}</th>`;
    });
  }
  html += "</tr></thead><tbody>";

  // Fetch bookings for all days
  const allBookings = [];
  for (const day of days) {
    const dateStr = day.toISOString().split("T")[0];
    const response = await fetch(`${API_BASE}/day/${dateStr}`);
    const dayBookings = await response.json();
    allBookings.push({ date: day, bookings: dayBookings });
  }

  // Time slots (8:00 - 20:00)
  for (let hour = 8; hour < 20; hour++) {
    html += "<tr>";
    html += `<td class="fw-bold" style="font-size: 0.8em;">${hour}:00</td>`;

    for (let dayIndex = 0; dayIndex < 5; dayIndex++) {
      const day = days[dayIndex];
      const dayBookings = allBookings[dayIndex].bookings;

      rooms.forEach((room) => {
        const slotStart = new Date(day);
        slotStart.setHours(hour, 0, 0, 0);
        const slotEnd = new Date(slotStart);
        slotEnd.setHours(hour + 1);

        const booking = dayBookings.find(
          (b) =>
            b.roomName === room.name &&
            new Date(b.startTime) < slotEnd &&
            new Date(b.endTime) > slotStart
        );

        if (booking) {
          html += `<td class="table-danger" style="font-size: 0.7em; padding: 2px;"></td>`;
        } else {
          html += `<td class="table-success" style="cursor: pointer; padding: 2px;" 
                             onclick="openBookingModal('${
                               room.name
                             }', '${slotStart.toISOString()}', '${slotEnd.toISOString()}')">
                    </td>`;
        }
      });
    }
    html += "</tr>";
  }

  html += "</tbody></table>";
  calendarContainer.innerHTML = html;
}

function updateDateDisplay() {
  const display = document.getElementById("currentDateDisplay");
  if (currentView === "day") {
    display.textContent = currentDate.toLocaleDateString("en-GB", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  } else {
    const weekStart = new Date(currentDate);
    weekStart.setDate(currentDate.getDate() - currentDate.getDay() + 1);
    const weekEnd = new Date(weekStart);
    weekEnd.setDate(weekStart.getDate() + 4);
    display.textContent = `Week: ${weekStart.toLocaleDateString(
      "en-GB"
    )} - ${weekEnd.toLocaleDateString("en-GB")}`;
  }
}

function openBookingModal(roomName, startTime, endTime) {
  selectedRoom = roomName;
  document.getElementById("modalRoomName").textContent = roomName;

  const start = new Date(startTime);
  const end = new Date(endTime);

  start.setMinutes(start.getMinutes() - start.getTimezoneOffset());
  end.setMinutes(end.getMinutes() - end.getTimezoneOffset());

  document.getElementById("modalStartTime").value = start
    .toISOString()
    .slice(0, 16);
  document.getElementById("modalEndTime").value = end
    .toISOString()
    .slice(0, 16);
  document.getElementById("modalError").style.display = "none";

  bookingModal.show();
}

async function createBookingFromModal() {
  const startTime = document.getElementById("modalStartTime").value;
  const endTime = document.getElementById("modalEndTime").value;
  const errorDiv = document.getElementById("modalError");

  if (!startTime || !endTime) {
    errorDiv.textContent = "Please fill in all fields";
    errorDiv.style.display = "block";
    return;
  }

  try {
    const response = await fetch(API_BASE, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        roomName: selectedRoom,
        startTime,
        endTime,
      }),
    });

    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.error || "Failed to create booking");
    }

    bookingModal.hide();
    loadCalendar();

    // Show success message
    const alertDiv = document.createElement("div");
    alertDiv.className =
      "alert alert-success alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3";
    alertDiv.style.zIndex = "9999";
    alertDiv.innerHTML = `
            Booking created successfully!
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
    document.body.appendChild(alertDiv);
    setTimeout(() => alertDiv.remove(), 5000);
  } catch (error) {
    console.error("Error creating booking:", error);
    errorDiv.textContent = error.message;
    errorDiv.style.display = "block";
  }
}
