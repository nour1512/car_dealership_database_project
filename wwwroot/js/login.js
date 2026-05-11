document.addEventListener("DOMContentLoaded", function () {
    const staffTab = document.getElementById("staff-tab");
    const customerTab = document.getElementById("customer-tab");
  
    const staffForm = document.getElementById("staff-form");
    const customerLoginForm = document.getElementById("customer-login-form");
    const customerSignupForm = document.getElementById("customer-signup-form");
  
    const switchToCustomerSignup = document.getElementById("switch-to-customer-signup");
    const switchToCustomerLogin = document.getElementById("switch-to-customer-login");
  
    // Switch to Customer Signup
    switchToCustomerSignup.addEventListener("click", () => {
      customerSignupForm.classList.add("active");
      customerLoginForm.classList.remove("active");
    });
  
    // Switch to Customer Login
    switchToCustomerLogin.addEventListener("click", () => {
      customerSignupForm.classList.remove("active");
      customerLoginForm.classList.add("active");
    });
  
    // Switch between Staff and Customer Tabs
    staffTab.addEventListener("click", () => {
      staffTab.classList.add("active");
      customerTab.classList.remove("active");
      staffForm.classList.add("active");
      customerLoginForm.classList.remove("active");
      customerSignupForm.classList.remove("active");
    });
  
    customerTab.addEventListener("click", () => {
      customerTab.classList.add("active");
      staffTab.classList.remove("active");
      customerLoginForm.classList.add("active");
      staffForm.classList.remove("active");
      customerSignupForm.classList.remove("active");
    });
  });
  function validateStaffForm(event) {
    event.preventDefault();
    var staffId = document.getElementById('staff-id').value.trim();
    var staffPassword = document.getElementById('staff-password').value.trim();

    if (staffId === "" || staffPassword === "") {
      alert("Please fill in all fields.");
      return false; // Prevent form submission
    }

    window.location.href = '../carvilla-v1.0/dashboard/index.html';
    return true; // Allow form submission if validation passes
  }

  // Customer login form validation
  function validateCustomerLogin(event) {
    event.preventDefault();
    var email = document.getElementById('customer-email').value.trim();
    var password = document.getElementById('customer-password').value.trim();

    if (email === "" || password === "") {
      alert("Please fill in all fields.");
      return false; // Prevent form submission
    }

    window.location.href="../carvilla-v1.0/index.html";
    return true; // Allow form submission if validation passes
  }

  // Customer signup form validation
  function validateCustomerSignup(event) {
    event.preventDefault();
    var name = document.getElementById('customer-name').value.trim();
    var email = document.getElementById('signup-email').value.trim();
    var password = document.getElementById('signup-password').value.trim();
    var confirmPassword = document.getElementById('confirm-password').value.trim();

    if (name === "" || email === "" || password === "" || confirmPassword === "") {
      alert("Please fill in all fields.");
      return false; // Prevent form submission
    }

    if (password !== confirmPassword) {
      alert("Passwords do not match.");
      return false; // Prevent form submission
    }

    window.location.href='../carvilla-v1.0/index.html';

    return true; // Allow form submission if validation passes
  }
  