// Theme management functions
window.toggleTheme = function() {
    const isDark = document.documentElement.classList.contains('dark');
    const newTheme = !isDark;
    
    if (newTheme) {
        document.documentElement.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    } else {
        document.documentElement.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    }
    
    return newTheme;
};

window.updateThemeIcons = function() {
    const isDark = document.documentElement.classList.contains('dark');
    const themeButton = document.querySelector('#theme-toggle');
    
    if (themeButton) {
        const moonIcon = themeButton.querySelector('svg[data-theme-target="moon"]');
        const sunIcon = themeButton.querySelector('svg[data-theme-target="sun"]');
        
        if (moonIcon && sunIcon) {
            if (isDark) {
                moonIcon.style.display = 'none';
                sunIcon.style.display = 'block';
            } else {
                moonIcon.style.display = 'block';
                sunIcon.style.display = 'none';
            }
        }
    }
};

window.initTheme = function() {
    const savedTheme = localStorage.getItem('theme');
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const isDark = savedTheme ? savedTheme === 'dark' : prefersDark;
    
    if (isDark) {
        document.documentElement.classList.add('dark');
    } else {
        document.documentElement.classList.remove('dark');
    }
    
    // Update icons after theme is applied
    setTimeout(() => window.updateThemeIcons(), 100);
    
    return isDark;
};

window.getCurrentTheme = function() {
    return document.documentElement.classList.contains('dark');
};

// Initialize theme on page load
document.addEventListener('DOMContentLoaded', function() {
    window.initTheme();
});
