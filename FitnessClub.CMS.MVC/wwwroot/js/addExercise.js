// Add Exercise Modal logic
document.addEventListener('DOMContentLoaded', () => {

    const dropdown = document.getElementById("exerciseDropdown");
    const rowsContainer = document.getElementById("exerciseRows");
    const addBtn = document.getElementById("addExerciseBtn");
    const listContainer = document.getElementById("selectedExercisesList");
    const modal = document.getElementById("addExerciseModal");
    const openModalBtn = document.getElementById("openExerciseModalBtn");
    const editBtn = document.getElementById("editProgramBtn");

    let tempRow = null;
    let selectedExercises = [];
    let tempIdCounter = 0;
    let isEditMode = false;

    // =======================================
    // Initialize selectedExercises from server
    // =======================================
    if (window.initialExercises && Array.isArray(window.initialExercises) && window.initialExercises.length > 0) {
        selectedExercises = window.initialExercises.map(ex => ({
            tempId: ++tempIdCounter,
            exerciseId: ex.exerciseID ?? ex.ExerciseID,
            exerciseName: ex.exerciseName ?? ex.ExerciseName,
            sets: ex.numberOfSets ?? ex.NumberOfSets,
            reps: ex.numberOfReps ?? ex.NumberOfReps,
            duration: ex.duration ?? ex.Duration
        }));
    }

    renderLockedRows();
    renderExerciseList();

    // ===============================
    // STEP 1: Selecting an exercise → TEMP row
    // ===============================
    dropdown.addEventListener("change", function () {
        if (!this.value) return;

        const exerciseId = this.value;
        const exerciseName = this.options[this.selectedIndex].text;

        if (tempRow) tempRow.remove();

        tempRow = document.createElement("div");
        tempRow.classList.add("row", "g-2", "align-items-end", "border", "p-2", "mb-2");

        tempRow.innerHTML = `
            <input type="hidden" value="${exerciseId}"/>
            <div class="col-3">
                <label class="form-label">Exercise</label>
                <input type="text" class="form-control" value="${exerciseName}" readonly />
            </div>
            <div class="col-2">
                <label class="form-label">Sets</label>
                <input type="number" class="form-control sets-input" min="1" />
            </div>
            <div class="col-2">
                <label class="form-label">Reps</label>
                <input type="number" class="form-control reps-input" min="1" />
            </div>
            <div class="col-3">
                <label class="form-label">Duration(min)</label>
                <input type="number" class="form-control duration-input" min="1" />
            </div>
        `;

        rowsContainer.appendChild(tempRow);
        addBtn.style.display = "inline-block";
    });

    // ===============================
    // STEP 2: Clicking Add → LOCK row + store it
    // ===============================
    addBtn.addEventListener("click", () => {
        if (!tempRow) return;

        const sets = tempRow.querySelector(".sets-input").value;
        const reps = tempRow.querySelector(".reps-input").value;
        const duration = tempRow.querySelector(".duration-input").value;

        if (!sets || !reps || !duration) {
            alert("Please fill all fields before adding.");
            return;
        }

        const exerciseId = tempRow.querySelector("input[type='hidden']").value;
        const exerciseName = tempRow.querySelector("input[readonly]").value;

        if (selectedExercises.some(e => e.exerciseId === exerciseId)) {
            alert("This exercise is already added.");
            tempRow.remove();
            tempRow = null;
            dropdown.value = "";
            addBtn.style.display = "none";
            return;
        }

        selectedExercises.push({
            tempId: ++tempIdCounter,
            exerciseId,
            exerciseName,
            sets,
            reps,
            duration
        });

        tempRow.remove();
        tempRow = null;
        renderLockedRows();
        renderExerciseList();

        dropdown.value = "";
        addBtn.style.display = "none";
        addBtn.blur();
    });

    // ===============================
    // Build LOCKED row template
    // ===============================
    function buildRow(ex, index) {
        const row = document.createElement("div");
        row.classList.add("row", "g-2", "align-items-end", "border", "p-2", "mb-2", "bg-light");
        row.dataset.id = ex.tempId;

        row.innerHTML = `
            <input type="hidden" name="Exercises[${index}].ExerciseID" value="${ex.exerciseId}" />
            <input type="hidden" name="Exercises[${index}].NumberOfSets" value="${ex.sets}" />
            <input type="hidden" name="Exercises[${index}].NumberOfReps" value="${ex.reps}" />
            <input type="hidden" name="Exercises[${index}].Duration" value="${ex.duration}" />

            <div class="col-3">
                <label class="form-label">Exercise</label>
                <input type="text" class="form-control" value="${ex.exerciseName}" readonly />
            </div>
            <div class="col-2">
                <label class="form-label">Sets</label>
                <input type="number" class="form-control sets-input" value="${ex.sets}" readonly />
            </div>
            <div class="col-2">
                <label class="form-label">Reps</label>
                <input type="number" class="form-control reps-input" value="${ex.reps}" readonly />
            </div>
            <div class="col-3">
                <label class="form-label">Duration</label>
                <input type="number" class="form-control duration-input" value="${ex.duration}" readonly />
            </div>
            <div class="col-2 d-flex gap-1">
                <button type="button" class="btn btn-sm btn-warning edit-btn">Edit</button>
                <button type="button" class="btn btn-sm btn-success save-btn d-none">Save</button>
                <button type="button" class="btn btn-sm btn-danger remove-btn">Remove</button>
            </div>
        `;

        return row;
    }

    function renderLockedRows() {
        rowsContainer.innerHTML = "";
        selectedExercises.forEach((ex, index) => rowsContainer.appendChild(buildRow(ex, index)));
    }

    // ===============================
    // Edit / Save / Remove row in modal
    // ===============================
    rowsContainer.addEventListener("click", e => {
        const row = e.target.closest(".row");
        if (!row) return;
        const id = parseInt(row.dataset.id);
        const ex = selectedExercises.find(x => x.tempId === id);

        if (e.target.classList.contains("edit-btn")) {
            row.querySelectorAll("input:not([readonly][type=text])").forEach(i => i.removeAttribute("readonly"));
            row.querySelector(".edit-btn").classList.add("d-none");
            row.querySelector(".save-btn").classList.remove("d-none");
        }

        if (e.target.classList.contains("save-btn")) {
            ex.sets = row.querySelector(".sets-input").value;
            ex.reps = row.querySelector(".reps-input").value;
            ex.duration = row.querySelector(".duration-input").value;
            renderLockedRows();
            renderExerciseList();
        }

        if (e.target.classList.contains("remove-btn")) {
            selectedExercises = selectedExercises.filter(x => x.tempId !== id);
            renderLockedRows();
            renderExerciseList();
        }
    });

    // ===============================
    // Render exercises ABOVE the button
    // ===============================
    function renderExerciseList() {
        listContainer.innerHTML = "";
        selectedExercises.forEach(ex => {
            listContainer.innerHTML += `
            <div class="card mb-2" data-id="${ex.tempId}">
                <div class="card-body d-flex align-items-center" style="padding:0.75em">
                    <div class="d-flex flex-column w-100">
                        <div class="d-flex justify-content-between">
                            <div>
                                <strong>${ex.exerciseName}</strong>
                                ${isEditMode ? `<span class="badge bg-success me-2">Added</span>` : ``}
                            </div>
                            ${isEditMode ? `<button type="button" class="remove-list-btn" style="all:unset; cursor:pointer">X</button>` : ``}
                        </div>
                        <small class="text-muted">
                            Sets: ${ex.sets} | Reps: ${ex.reps} | Duration: ${ex.duration} min
                        </small>
                    </div>
                </div>
            </div>`;
        });
    }

    // ===============================
    // Remove button in list (outside renderExerciseList)
    // ===============================
    listContainer.addEventListener("click", e => {
        if (!e.target.classList.contains("remove-list-btn")) return;
        const card = e.target.closest(".card");
        if (!card) return;
        const id = parseInt(card.dataset.id);
        selectedExercises = selectedExercises.filter(x => x.tempId !== id);
        renderLockedRows();
        renderExerciseList();
    });

    // ===============================
    // Modal hidden → clear temp rows
    // ===============================
    modal.addEventListener("hidden.bs.modal", () => {
        tempRow = null;
        renderLockedRows();
    });

    // Restore focus when modal fully closes
    modal.addEventListener("hide.bs.modal", () => {
        openModalBtn.focus();
    });

});