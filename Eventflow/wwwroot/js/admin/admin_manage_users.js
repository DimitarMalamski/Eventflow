document.addEventListener("DOMContentLoaded", function () {
    // Initial binding
    document.querySelectorAll(".btn-edit-user").forEach(btn => btn.addEventListener("click", onEditButtonClick));
    document.querySelectorAll(".btn-toggle-ban").forEach(btn => btn.addEventListener("click", onBanToggleClick));

    const editForm = document.getElementById("editUserForm");

    if (editForm) {
        editForm.addEventListener("submit", function (e) {
            e.preventDefault();
            const formData = new FormData(editForm);
            const formObject = Object.fromEntries(formData.entries());
            const userId = formObject.Id;

            fetch("/Admin/UpdateUser", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formObject)
            })
            .then(response => response.json())
            .then(result => {
                if (!result.success) {
                    alert(result.message || "Something went wrong.");
                    return;
                }

                const modal = bootstrap.Modal.getInstance(document.getElementById("editUserModal"));
                modal.hide();

                const row = document.querySelector(`tr[data-user-id="${userId}"]`);
                if (!row) return;

                row.querySelector(".username-cell").textContent = formObject.Username;
                row.querySelector(".email-cell").textContent = formObject.Email;

                const roleBadge = row.querySelector(".role-cell span");
                if (roleBadge) {
                    roleBadge.textContent = formObject.Role;
                    roleBadge.className = `badge bg-${formObject.Role === "Admin" ? "primary" : "secondary"}`;
                }

                const statusBadge = row.querySelector("td:nth-child(4) .badge");
                const currentStatus = statusBadge?.textContent?.trim() || "Active";
                const isBanned = currentStatus === "Banned";

                // Rebuild actions based on role
                let actionsHtml = `
                    <div class="d-flex justify-content-center flex-wrap gap-1">
                        <button type="button"
                            class="btn btn-sm btn-warning px-2 btn-edit-user"
                            title="Edit"
                            data-user-id="${userId}"
                            data-user-username="${formObject.Username}"
                            data-user-email="${formObject.Email}"
                            data-user-role="${formObject.Role}">
                            <i class="bi bi-pencil-fill"></i>
                        </button>`;

                if (formObject.Role === "User") {
                    const banLabel = isBanned ? "Unban" : "Ban";
                    const banIcon = isBanned ? "bi-unlock-fill" : "bi-slash-circle-fill";
                    const banClass = isBanned ? "btn-success" : "btn-danger";

                    actionsHtml += `
                        <button type="button"
                            class="btn btn-sm ${banClass} px-2 btn-toggle-ban"
                            data-user-id="${userId}"
                            data-user-status="${currentStatus}"
                            title="${banLabel}">
                            <i class="bi ${banIcon}"></i>
                        </button>
                        <a href="/Admin/DeleteUser?id=${userId}"
                            class="btn btn-sm btn-outline-danger px-2"
                            onclick="return confirm('Are you sure you want to delete this user?');"
                            title="Delete">
                            <i class="bi bi-trash-fill"></i>
                        </a>`;
                }

                actionsHtml += `</div>`;
                row.querySelector("td:last-child").innerHTML = actionsHtml;

                bindRowButtons(row); // 🔄 Reattach updated buttons
            })
            .catch(error => {
                console.error("Error during user update:", error);
                alert("An unexpected error occurred.");
            });
        });
    }

    function onEditButtonClick() {
        const btn = this;
        document.getElementById("editUserId").value = btn.dataset.userId;
        document.getElementById("editUsername").value = btn.dataset.userUsername;
        document.getElementById("editEmail").value = btn.dataset.userEmail;
        document.getElementById("editRole").value = btn.dataset.userRole;

        new bootstrap.Modal(document.getElementById("editUserModal")).show();
    }

    function onBanToggleClick() {
        const userId = this.dataset.userId;

        fetch("/Admin/ToggleBan", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ Id: userId })
        })
        .then(res => res.json())
        .then(result => {
            if (!result.success) {
                alert(result.message || "Failed to toggle user status.");
                return;
            }

            const row = document.querySelector(`tr[data-user-id="${userId}"]`);
            if (!row) return;

            const statusBadge = row.querySelector("td:nth-child(4) .badge");
            if (statusBadge) {
                statusBadge.textContent = result.newStatus;
                statusBadge.classList = `badge bg-${result.newStatus === "Banned" ? "danger" : "success"}`;
            }

            const toggleBtn = row.querySelector(".btn-toggle-ban");
            const icon = toggleBtn.querySelector("i");
            const isBanned = result.newStatus === "Banned";

            toggleBtn.classList.remove("btn-danger", "btn-success");
            toggleBtn.classList.add(isBanned ? "btn-success" : "btn-danger");
            toggleBtn.title = isBanned ? "Unban" : "Ban";
            toggleBtn.dataset.userStatus = result.newStatus;

            if (icon) {
                icon.className = isBanned ? "bi bi-unlock-fill" : "bi bi-slash-circle-fill";
            }
        })
        .catch(error => {
            console.error("Ban/Unban error:", error);
            alert("An unexpected error occurred.");
        });
    }

    function bindRowButtons(row) {
        row.querySelector(".btn-edit-user")?.addEventListener("click", onEditButtonClick);
        row.querySelector(".btn-toggle-ban")?.addEventListener("click", onBanToggleClick);
    }
});