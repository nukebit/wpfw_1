// Projects
/**
 * Data for one project card: four fields.
 * @typedef {Object} Project
 * @property {string} type - Filter value: applicatie or dashboard.
 * @property {string} title - Project name.
 * @property {string} image -  Folder path to the image.
 * @property {string} description - Short project summary.
 */
const projects = [
    {
        type: "applicatie",
        title: "Task management platform",
        image: "../images/projects/task-management.png",
        description: "Een applicatie voor het maken van projecten, verdelen van taken en volgen van de voortgang."
    },
    {
        type: "applicatie",
        title: "Event booking application",
        image: "../images/projects/event-booking.png",
        description: "Een applicatie waarin gebruikers evenementen bekijken, plaatsen reserveren en boekingen beheren."
    },
    {
        type: "dashboard",
        title: "Inventory dashboard",
        image: "../images/projects/inventory-dashboard.png",
        description: "Een dashboard voor productbeheer, voorraadupdates en meldingen bij lage voorraad."
    }
];

const projectList = document.querySelector("#project-list");
const projectCount = document.querySelector("#project-count");
const projectFilter = document.querySelector("#project-filter");
const projectSort = document.querySelector("#project-sort");

/**
 * Creates an HTML element with an optional class and text.
 * @param {string} tag - HTML tag, such as "p" or "section".
 * @param {string} [className] - CSS class, omitted means no class.
 * @param {string} [text] - Text content, omitted means no text.
 * @returns {HTMLElement} The new element.
 */
function makeElement(tag, className, text) {
    const element = document.createElement(tag);
    if (className) element.className = className;
    if (text) element.textContent = text;
    return element;
}

/**
 * Builds a complete project card from project data.
 * @param {Project} project - Data for the project.
 * @param {number} index - Zero based position in the visible list.
 * @returns {HTMLElement} The new article element.
 */
function makeProject(project, index) {
    const article = makeElement("article", "project");
    article.id = project.title.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "");

    const number = makeElement("span", "", String(index + 1).padStart(2, "0"));
    number.setAttribute("aria-hidden", "true");

    const header = makeElement("header");
    header.append(makeElement("p", "meta", project.type === "applicatie" ? "Applicatie" : "Dashboard"), makeElement("h2", "", project.title));

    const figure = makeElement("figure", "project-visual");
    const imageLink = makeElement("a");
    imageLink.href = project.image;
    imageLink.setAttribute("aria-label", `Bekijk de conceptpreview van ${project.title} op volledige grootte`);
    const image = makeElement("img", "project-image");
    image.src = project.image;
    image.alt = `Conceptpreview van ${project.title}.`;
    image.width = 1672;
    image.height = 941;
    image.loading = "lazy";
    image.decoding = "async";
    imageLink.append(image);
    figure.append(imageLink, makeElement("figcaption", "", "Conceptpreview"));

    article.append(number, header, figure, makeElement("p", "", project.description));
    return article;
}

/**
 * Checks whether a project matches the selected filter.
 * @param {Project} project - Project to check.
 * @returns {boolean} True if the project should be visible.
 */
function matchesSelectedType(project) {
    return projectFilter.value === "alle" || project.type === projectFilter.value;
}

/**
 * Compares two project names for a-z sorting.
 * @param {Project} first - First project.
 * @param {Project} second - Second project.
 * @returns {number} The ordering value for Array.sort().
 */
function compareProjectNames(first, second) {
    return first.title.localeCompare(second.title, "nl");
}

/**
 * Filters, sorts, and renders the visible project list.
 * reads the current filter and sort controls.
 * @returns {void} Updates the page without returning a value.
 */
function renderProjects() {
    const visibleProjects = projects.filter(matchesSelectedType);

    if (projectSort.value === "naam") {
        visibleProjects.sort(compareProjectNames);
    }

    projectList.replaceChildren();
    for (const [index, project] of visibleProjects.entries()) {
        projectList.append(makeProject(project, index));
    }
    projectCount.textContent = `${visibleProjects.length} ${visibleProjects.length === 1 ? "project" : "projecten"} gevonden`;
}

if (projectList) {
    projectFilter.addEventListener("change", renderProjects);
    projectSort.addEventListener("change", renderProjects);
    renderProjects();
    const linkedProject = document.getElementById(window.location.hash.slice(1));
    if (linkedProject) linkedProject.scrollIntoView();
}

// Contact form
/**
 * A form input and its error message element.
 * @typedef {Object} FormField
 * @property {HTMLInputElement|HTMLTextAreaElement} input - The form input.
 * @property {HTMLElement} error - The error message for that input.
 */
const contactForm = document.querySelector("#contact-form");
const formStatus = document.querySelector("#form-status");
const fields = [
    { input: document.querySelector("#contact-name"), error: document.querySelector("#name-error") },
    { input: document.querySelector("#contact-email"), error: document.querySelector("#email-error") },
    { input: document.querySelector("#contact-message"), error: document.querySelector("#message-error") }
];

/**
 * Determines the error message for the current input value.
 * @param {HTMLInputElement|HTMLTextAreaElement} input - Input to validate.
 * @returns {string} An error message, or an empty string if valid.
 */
function getErrorMessage(input) {
    const value = input.value.trim();
    if (!value) return "This field is required.";
    if (input.name === "email" && input.validity.typeMismatch) {
        return "Enter a valid email address, for example name@example.com.";
    }
    if (input.name === "message" && value.length < 10) {
        return "Write a message of at least 10 characters.";
    }
    return "";
}

/**
 * Validates one input and shows or clears its error message.
 * @param {FormField} field - Input and its error message element.
 * @returns {boolean} True if the input is valid.
 */
function validateField(field) {
    const message = getErrorMessage(field.input);
    field.error.textContent = message;
    if (message) {
        field.input.setAttribute("aria-invalid", "true");
    } else {
        field.input.removeAttribute("aria-invalid");
    }
    return !message;
}

/**
 * Clears old feedback when the user changes a form field.
 * @param {Event} event - Input event from a form field.
 * @returns {void} Updates the page without returning a value.
 */
function clearFieldFeedback(event) {
    const input = event.target;
    document.getElementById(input.getAttribute("aria-describedby")).textContent = "";
    input.removeAttribute("aria-invalid");
    formStatus.textContent = "";
}

/**
 * Prevents submission, validates all inputs, and shows feedback.
 * @param {SubmitEvent} event - Submit event from the contact form.
 * @returns {void} Updates the page without returning a value.
 */
function handleContactSubmit(event) {
    event.preventDefault();
    let firstInvalidInput = null;
    for (const field of fields) {
        if (!validateField(field) && !firstInvalidInput) {
            firstInvalidInput = field.input;
        }
    }
    if (firstInvalidInput) {
        formStatus.textContent = "Please check the highlighted fields.";
        firstInvalidInput.focus();
        return;
    }
    formStatus.textContent = "Your info is valid. No message sent, this is a demo.";
}

if (contactForm) {
    contactForm.addEventListener("input", clearFieldFeedback);
    contactForm.addEventListener("submit", handleContactSubmit);
}

// Weather
const weatherStatus = document.querySelector("#weather-status");
const weatherResult = document.querySelector("#weather-result");
const weatherRetry = document.querySelector("#weather-retry");
const weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=52.08&longitude=4.31&current=temperature_2m,relative_humidity_2m&timezone=Europe%2FAmsterdam";

/**
 * Displays temperature and humidity from the API response.
 * @param {{temperature_2m: number, relative_humidity_2m: number}} weather - Current weather values.
 * @returns {void} Updates the page without returning a value.
 */
function showWeather(weather) {
    const temperature = document.createElement("p");
    temperature.className = "weather-value";
    temperature.textContent = `${Math.round(weather.temperature_2m)} °C`;

    const humidity = document.createElement("p");
    humidity.textContent = `Luchtvochtigheid: ${weather.relative_humidity_2m}%`;

    weatherResult.replaceChildren(temperature, humidity);
}

/**
 * Fetches current weather and shows loading, success, or error feedback.
 * fetches data for the fixed location in weatherUrl.
 * @returns {Promise<void>} Resolves after the fetch and display steps finish.
 */
async function loadWeather() {
    weatherStatus.textContent = "The weather is loading...";
    weatherResult.replaceChildren();
    weatherRetry.hidden = true;

    try {
        const response = await fetch(weatherUrl);
        if (!response.ok) throw new Error("The weather API is unavailable");
        const data = await response.json();
        if (!data.current || !Number.isFinite(data.current.temperature_2m) || !Number.isFinite(data.current.relative_humidity_2m)) {
            throw new Error("No current weather data is available");
        }
        showWeather(data.current);
        weatherStatus.textContent = "Weather in The Hague";
    } catch {
        weatherStatus.textContent = "The weather could not be loaded. Please try again later.";
        weatherRetry.hidden = false;
    }
}

if (weatherStatus) {
    weatherRetry.addEventListener("click", loadWeather);
    loadWeather();
}
