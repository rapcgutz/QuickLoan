// QuickLoan Web - Site JavaScript

// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function() {
    const alerts = document.querySelectorAll('.alert:not(.alert-static)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });
});

// Format currency inputs
function formatCurrency(value) {
    return '$' + parseFloat(value).toLocaleString('en-US', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    });
}

// Format currency with decimals
function formatCurrencyWithDecimals(value) {
    return '$' + parseFloat(value).toLocaleString('en-US', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// Validate Australian mobile number
function validateMobile(mobile) {
    const pattern = /^04\d{8}$/;
    return pattern.test(mobile.replace(/\s/g, ''));
}

// Format mobile number as user types
function formatMobileInput(input) {
    let value = input.value.replace(/\s/g, '');
    if (value.length > 4 && value.length <= 7) {
        value = value.slice(0, 4) + ' ' + value.slice(4);
    } else if (value.length > 7) {
        value = value.slice(0, 4) + ' ' + value.slice(4, 7) + ' ' + value.slice(7, 10);
    }
    input.value = value;
}

// Add mobile formatting to all mobile inputs
document.addEventListener('DOMContentLoaded', function() {
    const mobileInputs = document.querySelectorAll('input[name="Mobile"]');
    mobileInputs.forEach(input => {
        input.addEventListener('input', function() {
            formatMobileInput(this);
        });
    });
});

// Confirm before logout
function confirmLogout() {
    return confirm('Are you sure you want to logout?');
}

// Smooth scroll to top
function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

// Log API errors to console (development only)
function logApiError(error) {
    if (console && console.error) {
        console.error('API Error:', error);
    }
}
