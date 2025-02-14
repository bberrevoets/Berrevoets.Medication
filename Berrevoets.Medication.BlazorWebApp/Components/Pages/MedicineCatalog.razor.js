window.bootstrapToast = {
    show: function(toastId, title, message) {
        const toastEl = document.getElementById(toastId);
        if (!toastEl) return;
        document.getElementById("toastTitle").textContent = title;
        document.getElementById("toastMessage").textContent = message;
        // Remove any previous status classes
        toastEl.classList.remove("success", "error");
        // Add a class based on the title
        if (title.toLowerCase() === "success") {
            toastEl.classList.add("success");
        } else if (title.toLowerCase() === "error") {
            toastEl.classList.add("error");
        }
        const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
        toast.show();
    }
};