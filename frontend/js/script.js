// Projects
/**
 * The data used for a project card.
 * @typedef {Object} Project
 * @property {string} type - Project type: applicatie or dashboard.
 * @property {string} title - Project name.
 * @property {string} image - Path to the project image.
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
 * Makes an HTML element with a class and text.
 * @param {string} tag - The HTML tag, such as "p".
 * @param {string} [className=""] - Optional CSS class.
 * @param {string} [text=""] - Optional text to show.
 * @returns {HTMLElement} The new element.
 */
function makeElement(tag, className = "", text = "") {
    const element = document.createElement(tag);
    element.className = className;
    element.textContent = text;
    return element;
}

/**
 * Makes a project card.
 * @param {Project} project - The project's data.
 * @param {number} index - Its position in the list, starting at zero.
 * @returns {HTMLElement} The project card.
 */
function makeProject(project, index) {
    const article = makeElement("article", "project");
    article.id = project.title.toLowerCase().replace(/[^a-z0-9]+/g, "-").replace(/^-|-$/g, "");

    const number = makeElement("span", "", String(index + 1).padStart(2, "0"));
    number.setAttribute("aria-hidden", "true");

    const heading = makeElement("div", "project-heading");
    heading.append(
        makeElement("p", "meta", project.type === "applicatie" ? "Applicatie" : "Dashboard"),
        makeElement("h2", "", project.title)
    );

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

    article.append(number, heading, figure, makeElement("p", "", project.description));
    return article;
}

/**
 * Checks if a project matches the filter.
 * @param {Project} project - The project to check.
 * @returns {boolean} True if the project should be visible.
 */
function matchesSelectedType(project) {
    return projectFilter.value === "alle" || project.type === projectFilter.value;
}

/**
 * Compares project names to sort them from A to Z.
 * @param {Project} first - The first project.
 * @param {Project} second - The second project.
 * @returns {number} A negative, zero, or positive number to set their order.
 */
function compareProjectNames(first, second) {
    return first.title.localeCompare(second.title, "nl");
}

/**
 * Shows the projects using the chosen filter and sort order.
 * @returns {void} Updates the page.
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
 * A form input and its error message.
 * @typedef {Object} FormField
 * @property {HTMLInputElement|HTMLTextAreaElement} input - The form input.
 * @property {HTMLElement} error - The error message for that input.
 * @property {string} message - Text to show when the input is invalid.
 */
const contactForm = document.querySelector("#contact-form");
const formStatus = document.querySelector("#form-status");
const fields = [
    { input: document.querySelector("#contact-name"), error: document.querySelector("#name-error"), message: "Enter at least 2 characters." },
    { input: document.querySelector("#contact-email"), error: document.querySelector("#email-error"), message: "Enter a valid email address." },
    { input: document.querySelector("#contact-message"), error: document.querySelector("#message-error"), message: "Enter at least 10 characters." }
];

/**
 * Checks an input and shows its error message.
 * @param {FormField} field - The input, error element, and message.
 * @returns {boolean} True when the input is valid.
 */
function validateField(field) {
    const valid = field.input.checkValidity();
    field.input.setAttribute("aria-invalid", String(!valid));
    field.error.textContent = valid ? "" : field.message;
    return valid;
}

/**
 * Clears errors when the user changes an input.
 * @param {Event} event - The input event.
 * @returns {void} Updates the page.
 */
function clearFieldFeedback(event) {
    const input = event.target;
    document.getElementById(input.getAttribute("aria-describedby")).textContent = "";
    input.setAttribute("aria-invalid", "false");
    formStatus.textContent = "";
}

/**
 * Checks the form and shows whether the input is valid.
 * @param {SubmitEvent} event - The form's submit event.
 * @returns {void} Updates the page.
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

/**
 * Shows the temperature and humidity.
 * @param {{temperature_2m: number, relative_humidity_2m: number}} weather - The current weather.
 * @returns {void} Updates the page.
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
 * Loads the weather and shows a loading message, result, or error.
 * @returns {Promise<void>} Finishes when the page has been updated.
 */
async function loadWeather() {
    weatherStatus.textContent = "The weather is loading...";
    weatherResult.replaceChildren();
    weatherRetry.hidden = true;

    try {
        const weather = await getWeather();
        showWeather(weather);
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
