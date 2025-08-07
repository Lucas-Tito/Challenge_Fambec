// Firebase configuration
const firebaseConfig = {
    apiKey: "AIzaSyC2_xBoWKUzY_a-qeuVmyiKkdXOqfCM-No",
    authDomain: "challenge-fambec.firebaseapp.com",
    projectId: "challenge-fambec",
    storageBucket: "challenge-fambec.firebasestorage.app",
    messagingSenderId: "446280565359",
    appId: "1:446280565359:web:3795b6ad4e01b3f1c5039a"
};

// Firebase Auth functions
window.firebaseAuth = {
    app: null,
    auth: null,
    
    // Initialize Firebase
    initialize: async function() {
        try {
            // Import Firebase modules dynamically using the latest version
            const { initializeApp } = await import('https://www.gstatic.com/firebasejs/12.0.0/firebase-app.js');
            const { getAuth, GoogleAuthProvider, signInWithPopup, signOut, onAuthStateChanged } = 
                await import('https://www.gstatic.com/firebasejs/12.0.0/firebase-auth.js');
            
            // Initialize Firebase
            this.app = initializeApp(firebaseConfig);
            this.auth = getAuth(this.app);
            
            // Store auth functions globally for easier access
            this.GoogleAuthProvider = GoogleAuthProvider;
            this.signInWithPopup = signInWithPopup;
            this.firebaseSignOut = signOut; // Store Firebase signOut function with different name
            this.onAuthStateChanged = onAuthStateChanged;
            
            console.log('Firebase initialized successfully');
            return true;
        } catch (error) {
            console.error('Error initializing Firebase:', error);
            return false;
        }
    },
    
    // Sign in with Google
    signInWithGoogle: async function() {
        try {
            if (!this.auth) {
                await this.initialize();
            }
            
            const provider = new this.GoogleAuthProvider();
            // Request additional scopes if needed
            provider.addScope('profile');
            provider.addScope('email');
            
            // Configure the provider with custom parameters
            provider.setCustomParameters({
                'login_hint': '',
                'prompt': 'select_account'
            });
            
            const result = await this.signInWithPopup(this.auth, provider);
            const user = result.user;
            
            // Get the ID token
            const idToken = await user.getIdToken();
            
            console.log('Google sign-in successful:', user.email);
            return idToken;
        } catch (error) {
            console.error('Error during Google sign-in:', error);
            
            // Handle specific error codes
            if (error.code === 'auth/popup-closed-by-user') {
                console.log('Sign-in popup was closed by user');
                throw new Error('Login cancelado pelo usuário');
            } else if (error.code === 'auth/popup-blocked') {
                console.log('Sign-in popup was blocked by browser');
                throw new Error('Popup bloqueado pelo navegador. Verifique as configurações de popup.');
            } else if (error.code === 'auth/unauthorized-domain') {
                console.log('Domain not authorized');
                throw new Error('Domínio não autorizado para Firebase Auth');
            } else if (error.code === 'auth/operation-not-allowed') {
                console.log('Google sign-in not enabled');
                throw new Error('Login com Google não está habilitado');
            }
            
            throw error;
        }
    },
    
    // Sign out
    signOut: async function() {
        try {
            if (!this.auth) {
                await this.initialize();
            }
            
            await this.firebaseSignOut(this.auth);
            console.log('User signed out successfully');
        } catch (error) {
            console.error('Error during sign out:', error);
            throw error;
        }
    },
    
    // Get current user
    getCurrentUser: function() {
        if (!this.auth) {
            return null;
        }
        return this.auth.currentUser;
    },
    
    // Listen to auth state changes
    onAuthStateChanged: function(callback) {
        if (!this.auth) {
            this.initialize().then(() => {
                this.onAuthStateChanged(this.auth, callback);
            });
            return;
        }
        this.onAuthStateChanged(this.auth, callback);
    }
};

// Initialize Firebase when the script loads
document.addEventListener('DOMContentLoaded', function() {
    window.firebaseAuth.initialize();
});
