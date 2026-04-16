document.getElementById("overtimeForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const payload = {
        date: document.getElementById("date").value,
        hours: document.getElementById("hours").value,
        reason: document.getElementById("reason").value
    };

    const response = await fetch("/api/submit-overtime", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    const result = await response.text();
    document.getElementById("status").innerText = result;
});

