const API_BASE = "/api/rooms";
let deleteModal;
let selectedRoomName = null;

document.addEventListener("DOMContentLoaded", () => {
  deleteModal = new bootstrap.Modal(document.getElementById("deleteModal"));
  setupEventListeners();
  loadRooms();
});

function setupEventListeners() {
  document
    .getElementById("createRoomForm")
    .addEventListener("submit", handleCreateRoom);
  document
    .getElementById("confirmDeleteBtn")
    .addEventListener("click", handleDeleteRoom);
}

async function loadRooms() {
  const loadingSpinner = document.getElementById("loadingSpinner");
  const tableContainer = document.getElementById("roomsTableContainer");

  try {
    loadingSpinner.style.display = "block";
    tableContainer.style.display = "none";

    const response = await fetch(API_BASE);
    if (!response.ok) {
      throw new Error("Failed to load rooms");
    }

    const rooms = await response.json();
    displayRooms(rooms);
  } catch (error) {
    console.error("Error loading rooms:", error);
    showMessage("Failed to load rooms. Please refresh the page.", "error");
  } finally {
    loadingSpinner.style.display = "none";
    tableContainer.style.display = "block";
  }
}

function displayRooms(rooms) {
  const tableContainer = document.getElementById("roomsTableContainer");
  const roomCount = document.getElementById("roomCount");

  roomCount.textContent = rooms.length;

  if (rooms.length === 0) {
    tableContainer.innerHTML = `
            <div class="text-center py-4 text-muted">
                <i class="bi bi-inbox" style="font-size: 3rem;"></i>
                <p class="mt-2">No rooms available. Add your first room using the form above.</p>
            </div>
        `;
    return;
  }

  // Sort rooms by name
  rooms.sort((a, b) => a.name.localeCompare(b.name));

  let html =
    '<div class="table-responsive"><table class="table table-hover table-striped">';
  html += `
        <thead class="table-light">
            <tr>
                <th><i class="bi bi-tag"></i> Room Name</th>
                <th><i class="bi bi-people"></i> Capacity</th>
                <th><i class="bi bi-info-circle"></i> Status</th>
                <th class="text-center"><i class="bi bi-gear"></i> Actions</th>
            </tr>
        </thead>
        <tbody>
    `;

  rooms.forEach((room) => {
    const statusBadge = room.isAvailable
      ? '<span class="badge bg-success"><i class="bi bi-check-circle"></i> Available</span>'
      : '<span class="badge bg-warning text-dark"><i class="bi bi-clock"></i> Occupied</span>';

    html += `
            <tr>
                <td><strong>${escapeHtml(room.name)}</strong></td>
                <td>
                    <span class="badge bg-info">
                        <i class="bi bi-people"></i> ${room.capacity} people
                    </span>
                </td>
                <td>${statusBadge}</td>
                <td class="text-center">
                    <button type="button" 
                            class="btn btn-sm btn-danger" 
                            onclick="confirmDelete('${escapeHtml(room.name)}')">
                        <i class="bi bi-trash"></i> Delete
                    </button>
                </td>
            </tr>
        `;
  });

  html += "</tbody></table></div>";
  tableContainer.innerHTML = html;
}

async function handleCreateRoom(e) {
  e.preventDefault();

  const name = document.getElementById("Name").value.trim();
  const capacity = parseInt(document.getElementById("Capacity").value);

  if (!name || !capacity || capacity < 1) {
    showMessage("Please fill in all fields correctly.", "error");
    return;
  }

  try {
    const response = await fetch(API_BASE, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: name,
        capacity: capacity,
      }),
    });

    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.error || "Failed to create room");
    }

    showMessage(`Room '${name}' created successfully!`, "success");
    document.getElementById("createRoomForm").reset();
    await loadRooms();
  } catch (error) {
    console.error("Error creating room:", error);
    showMessage(error.message, "error");
  }
}

function confirmDelete(roomName) {
  selectedRoomName = roomName;
  document.getElementById("roomNameDisplay").textContent = roomName;
  deleteModal.show();
}

async function handleDeleteRoom() {
  if (!selectedRoomName) return;

  try {
    const response = await fetch(
      `${API_BASE}/${encodeURIComponent(selectedRoomName)}`,
      {
        method: "DELETE",
      }
    );

    if (!response.ok) {
      const data = await response.json();
      throw new Error(data.error || "Failed to delete room");
    }

    showMessage(`Room '${selectedRoomName}' deleted successfully!`, "success");
    deleteModal.hide();
    selectedRoomName = null;
    await loadRooms();
  } catch (error) {
    console.error("Error deleting room:", error);
    showMessage(error.message, "error");
  }
}

function showMessage(message, type = "success") {
  const messageContainer = document.getElementById("messageContainer");
  const alertClass = type === "success" ? "alert-success" : "alert-danger";
  const icon = type === "success" ? "check-circle" : "exclamation-triangle";

  messageContainer.innerHTML = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            <i class="bi bi-${icon}"></i> ${escapeHtml(message)}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;

  // Auto-dismiss after 5 seconds
  setTimeout(() => {
    const alert = messageContainer.querySelector(".alert");
    if (alert) {
      const bsAlert = new bootstrap.Alert(alert);
      bsAlert.close();
    }
  }, 5000);
}

function escapeHtml(text) {
  const map = {
    "&": "&amp;",
    "<": "&lt;",
    ">": "&gt;",
    '"': "&quot;",
    "'": "&#039;",
  };
  return text.replace(/[&<>"']/g, (m) => map[m]);
}
