async function loadRequests() {
    const response = await fetch("/api/pending-requests");
    const data = await response.json();

    const container = document.getElementById("requests");
    container.innerHTML = "";

    data.forEach(req => {
        const div = document.createElement("div");
        div.className = "request";

        div.innerHTML = `
            <p><strong>Date:</strong> ${req.date}</p>
            <p><strong>Hours:</strong> ${req.hours}</p>
            <p><strong>Reason:</strong> ${req.reason}</p>
            <button onclick="approve('${req.id}')">Approve</button>
            <button onclick="decline('${req.id}')">Decline</button>
        `;

        container.appendChild(div);
    });
}

async function approve(id) {
    await fetch("/api/approve", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id })
    });
    loadRequests();
}

async function decline(id) {
    await fetch("/api/decline", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id })
    });
    loadRequests();
}

loadRequests();
