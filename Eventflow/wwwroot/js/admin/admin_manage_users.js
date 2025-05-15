document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".btn-edit-user").forEach(button => {
        button.addEventListener("click", function () {
            const userId = this.dataset.userId;
            const username = this.dataset.userUsername;
            const email = this.dataset.userEmail;
            const role = this.dataset.userRole;

            document.getElementById("editUserId").value = userId;
            document.getElementById("editUsername").value = username;
            document.getElementById("editEmail").value = email;
            document.getElementById("editRole").value = role;

            const modal = new bootstrap.Modal(document.getElementById("editUserModal"));
            modal.show();
        });
    });

    const editForm = document.getElementById("editUserForm");

    if (editForm) {
        editForm.addEventListener("submit", function (e) {
            e.preventDefault();

            const formData = new FormData(editForm);
            const formObject = Object.fromEntries(formData.entries());

            fetch("/Admin/UpdateUser", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(formObject)
            })
                .then(response => response.json())
                .then(result => {
                     if (result.success) {
                     const modalEl = document.getElementById("editUserModal");
                     const modal = bootstrap.Modal.getInstance(modalEl);
                     modal.hide();

                     const row = document.querySelector(`tr[data-user-id="${formObject.Id}"]`);
                     if (row) {
                           row.querySelector(".username-cell").textContent = formObject.Username;
                           row.querySelector(".email-cell").textContent = formObject.Email;

                           const roleBadge = row.querySelector(".role-cell span");
                           if (roleBadge) {
                              roleBadge.textContent = formObject.Role;
                              roleBadge.className = `badge bg-${formObject.Role === "Admin" ? "primary" : "secondary"}`;
                           }

                           const editBtn = row.querySelector(".btn-edit-user");
                           if (editBtn) {
                                 editBtn.dataset.userUsername = formObject.Username;
                                 editBtn.dataset.userEmail = formObject.Email;
                                 editBtn.dataset.userRole = formObject.Role;
                           }
                     }
                  } else {
                     alert(result.message || "Something went wrong.");
                  }
                })
                .catch(error => {
                    console.error("Error during user update:", error);
                    alert("An unexpected error occurred.");
                });
        });
    }
});