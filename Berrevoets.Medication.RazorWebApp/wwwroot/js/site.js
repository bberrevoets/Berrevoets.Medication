window.showToast = function(title, message, toastType) {
    // Optionally, you can add logic to change colors based on toastType (e.g. success, error)
    const toastEl = document.getElementById("notificationToast");
    document.getElementById("toastTitle").textContent = title;
    document.getElementById("toastBody").textContent = message;

    // Remove any previous type classes and add the new one if needed
    toastEl.classList.remove("bg-success", "bg-danger", "text-white");
    if (toastType === "success") {
        toastEl.classList.add("bg-success", "text-white");
    } else if (toastType === "error") {
        toastEl.classList.add("bg-danger", "text-white");
    }

    const toast = new bootstrap.Toast(toastEl, { delay: 3000 });
    toast.show();
};