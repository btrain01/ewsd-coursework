const API_BASE = "http://localhost:5188";

// ── AUTH GUARD ─────────────────────────────────────────────
const currentUser = JSON.parse(localStorage.getItem("currentUser") || "null");

if (!currentUser) {
    window.location.href = "../login.html";
}

// ── AUTH HEADER ────────────────────────────────────────────
// AuthenticationAttribute expects the full user object as a JSON string
function authHeaders() {
    return {
        "Content-Type": "application/json",
        Authorization: JSON.stringify(currentUser),
    };
}

// ── HELPERS ────────────────────────────────────────────────
function formatDate(dateStr) {
    const [time, date] = dateStr.split(" ");
    const [day, month, year] = date.split("/");
    const iso = `${year}-${month}-${day}T${time}`;

    const d = new Date(iso);
    return d.toLocaleDateString("en-GB", {
        weekday: "short",
        day: "numeric",
        month: "long",
    });
}

function formatTime(dateStr) {
    const [time, date] = dateStr.split(" ");
    const [day, month, year] = date.split("/");
    const iso = `${year}-${month}-${day}T${time}`;
    const d = new Date(iso);
    return d.toLocaleTimeString("en-GB", {
        hour: "2-digit",
        minute: "2-digit",
    });
}

function timeAgo(dateStr) {
    const diff = Math.floor((Date.now() - new Date(dateStr)) / 60000);
    if (diff < 1) return "Just now";
    if (diff < 60) return `${diff}m ago`;
    if (diff < 1440) return `${Math.floor(diff / 60)}h ago`;
    return `${Math.floor(diff / 1440)}d ago`;
}

function initials(name) {
    if (!name) return "?";
    return name
        .split(" ")
        .map((w) => w[0])
        .join("")
        .substring(0, 2)
        .toUpperCase();
}

function meetingPill(status) {
    const map = {
        SCHEDULED: { cls: "pill-pending", label: "⊙ Scheduled" },
        CONFIRMED: { cls: "pill-done", label: "Confirmed" },
        IN_PROGRESS: { cls: "pill-done", label: "In Progress" },
        COMPLETED: { cls: "pill-done", label: "Completed" },
        CANCELLED: { cls: "pill-pending", label: "Cancelled" },
    };
    const p = map[status] || { cls: "pill-pending", label: status };
    return `<span class="pill ${p.cls}">${p.label}</span>`;
}

function meetingTypePill(type) {
    return type === "VIRTUAL"
        ? `<span class="pill pill-virtual">Virtual</span>`
        : `<span class="pill pill-done">In-Person</span>`;
}

// ── LOAD DASHBOARD DATA ────────────────────────────────────
async function loadDashboard() {
    const id = currentUser.id;

    // Set username in sidebar and topbar while data loads
    const username = currentUser.username || "Student";
    document.getElementById("profileName").textContent = username;
    document.getElementById("topbarAv").textContent = username
        .substring(0, 2)
        .toUpperCase();

    try {
        // Fire all requests in parallel for speed
        const [profileRes, tutorRes, meetingsRes, messagesRes, documentsRes] =
            await Promise.all([
                fetch(`${API_BASE}/User/dashboard/${id}`, {
                headers: authHeaders(),
                }),
                fetch(`${API_BASE}/User/assignment/student/${id}`, {
                headers: authHeaders(),
                }),
                fetch(`${API_BASE}/User/meetings/${id}`, {
                headers: authHeaders(),
                }),
                fetch(`${API_BASE}/User/messages/${id}`, {
                headers: authHeaders(),
                }),
                fetch(`${API_BASE}/User/documents/${id}`, {
                headers: authHeaders(),
                }),
        ]);

        // ── PROFILE ──────────────────────────────────────────
        if (profileRes.ok) {
            const profile = await profileRes.json();
            // Use fullName from API if available
            const displayName = profile.fullName || profile.username || username;
            document.getElementById("profileName").textContent = displayName;
            document.getElementById("topbarAv").textContent = initials(displayName);
        }

        // ── TUTOR CARD ───────────────────────────────────────
        const tutorBody = document.getElementById("tutorCardBody");
        const tutorStatus = document.getElementById("tutorStatus");

        if (tutorRes.ok) {
        const tutor = await tutorRes.json();
        const tutorInitials = initials(tutor.name || "TU");
        // Backend returns date as "HH:mm:ss dd/MM/yyyy" — parse manually
        let allocDate = "Unknown";
        if (tutor.allocatedAt) {
            const parts = tutor.allocatedAt.split(" ");
            // parts[1] is "dd/MM/yyyy"
            const dateParts = (parts[1] || parts[0]).split("/");
            if (dateParts.length === 3) {
            const d = new Date(
                `${dateParts[2]}-${dateParts[1]}-${dateParts[0]}`,
            );
            allocDate = d.toLocaleDateString("en-GB", {
                day: "numeric",
                month: "long",
                year: "numeric",
            });
            }
        }

        tutorStatus.textContent = "Assigned";
        tutorStatus.style.color = "var(--green)";
        tutorStatus.style.fontWeight = "600";

        tutorBody.innerHTML = `
            <div class="tutor-card-body">
            <div class="tutor-av-lg">${tutorInitials}</div>
            <div class="tutor-details">
                <div class="tutor-name">${tutor.name || "Your Tutor"}</div>
                <div class="tutor-email">${tutor.email || ""}</div>
                ${tutor.notes ? `<div class="tutor-notes">Notes: ${tutor.notes}</div>` : ""}
                <div class="tutor-allocated">Assigned on ${allocDate}</div>
            </div>
            <div class="tutor-actions">
                <button class="btn-msg">
                <svg width="14" height="14" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"/></svg>
                Send Message
                </button>
                <button class="btn-meet">
                <svg width="14" height="14" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="18" rx="2"/><line x1="16" y1="2" x2="16" y2="6"/><line x1="8" y1="2" x2="8" y2="6"/><line x1="3" y1="10" x2="21" y2="10"/></svg>
                Schedule Meeting
                </button>
            </div>
            </div>
        `;
        } else if (tutorRes.status === 404) {
            // No tutor assigned yet
            tutorStatus.textContent = "Not assigned";
            tutorStatus.style.color = "var(--amber)";
            tutorStatus.style.fontWeight = "600";

            tutorBody.innerHTML = `
                <div class="no-tutor-body">
                <svg width="36" height="36" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                </svg>
                <div style="font-weight:600;color:var(--gray-600)">No tutor assigned yet</div>
                <p>An authorised staff member will assign a personal tutor to you shortly. You will receive a notification when this happens.</p>
                </div>
            `;
        } else {
            tutorStatus.textContent = "Unavailable";
            tutorBody.innerHTML = `<div class="empty-state">Could not load tutor details</div>`;
        }

        // ── MEETINGS ─────────────────────────────────────────
        if (meetingsRes.ok) {
        const meetings = await meetingsRes.json();
        const upcoming = meetings.filter((m) =>
            m.meetingStatus === "Scheduled" ||
            m.meetingStatus === "Confirmed",
        );

        // Update summary card
        document.getElementById("meetCount").textContent = upcoming.length;
        document.getElementById("meetSub").textContent =
            upcoming.length === 1
            ? "1 Scheduled"
            : `${upcoming.length} Scheduled`;

        // Render meetings panel
        const panel = document.getElementById("meetingsPanel");
        if (upcoming.length === 0) {
            panel.innerHTML = `<div class="empty-state">No upcoming meetings</div>`;
        } else {
            panel.innerHTML = upcoming
            .slice(0, 4)
            .map(
                (m) => `
            <div class="m-row">
                <div class="m-icon">
                <svg width="16" height="16" fill="none" stroke="currentColor" stroke-width="2" viewBox="0 0 24 24">
                    <rect x="3" y="4" width="18" height="18" rx="2"/>
                    <line x1="16" y1="2" x2="16" y2="6"/>
                    <line x1="8" y1="2" x2="8" y2="6"/>
                    <line x1="3" y1="10" x2="21" y2="10"/>
                </svg>
                </div>
                <div class="m-info">
                <div class="m-date">${formatDate(m.scheduledAt)}</div>
                <div class="m-time">${formatTime(m.scheduledAt)} · ${m.durationInMinutes} mins</div>
                </div>
                ${meetingPill(m.meetingStatus)}
            </div>
            <div class="m-row">
                <div class="m-av" style="background:var(--blue)">
                ${m.agenda ? initials(m.agenda) : "MT"}
                </div>
                <div class="m-person">
                <div class="m-name">${m.agenda || "Meeting"}</div>
                <div class="m-role">${m.meetingLink ? m.meetingLink : "No link provided"}</div>
                </div>
                ${meetingTypePill(m.meetingType)}
            </div>
            `,
            )
            .join("");
        }
        } else {
        document.getElementById("meetingsPanel").innerHTML =
            `<div class="empty-state">Could not load meetings</div>`;
        document.getElementById("meetCount").textContent = "—";
        document.getElementById("meetSub").textContent = "Unavailable";
        }

        // ── MESSAGES ─────────────────────────────────────────
        if (messagesRes.ok) {
        const messages = await messagesRes.json();
        const unread = messages.filter((m) => !m.isRead);

        // Update summary card
        document.getElementById("msgCount").textContent = messages.length;
        document.getElementById("msgSub").textContent =
            unread.length > 0 ? `${unread.length} Unread` : "All read";

        // Render messages panel
        const panel = document.getElementById("messagesPanel");
        if (messages.length === 0) {
            panel.innerHTML = `<div class="empty-state">No messages yet</div>`;
        } else {
            panel.innerHTML = messages
            .slice(0, 4)
            .map((m) => {
                const isFromMe = m.senderId === currentUser.id;
                const senderLabel = isFromMe ? "You" : `User #${m.senderId}`;
                const avatarBg = isFromMe
                ? "background:var(--gray-400)"
                : "background:var(--blue)";
                const avatarText = isFromMe ? "ME" : initials(senderLabel);
                return `
                <div class="msg-row">
                <div class="msg-av" style="${avatarBg}">${avatarText}</div>
                <div class="msg-body">
                    <div class="msg-top">
                    <span class="msg-name">${senderLabel}</span>
                    <span class="msg-time">${timeAgo(m.createdAt)}</span>
                    </div>
                    <div class="msg-preview">${m.subject || m.body || "No content"}</div>
                    <div class="msg-actions">
                    ${!m.isRead && !isFromMe ? `<span class="pill pill-pending" style="font-size:10px;padding:2px 7px">Unread</span>` : ""}
                    <button class="view-btn-sm">View</button>
                    </div>
                </div>
                </div>
            `;
            })
            .join("");
        }
        } else {
        document.getElementById("messagesPanel").innerHTML =
            `<div class="empty-state">Could not load messages</div>`;
        document.getElementById("msgCount").textContent = "—";
        document.getElementById("msgSub").textContent = "Unavailable";
        }

        //  DOCUMENTS
        if (documentsRes.ok) {
        const documents = await documentsRes.json();
        document.getElementById("docCount").textContent = documents.length;
        document.getElementById("docSub").textContent =
            `${documents.length} Shared`;
        } else {
        document.getElementById("docCount").textContent = "—";
        document.getElementById("docSub").textContent = "Unavailable";
        }
    } catch (err) {
        console.error("Dashboard load error:", err);
    }
}

// LOGOUT
async function logoutUser() {
    try {
        await fetch(`${API_BASE}/Authentication/logout/${currentUser.id}`, {
        method: "POST",
        headers: authHeaders(),
        });
    } catch (e) {
        console.warn("Logout failed, clearing session anyway.", e);
    }
    localStorage.removeItem("currentUser");
    localStorage.removeItem("registeredRoleId");
    localStorage.removeItem("registeredUsername");
    window.location.href = "../login.html";
}

// SIDEBAR TOGGLE & NAV ACTIVE STATE
function openSidebar() {
    document.getElementById("sidebar").classList.add("open");
    document.getElementById("overlay").classList.add("show");
}
function closeSidebar() {
    document.getElementById("sidebar").classList.remove("open");
    document.getElementById("overlay").classList.remove("show");
}
function setActive(el) {
    document
        .querySelectorAll(".nav-link")
        .forEach((l) => l.classList.remove("active"));
    el.classList.add("active");
    if (window.innerWidth <= 768) closeSidebar();
}

// Sign out button hover effect
const signOutBtn = document.getElementById("signOutBtn");
    if (signOutBtn) {
    signOutBtn.addEventListener("mouseover", () => {
        signOutBtn.style.background = "#fee2e2";
        signOutBtn.style.color = "#dc2626";
        signOutBtn.style.borderColor = "#fecaca";
    });
    signOutBtn.addEventListener("mouseout", () => {
        signOutBtn.style.background = "var(--white)";
        signOutBtn.style.color = "var(--gray-600)";
        signOutBtn.style.borderColor = "var(--gray-200)";
    });
}

loadDashboard();