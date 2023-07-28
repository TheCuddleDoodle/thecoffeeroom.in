// Define Vue components
const Home = {
    template: '<div><h1>Home Page</h1><p>Welcome to the home page!</p></div>',
};

const About = {
    template: '<div><h1>About Page</h1><p>This is the about page of our app.</p></div>',
};

// Define routes
const routes = [
    { path: '/', component: Home },
    { path: '/about', component: About },
];

// Create and configure Vue Router
const router = new VueRouter({
    routes,
    mode: 'hash', // Use hash routing
});

// Create and mount Vue instance
new Vue({
    el: '#app',
    router,
});