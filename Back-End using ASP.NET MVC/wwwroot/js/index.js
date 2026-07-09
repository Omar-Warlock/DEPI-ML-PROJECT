// ==========================
// Position Buttons
// ==========================

const positionButtons = document.querySelectorAll(".positions button");

positionButtons.forEach(button => {

    button.addEventListener("click", () => {

        positionButtons.forEach(btn => btn.classList.remove("active"));

        button.classList.add("active");

    });

});


// ==========================
// Search (Frontend Only)
// ==========================

const searchInput = document.querySelector(".search-box input");
const rows = document.querySelectorAll("tbody tr");

searchInput.addEventListener("keyup", () => {

    const value = searchInput.value.toLowerCase();

    rows.forEach(row => {

        if (row.innerText.toLowerCase().includes(value)) {

            row.style.display = "";

        } else {

            row.style.display = "none";

        }

    });

});


// ==========================
// Apply Button
// ==========================

const applyBtn = document.querySelector(".apply-btn");

applyBtn.addEventListener("click", () => {

    alert("Filters Applied Successfully ✔");

});


// ==========================
// Hover Effect For Cards
// ==========================

const cards = document.querySelectorAll(".card");

cards.forEach(card => {

    card.addEventListener("mouseenter", () => {

        card.style.transform = "translateY(-8px)";
        card.style.transition = ".3s";

    });

    card.addEventListener("mouseleave", () => {

        card.style.transform = "translateY(0)";

    });

});


// ==========================
// Export Button
// ==========================

document.querySelector(".btn-outline").addEventListener("click", () => {

    alert("Export Report");

});


// ==========================
// New Shortlist
// ==========================

document.querySelector(".btn-primary").addEventListener("click", () => {

    alert("New Shortlist Created");

});


// ==========================
// Table Hover Animation
// ==========================

rows.forEach(row => {

    row.addEventListener("mouseenter", () => {

        row.style.transition = ".3s";

    });

});