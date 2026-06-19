// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    document.addEventListener('DOMContentLoaded', function () {
        fetch('/api/ClassesApi')
            .then(response => response.json())
            .then(data => {
                const container = document.getElementById('classCards');

                data.forEach(item => {
                    container.innerHTML += `
                        <div class="col-xl-6 col-lg-6">
                            <div class="single-topic text-center mb-30">
                                <div class="topic-img">
                                    <img src="${item.imageUrl}" alt="${item.title}">
                                    <div class="topic-content-box">
                                        <div class="topic-content">
                                            <h3>${item.title}</h3>
                                            <p>${item.description}</p>
                                            <a href="/courses.html" class="border-btn">View Courses</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                });
            })
            .catch(error => console.error('Error loading class data:', error));
    });

async function bookClass(classId) {
    try {
        const response = await fetch('https://localhost:7089/api/booking', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ classId: classId, clientId: 9 }) // static clientId for now
        });

        let resultText = await response.text();
        let result;

        try {
            result = JSON.parse(resultText);
        } catch (e) {
            console.error("Invalid JSON response:", resultText);
        }

        if (response.ok) {
            alert("✅ Booking successful!");
        } else {
            alert("❌ Booking failed: " + (result?.message || response.statusText));
        }
    } catch (err) {
        alert("⚠️ Error: " + err.message);
    }
}

async function sendMessage() {
    const title = document.getElementById("subject").value.trim();
    const content = document.getElementById("message").value.trim();
    const type = document.getElementById("type").value;
    const userId = 5; // Static for now

    // ✋ Basic validation
    if (!title || !content || !type) {
        alert("❗Please fill out all fields before sending.");
        return;
    }

    try {
        const response = await fetch("https://localhost:7089/api/messages", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ title, content, type, userId })
        });

        if (response.ok) {
            alert("✅ Message sent!");
            document.getElementById("contactForm").reset();
        } else {
            const error = await response.json();
            alert("❌ Failed: " + (error?.message || "Unknown error"));
        }
    } catch (err) {
        alert("⚠️ Error: " + err.message);
    }
}

/*async function loadMessageHistory() {
    const userId = 5; // Replace with actual logged-in user ID if available dynamically

    try {
        const response = await fetch(`https://localhost:7089/api/messages/user/${userId}`);

        if (response.ok) {
            const messages = await response.json();

            let historyHtml = "<h5>Your Previous Messages:</h5><ul>";
            messages.forEach(msg => {
                historyHtml += `<li><strong>${msg.type}</strong> - ${msg.title}<br>${msg.content}<br><small>${msg.time}</small></li><hr>`;
            });
            historyHtml += "</ul>";

            // Inject into modal or div
            document.getElementById("messageHistory").innerHTML = historyHtml;

        } else {
            alert("❌ Failed to load message history");
        }

    } catch (err) {
        alert("⚠️ Error: " + err.message);
    }
}*/

let messages = [];
let currentPage = 1;
const pageSize = 8;

async function loadMessageHistory(button) {
    const userId = 5; // Static user ID
    const historySection = document.getElementById("messageHistory");

    const isHidden = historySection.style.display === "none" || historySection.style.display === "";
    historySection.style.display = isHidden ? "block" : "none";
    button.innerText = isHidden ? "Hide History" : "View History";

    if (!isHidden) return;

    try {
        const response = await fetch(`https://localhost:7089/api/messages/user/${userId}/all`);
        if (!response.ok) throw new Error('Failed to load messages.');

        messages = await response.json();
        currentPage = 1;
        renderMessages();
    } catch (error) {
        alert("⚠️ Could not fetch message history.");
        console.error(error);
    }
}
function renderMessages() {
    const messageList = document.getElementById("messageList");
    messageList.innerHTML = '';

    const filter = document.getElementById("filterType").value;
    const filtered = filter ? messages.filter(msg => msg.type === filter) : messages;

    if (filtered.length === 0) {
        messageList.innerHTML = "<p style='color: gray;'>No previous messages found.</p>";
        renderPaginationControls(0);
        return;
    }

    const start = (currentPage - 1) * pageSize;
    const paginated = filtered.slice(start, start + pageSize);

    paginated.forEach(msg => {
        const card = document.createElement('div');
        card.style.flex = "1 1 calc(45% - 20px)";
        card.style.maxWidth = "400px";
        card.style.minWidth = "250px";
        card.style.border = "2px solid red";
        card.style.borderRadius = "15px";
        card.style.padding = "20px";
        card.style.marginBottom = "15px";
        card.style.backgroundColor = "#fff0f0";
        card.style.boxShadow = "0 4px 10px rgba(0, 0, 0, 0.1)";
        card.style.textAlign = "center";
        card.style.fontSize = "16px";

        card.innerHTML = `
            <strong style="color: #cc0000; font-size: 18px;">${msg.type} - ${msg.title}</strong><br>
            <p style="color: #333; margin: 10px 0;">${msg.content}</p>
            <small style="color: #666;">${new Date(msg.time).toLocaleString()}</small>
        `;
        messageList.appendChild(card);
    });

    renderPaginationControls(filtered.length);
}
function renderPaginationControls(totalItems) {
    const controls = document.getElementById("paginationControls");
    controls.innerHTML = '';
    const totalPages = Math.ceil(totalItems / pageSize);

    for (let i = 1; i <= totalPages; i++) {
        const btn = document.createElement('button');
        btn.innerText = i;
        btn.className = 'btn btn-sm btn-outline-danger mx-1';
        if (i === currentPage) btn.classList.add('active');
        btn.onclick = () => {
            currentPage = i;
            renderMessages();
        };
        controls.appendChild(btn);
    }
}
function exportToCSV() {
    if (!messages || messages.length === 0) {
        alert("No messages to export.");
        return;
    }

    const headers = ['Type', 'Title', 'Content', 'Time'];
    const rows = messages.map(m => [m.type, m.title, m.content, new Date(m.time).toLocaleString()]);

    let csvContent = "data:text/csv;charset=utf-8," + [headers, ...rows].map(e => e.join(",")).join("\n");

    const encodedUri = encodeURI(csvContent);
    const link = document.createElement("a");
    link.setAttribute("href", encodedUri);
    link.setAttribute("download", "messages_history.csv");
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
async function exportToPDF() {
    if (!messages || messages.length === 0) {
        alert("No messages to export.");
        return;
    }

    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();

    const tableData = messages.map(msg => [
        msg.type,
        msg.title,
        msg.content,
        new Date(msg.time).toLocaleString()
    ]);

    doc.autoTable({
        head: [['Type', 'Title', 'Content', 'Time']],
        body: tableData,
        startY: 20
    });

    doc.save("messages_history.pdf");
}
